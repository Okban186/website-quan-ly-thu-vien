using System.Linq.Expressions;
using WebsiteQuanLyThuVien.Exceptions;
using WebsiteQuanLyThuVien.Models;

namespace WebsiteQuanLyThuVien.Repositories.Filters;

/// <summary>
/// Service chịu trách nhiệm chuyển FilterExpression thành
/// Expression<Func&lt;Book, bool&gt;&gt;.
///
/// Service này điều phối tokenizer, parser và filter handlers.
/// </summary>
public sealed class ResourceFilterExpressionService
{
    private readonly FilterExpressionTokenizer _tokenizer;
    private readonly FilterHandlerRegistry _registry;

    public ResourceFilterExpressionService(FilterExpressionTokenizer tokenizer, FilterHandlerRegistry registry)
    {
        _tokenizer = tokenizer;
        _registry = registry;
    }

    /// <summary>
    /// Chuyển FilterExpression thành Expression có thể sử dụng
    /// trong LINQ Where().
    /// </summary>
    public async Task<Expression<Func<Resource, bool>>> BuildAsync(string expression, CancellationToken cancellationToken = default)
    {
        var tokens = _tokenizer.Tokenize(expression);

        if (tokens.Count == 0)
        {
            throw new BusinessException("Filter expression không được rỗng.");
        }

        var parser = new FilterExpressionParser(tokens);

        var ast = parser.Parse();

        var parameter = Expression.Parameter(typeof(Resource), "resource");

        var body = await BuildNodeAsync(ast, parameter, cancellationToken);

        return Expression.Lambda<Func<Resource, bool>>(body, parameter);
    }

    /// <summary>
    /// Chuyển từng node trong AST thành Expression.
    /// </summary>
    private async Task<Expression> BuildNodeAsync(ResourceFilterNode node, ParameterExpression parameter, CancellationToken cancellationToken)
    {
        switch (node)
        {
            case FilterConditionNode condition:

                return await BuildConditionAsync(
                    condition.Condition,
                    parameter,
                    cancellationToken);

            case FilterAnd and:

                var leftAnd = await BuildNodeAsync(
                    and.Left,
                    parameter,
                    cancellationToken);

                var rightAnd = await BuildNodeAsync(
                    and.Right,
                    parameter,
                    cancellationToken);

                return Expression.AndAlso(
                    leftAnd,
                    rightAnd);

            case FilterOr or:

                var leftOr = await BuildNodeAsync(
                    or.Left,
                    parameter,
                    cancellationToken);

                var rightOr = await BuildNodeAsync(
                    or.Right,
                    parameter,
                    cancellationToken);

                return Expression.OrElse(
                    leftOr,
                    rightOr);

            default:

                throw new BusinessException("Filter node không được hỗ trợ.");
        }
    }

    /// <summary>
    /// Tìm handler phù hợp với field và yêu cầu handler
    /// xây dựng Expression tương ứng.
    /// </summary>
    private async Task<Expression> BuildConditionAsync(FilterCondition condition, ParameterExpression parameter, CancellationToken cancellationToken)
    {
        var handler = _registry.GetHandler(
            condition.Field);

        return await handler.BuildExpressionAsync(condition, parameter, cancellationToken);
    }
}