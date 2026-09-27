using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyThuVien.Data;
using WebsiteQuanLyThuVien.DTOs;
using WebsiteQuanLyThuVien.DTOs.Requests;
using WebsiteQuanLyThuVien.DTOs.Response;
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories;

public class BookRepository : IBookRepository
{
    private readonly ApplicationDbContext _context;

    public BookRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<BookSearchItemDto>> SearchAsync(
        BookSearchRequest request,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Books.AsNoTracking().AsQueryable();

        /*
         * Keyword
         */
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var keyword = request.Keyword.Trim();
            query = query.Where(book => book.Title.Contains(keyword));
        }

        /*
         * Filter expression
         *
         * Ví dụ:
         *
         * categories:("Âm nhạc" | "Công nghệ thông tin")
         * categories:("Âm nhạc" & "Công nghệ thông tin")
         * categories:("Âm nhạc" | "Văn học")
         * &
         * authors:("Nguyễn Du" | "Nam Cao")
         */
        if (!string.IsNullOrWhiteSpace(request.FilterExpression))
        {
            query = await ApplyFilterExpressionAsync(
                query,
                request.FilterExpression,
                cancellationToken);
        }

        /*
         * Publication year
         */
        if (request.PublicationYear.HasValue)
        {
            query = query.Where(book => book.PublicationYear == request.PublicationYear.Value);
        }

        /*
         * Availability status
         */
        switch (request.Status?.ToLowerInvariant())
        {
            case "available":
                query = query.Where(book => book.BookCopies.Any(copy => copy.Status.ToString() == "AVAILABLE"));
                break;

            case "unavailable":
                query = query.Where(book => !book.BookCopies.Any(copy => copy.Status.ToString() == "AVAILABLE"));
                break;
        }

        /*
         * Sorting
         */
        query = request.Sort?.ToLowerInvariant() switch
        {
            "title" => query.OrderBy(book => book.Title),
            "year" => query.OrderByDescending(book => book.PublicationYear).ThenBy(book => book.Title),
            "newest" => query.OrderByDescending(book => book.CreatedAt),
            _ => query.OrderBy(book => book.Title)
        };

        /*
         * Total count
         */
        var totalCount = await query.CountAsync(cancellationToken);

        /*
         * Pagination
         */
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 12 : request.PageSize;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(book => new BookSearchItemDto
            {
                Id = book.Id,
                Title = book.Title,
                PublicationYear = book.PublicationYear,
                PublisherName = book.Publisher != null ? book.Publisher.Name : null,
                Authors = book.BookAuthors.Select(ba => ba.Author.Name).ToList(),
                Categories = book.BookCategories.Select(bc => bc.Category.Name).ToList()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<BookSearchItemDto>
        {
            Items = items,
            TotalItems = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    private async Task<IQueryable<Book>> ApplyFilterExpressionAsync(
        IQueryable<Book> query,
        string expression,
        CancellationToken cancellationToken)
    {
        var tokens = TokenizeFilterExpression(expression);

        if (tokens.Count == 0)
        {
            return query;
        }

        var parser = new FilterExpressionParser(tokens);
        var ast = parser.Parse();

        var predicate = await BuildFilterExpressionAsync(ast, cancellationToken);
        return query.Where(predicate);
    }

    private abstract record BookFilterNode;

    private sealed record FilterCondition(string Field, string Value) : BookFilterNode;

    private sealed record FilterAnd(BookFilterNode Left, BookFilterNode Right) : BookFilterNode;

    private sealed record FilterOr(BookFilterNode Left, BookFilterNode Right) : BookFilterNode;

    private enum FilterTokenType
    {
        Field,
        Value,
        Colon,
        And,
        Or,
        OpenParenthesis,
        CloseParenthesis
    }

    private sealed record FilterToken(FilterTokenType Type, string Value);

    private static List<FilterToken> TokenizeFilterExpression(string expression)
    {
        var tokens = new List<FilterToken>();
        var position = 0;

        while (position < expression.Length)
        {
            /*
             * Skip whitespace
             */
            while (position < expression.Length && char.IsWhiteSpace(expression[position]))
            {
                position++;
            }

            if (position >= expression.Length)
            {
                break;
            }

            var current = expression[position];

            /*
             * AND
             */
            if (current == '&')
            {
                tokens.Add(new FilterToken(FilterTokenType.And, "&"));
                position++;
                continue;
            }

            /*
             * OR
             */
            if (current == '|')
            {
                tokens.Add(new FilterToken(FilterTokenType.Or, "|"));
                position++;
                continue;
            }

            /*
             * Open parenthesis
             */
            if (current == '(')
            {
                tokens.Add(new FilterToken(FilterTokenType.OpenParenthesis, "("));
                position++;
                continue;
            }

            /*
             * Close parenthesis
             */
            if (current == ')')
            {
                tokens.Add(new FilterToken(FilterTokenType.CloseParenthesis, ")"));
                position++;
                continue;
            }

            /*
             * Quoted value
             */
            if (current == '"')
            {
                var value = ReadQuotedValue(expression, ref position);
                tokens.Add(new FilterToken(FilterTokenType.Value, value));
                continue;
            }

            /*
             * Field
             */
            var start = position;

            while (
                position < expression.Length &&
                !char.IsWhiteSpace(expression[position]) &&
                expression[position] != ':' &&
                expression[position] != '&' &&
                expression[position] != '|' &&
                expression[position] != '(' &&
                expression[position] != ')')
            {
                position++;
            }

            if (position == start)
            {
                throw new ArgumentException("Filter expression không hợp lệ.");
            }

            var field = expression[start..position].Trim().ToLowerInvariant();

            if (!IsSupportedFilterField(field))
            {
                throw new ArgumentException($"Field filter không được hỗ trợ: {field}");
            }

            tokens.Add(new FilterToken(FilterTokenType.Field, field));

            /*
             * Colon
             */
            if (position >= expression.Length || expression[position] != ':')
            {
                throw new ArgumentException($"Field '{field}' phải có dấu ':'.");
            }

            tokens.Add(new FilterToken(FilterTokenType.Colon, ":"));
            position++;
        }

        return tokens;
    }

    private static string ReadQuotedValue(string expression, ref int position)
    {
        /*
         * Bỏ dấu "
         */
        position++;

        var result = new System.Text.StringBuilder();

        while (position < expression.Length)
        {
            var current = expression[position];

            /*
             * Escape
             */
            if (current == '\\')
            {
                if (position + 1 >= expression.Length)
                {
                    throw new ArgumentException("Filter expression có escape không hợp lệ.");
                }

                var next = expression[position + 1];

                if (next == '"' || next == '\\')
                {
                    result.Append(next);
                    position += 2;
                    continue;
                }

                result.Append(next);
                position += 2;
                continue;
            }

            /*
             * End quote
             */
            if (current == '"')
            {
                position++;
                return result.ToString();
            }

            result.Append(current);
            position++;
        }

        throw new ArgumentException("Filter expression có chuỗi chưa đóng dấu \".");
    }

    private static bool IsSupportedFilterField(string field)
    {
        return field switch
        {
            "categories" => true,
            "authors" => true,
            "publishers" => true,
            "document_types" => true,
            _ => false
        };
    }

    private sealed class FilterExpressionParser
    {
        private readonly List<FilterToken> _tokens;
        private int _position;

        public FilterExpressionParser(List<FilterToken> tokens)
        {
            _tokens = tokens;
        }

        public BookFilterNode Parse()
        {
            if (_tokens.Count == 0)
            {
                throw new ArgumentException("Filter expression không được rỗng.");
            }

            var result = ParseOr();

            if (_position != _tokens.Count)
            {
                throw new ArgumentException("Filter expression không hợp lệ.");
            }

            return result;
        }

        /*
         * OR có precedence thấp hơn AND.
         *
         * A | B & C
         *
         * được hiểu là:
         *
         * A | (B & C)
         */
        private BookFilterNode ParseOr()
        {
            var left = ParseAnd();

            while (CurrentType() == FilterTokenType.Or)
            {
                Advance();

                var right = ParseAnd();
                left = new FilterOr(left, right);
            }

            return left;
        }

        /*
         * AND có precedence cao hơn OR.
         */
        private BookFilterNode ParseAnd()
        {
            var left = ParsePrimary();

            while (CurrentType() == FilterTokenType.And)
            {
                Advance();

                var right = ParsePrimary();
                left = new FilterAnd(left, right);
            }

            return left;
        }

        private BookFilterNode ParsePrimary()
        {
            /*
             * Generic parenthesis
             *
             * (A | B)
             */
            if (CurrentType() == FilterTokenType.OpenParenthesis)
            {
                Advance();

                var expression = ParseOr();
                Expect(FilterTokenType.CloseParenthesis);

                return expression;
            }

            /*
             * field:(...)
             */
            return ParseFieldExpression();
        }

        private BookFilterNode ParseFieldExpression()
        {
            var field = Expect(FilterTokenType.Field);

            Expect(FilterTokenType.Colon);
            Expect(FilterTokenType.OpenParenthesis);

            var expression = ParseFieldValues(field.Value);
            Expect(FilterTokenType.CloseParenthesis);

            return expression;
        }

        /*
         * Parse:
         *
         * "A"
         * "A" & "B"
         * "A" | "B"
         * "A" | "B" & "C"
         */
        private BookFilterNode ParseFieldValues(string field)
        {
            var left = ParseFieldValue(field);

            while (CurrentType() == FilterTokenType.And ||
                   CurrentType() == FilterTokenType.Or)
            {
                var operatorType = CurrentType();
                Advance();

                var right = ParseFieldValue(field);

                left = operatorType == FilterTokenType.And
                    ? new FilterAnd(left, right)
                    : new FilterOr(left, right);
            }

            return left;
        }

        private BookFilterNode ParseFieldValue(string field)
        {
            var value = Expect(FilterTokenType.Value);
            return new FilterCondition(field, value.Value);
        }

        private FilterToken Expect(FilterTokenType type)
        {
            if (_position >= _tokens.Count)
            {
                throw new ArgumentException($"Thiếu token {type} trong filter expression.");
            }

            var token = _tokens[_position];

            if (token.Type != type)
            {
                throw new ArgumentException(
                    $"Filter expression không hợp lệ. " +
                    $"Đang cần {type} nhưng nhận được {token.Type}.");
            }

            _position++;
            return token;
        }

        private FilterTokenType? CurrentType()
        {
            if (_position >= _tokens.Count)
            {
                return null;
            }

            return _tokens[_position].Type;
        }

        private void Advance()
        {
            if (_position < _tokens.Count)
            {
                _position++;
            }
        }
    }

    // ============================================================
    // RESOLVE FILTER VALUES
    // ============================================================

    private async Task<Expression<Func<Book, bool>>> BuildFilterExpressionAsync(
        BookFilterNode ast,
        CancellationToken cancellationToken)
    {
        var conditions = CollectFilterConditions(ast);

        var categoryIds = await ResolveCategoryIdsAsync(conditions, cancellationToken);
        var authorIds = await ResolveAuthorIdsAsync(conditions, cancellationToken);
        var publisherIds = await ResolvePublisherIdsAsync(conditions, cancellationToken);
        var documentTypeIds = await ResolveDocumentTypeIdsAsync(conditions, cancellationToken);

        var parameter = Expression.Parameter(typeof(Book), "book");

        var body = BuildFilterBody(
            ast,
            parameter,
            categoryIds,
            authorIds,
            publisherIds,
            documentTypeIds);

        return Expression.Lambda<Func<Book, bool>>(body, parameter);
    }

    private static List<FilterCondition> CollectFilterConditions(BookFilterNode node)
    {
        var result = new List<FilterCondition>();
        CollectFilterConditions(node, result);
        return result;
    }

    private static void CollectFilterConditions(
        BookFilterNode node,
        List<FilterCondition> result)
    {
        switch (node)
        {
            case FilterCondition condition:
                result.Add(condition);
                break;

            case FilterAnd and:
                CollectFilterConditions(and.Left, result);
                CollectFilterConditions(and.Right, result);
                break;

            case FilterOr or:
                CollectFilterConditions(or.Left, result);
                CollectFilterConditions(or.Right, result);
                break;
        }
    }

    private async Task<Dictionary<string, Guid>> ResolveCategoryIdsAsync(
        List<FilterCondition> conditions,
        CancellationToken cancellationToken)
    {
        var names = conditions
            .Where(x => x.Field == "categories")
            .Select(x => x.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (names.Count == 0)
        {
            return new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        }

        var categories = await _context.Categories
            .Where(category => names.Contains(category.Name))
            .Select(category => new
            {
                category.Id,
                category.Name
            })
            .ToListAsync(cancellationToken);

        return categories.ToDictionary(
            x => x.Name,
            x => x.Id,
            StringComparer.OrdinalIgnoreCase);
    }

    private async Task<Dictionary<string, Guid>> ResolveAuthorIdsAsync(
        List<FilterCondition> conditions,
        CancellationToken cancellationToken)
    {
        var names = conditions
            .Where(x => x.Field == "authors")
            .Select(x => x.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (names.Count == 0)
        {
            return new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        }

        var authors = await _context.Authors
            .Where(author => names.Contains(author.Name))
            .Select(author => new
            {
                author.Id,
                author.Name
            })
            .ToListAsync(cancellationToken);

        return authors.ToDictionary(
            x => x.Name,
            x => x.Id,
            StringComparer.OrdinalIgnoreCase);
    }

    private async Task<Dictionary<string, Guid>> ResolvePublisherIdsAsync(
        List<FilterCondition> conditions,
        CancellationToken cancellationToken)
    {
        var names = conditions
            .Where(x => x.Field == "publishers")
            .Select(x => x.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (names.Count == 0)
        {
            return new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        }

        var publishers = await _context.Publishers
            .Where(publisher => names.Contains(publisher.Name))
            .Select(publisher => new
            {
                publisher.Id,
                publisher.Name
            })
            .ToListAsync(cancellationToken);

        return publishers.ToDictionary(
            x => x.Name,
            x => x.Id,
            StringComparer.OrdinalIgnoreCase);
    }

    private async Task<Dictionary<string, Guid>> ResolveDocumentTypeIdsAsync(
        List<FilterCondition> conditions,
        CancellationToken cancellationToken)
    {
        var names = conditions
            .Where(x => x.Field == "document_types")
            .Select(x => x.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (names.Count == 0)
        {
            return new Dictionary<string, Guid>(StringComparer.OrdinalIgnoreCase);
        }

        var documentTypes = await _context.DocumentTypes
            .Where(documentType => names.Contains(documentType.Name))
            .Select(documentType => new
            {
                documentType.Id,
                documentType.Name
            })
            .ToListAsync(cancellationToken);

        return documentTypes.ToDictionary(
            x => x.Name,
            x => x.Id,
            StringComparer.OrdinalIgnoreCase);
    }

    private static Expression BuildFilterBody(
        BookFilterNode node,
        ParameterExpression parameter,
        Dictionary<string, Guid> categoryIds,
        Dictionary<string, Guid> authorIds,
        Dictionary<string, Guid> publisherIds,
        Dictionary<string, Guid> documentTypeIds)
    {
        switch (node)
        {
            case FilterCondition condition:
                return BuildCondition(
                    condition,
                    parameter,
                    categoryIds,
                    authorIds,
                    publisherIds,
                    documentTypeIds);

            case FilterAnd and:
                {
                    var left = BuildFilterBody(
                        and.Left,
                        parameter,
                        categoryIds,
                        authorIds,
                        publisherIds,
                        documentTypeIds);

                    var right = BuildFilterBody(
                        and.Right,
                        parameter,
                        categoryIds,
                        authorIds,
                        publisherIds,
                        documentTypeIds);

                    return Expression.AndAlso(left, right);
                }

            case FilterOr or:
                {
                    var left = BuildFilterBody(
                        or.Left,
                        parameter,
                        categoryIds,
                        authorIds,
                        publisherIds,
                        documentTypeIds);

                    var right = BuildFilterBody(
                        or.Right,
                        parameter,
                        categoryIds,
                        authorIds,
                        publisherIds,
                        documentTypeIds);

                    return Expression.OrElse(left, right);
                }

            default:
                throw new ArgumentException("Filter node không được hỗ trợ.");
        }
    }

    private static Expression BuildCondition(
        FilterCondition condition,
        ParameterExpression parameter,
        Dictionary<string, Guid> categoryIds,
        Dictionary<string, Guid> authorIds,
        Dictionary<string, Guid> publisherIds,
        Dictionary<string, Guid> documentTypeIds)
    {
        return condition.Field switch
        {
            "categories" => BuildCategoryCondition(condition.Value, parameter, categoryIds),
            "authors" => BuildAuthorCondition(condition.Value, parameter, authorIds),
            "publishers" => BuildPublisherCondition(condition.Value, parameter, publisherIds),
            "document_types" => BuildDocumentTypeCondition(
                condition.Value,
                parameter,
                documentTypeIds),
            _ => throw new ArgumentException($"Field không được hỗ trợ: {condition.Field}")
        };
    }

    private static Expression BuildCategoryCondition(
        string name,
        ParameterExpression parameter,
        Dictionary<string, Guid> categoryIds)
    {
        if (!categoryIds.TryGetValue(name, out var categoryId))
        {
            return Expression.Constant(false);
        }

        var bookCategories = Expression.Property(parameter, nameof(Book.BookCategories));
        var categoryParameter = Expression.Parameter(typeof(BookCategory), "bc");

        var categoryIdProperty = Expression.Property(
            categoryParameter,
            nameof(BookCategory.CategoryId));

        var equals = Expression.Equal(
            categoryIdProperty,
            Expression.Constant(categoryId));

        var predicate = Expression.Lambda<Func<BookCategory, bool>>(equals, categoryParameter);

        var anyMethod = typeof(Enumerable)
            .GetMethods()
            .Single(method =>
                method.Name == nameof(Enumerable.Any) &&
                method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(BookCategory));

        return Expression.Call(anyMethod, bookCategories, predicate);
    }

    private static Expression BuildAuthorCondition(
        string name,
        ParameterExpression parameter,
        Dictionary<string, Guid> authorIds)
    {
        if (!authorIds.TryGetValue(name, out var authorId))
        {
            return Expression.Constant(false);
        }

        var bookAuthors = Expression.Property(parameter, nameof(Book.BookAuthors));
        var authorParameter = Expression.Parameter(typeof(BookAuthor), "ba");

        var authorIdProperty = Expression.Property(
            authorParameter,
            nameof(BookAuthor.AuthorId));

        var equals = Expression.Equal(
            authorIdProperty,
            Expression.Constant(authorId));

        var predicate = Expression.Lambda<Func<BookAuthor, bool>>(equals, authorParameter);

        var anyMethod = typeof(Enumerable)
            .GetMethods()
            .Single(method =>
                method.Name == nameof(Enumerable.Any) &&
                method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(BookAuthor));

        return Expression.Call(anyMethod, bookAuthors, predicate);
    }

    private static Expression BuildPublisherCondition(
        string name,
        ParameterExpression parameter,
        Dictionary<string, Guid> publisherIds)
    {
        if (!publisherIds.TryGetValue(name, out var publisherId))
        {
            return Expression.Constant(false);
        }

        var publisherIdProperty = Expression.Property(
            parameter,
            nameof(Book.PublisherId));

        var publisherIdValue = Expression.Constant(publisherId, typeof(Guid?));

        return Expression.Equal(publisherIdProperty, publisherIdValue);
    }

    private static Expression BuildDocumentTypeCondition(
        string name,
        ParameterExpression parameter,
        Dictionary<string, Guid> documentTypeIds)
    {
        if (!documentTypeIds.TryGetValue(name, out var documentTypeId))
        {
            return Expression.Constant(false);
        }

        var documentTypeIdProperty = Expression.Property(
            parameter,
            nameof(Book.DocumentTypeId));

        var documentTypeIdValue = Expression.Constant(documentTypeId, typeof(Guid?));

        return Expression.Equal(documentTypeIdProperty, documentTypeIdValue);
    }
}
