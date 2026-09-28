using WebsiteQuanLyThuVien.Exceptions;

namespace WebsiteQuanLyThuVien.Repositories.Filters;

/// <summary>
/// Phân tích FilterExpression thành Abstract Syntax Tree (AST).
/// </summary>
public sealed class FilterExpressionParser
{
    private readonly List<FilterToken> _tokens;
    private int _position;

    public FilterExpressionParser(List<FilterToken> tokens)
    {
        _tokens = tokens;
    }

    /// <summary>
    /// Parse toàn bộ FilterExpression thành AST.
    /// </summary>
    public BookFilterNode Parse()
    {
        if (_tokens.Count == 0)
        {
            throw new BusinessException("Filter expression không được rỗng.");
        }

        var result = ParseOr();

        if (_position != _tokens.Count)
        {
            throw new BusinessException("Filter expression không hợp lệ.");
        }

        return result;
    }

    /// <summary>
    /// OR có precedence thấp hơn AND.
    ///
    /// A | B & C
    ///
    /// được hiểu là:
    ///
    /// A | (B & C)
    /// </summary>
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

    /// <summary>
    /// AND có precedence cao hơn OR.
    /// </summary>
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

    /// <summary>
    /// Parse một biểu thức cơ bản.
    ///
    /// Có thể là:
    /// (A | B)
    /// hoặc:
    /// authors:(...)
    /// </summary>
    private BookFilterNode ParsePrimary()
    {
        if (CurrentType() == FilterTokenType.OpenParenthesis)
        {
            Advance();

            var expression = ParseOr();

            Expect(FilterTokenType.CloseParenthesis);

            return expression;
        }

        return ParseFieldExpression();
    }

    /// <summary>
    /// Parse biểu thức của một field.
    ///
    /// Ví dụ:
    ///
    /// authors:("Nguyễn Du" | "Nam Cao")
    /// </summary>
    private BookFilterNode ParseFieldExpression()
    {
        var field = Expect(FilterTokenType.Field);

        Expect(FilterTokenType.Colon);

        Expect(FilterTokenType.OpenParenthesis);

        var expression = ParseFieldValues(field.Value);

        Expect(FilterTokenType.CloseParenthesis);

        return expression;
    }

    /// <summary>
    /// Parse nhiều giá trị của cùng một field.
    /// </summary>
    private BookFilterNode ParseFieldValues(string field)
    {
        var left = ParseFieldValue(field);

        while (
            CurrentType() == FilterTokenType.And ||
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

    /// <summary>
    /// Parse một giá trị đơn.
    /// </summary>
    private BookFilterNode ParseFieldValue(string field)
    {
        var value = Expect(FilterTokenType.Value);

        return new FilterConditionNode(
            new FilterCondition(
                field,
                "=",
                value.Value));
    }

    private FilterToken Expect(FilterTokenType type)
    {
        if (_position >= _tokens.Count)
        {
            throw new BusinessException($"Thiếu token {type} trong filter expression.");
        }

        var token = _tokens[_position];

        if (token.Type != type)
        {
            throw new BusinessException($"Filter expression không hợp lệ. " + $"Đang cần {type} nhưng nhận được {token.Type}.");
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

/// <summary>
/// Node cơ sở của AST.
/// </summary>
public abstract record BookFilterNode;

/// <summary>
/// Node chứa một điều kiện filter đơn.
/// </summary>
public sealed record FilterConditionNode(FilterCondition Condition) : BookFilterNode;

/// <summary>
/// Node AND.
/// </summary>
public sealed record FilterAnd(BookFilterNode Left, BookFilterNode Right) : BookFilterNode;

/// <summary>
/// Node OR.
/// </summary>
public sealed record FilterOr(BookFilterNode Left, BookFilterNode Right) : BookFilterNode;

/// <summary>
/// Loại token trong FilterExpression.
/// </summary>
public enum FilterTokenType
{
    Field,
    Value,
    Colon,
    And,
    Or,
    OpenParenthesis,
    CloseParenthesis
}

/// <summary>
/// Token được tạo ra từ FilterExpression.
/// </summary>
public sealed record FilterToken(FilterTokenType Type, string Value);