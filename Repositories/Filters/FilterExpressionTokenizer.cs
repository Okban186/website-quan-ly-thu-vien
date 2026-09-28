using System.Text;
using WebsiteQuanLyThuVien.Exceptions;

namespace WebsiteQuanLyThuVien.Repositories.Filters;

/// <summary>
/// Chuyển chuỗi FilterExpression thành danh sách token. VD authors: ("Nam Cao" | "Kim Lân") , field : authors, colon : , value "Nam Cao" AND value: "Kim Lan"
/// </summary>
public sealed class FilterExpressionTokenizer
{
    /// <summary>
    /// Tokenize FilterExpression.
    /// </summary>
    public List<FilterToken> Tokenize(string expression)
    {
        var tokens = new List<FilterToken>();

        var position = 0;

        while (position < expression.Length)
        {

            ///Bỏ qua khoảng trắng
            while (position < expression.Length && char.IsWhiteSpace(expression[position]))
            {
                position++;
            }

            if (position >= expression.Length)
            {
                break;
            }

            var current = expression[position];

            if (current == '&')
            {
                tokens.Add(new FilterToken(FilterTokenType.And, "&"));

                position++;

                continue;
            }

            if (current == '|')
            {
                tokens.Add(new FilterToken(FilterTokenType.Or, "|"));

                position++;

                continue;
            }

            if (current == '(')
            {
                tokens.Add(new FilterToken(FilterTokenType.OpenParenthesis, "("));

                position++;

                continue;
            }

            if (current == ')')
            {
                tokens.Add(new FilterToken(FilterTokenType.CloseParenthesis, ")"));

                position++;

                continue;
            }

            if (current == '"')
            {
                var value = ReadQuotedValue(expression, ref position);

                tokens.Add(new FilterToken(FilterTokenType.Value, value));

                continue;
            }

            var start = position;

            //Láy field vd authors
            while (
                position < expression.Length && !char.IsWhiteSpace(expression[position]) &&
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
                throw new BusinessException("Filter expression không hợp lệ.");
            }

            var field = expression[start..position]
                .Trim()
                .ToLowerInvariant();

            tokens.Add(new FilterToken(FilterTokenType.Field, field));

            if (position >= expression.Length || expression[position] != ':')
            {
                throw new BusinessException($"Field '{field}' phải có dấu ':'.");
            }

            tokens.Add(new FilterToken(FilterTokenType.Colon, ":"));

            position++;
        }

        return tokens;
    }

    /// <summary>
    /// Đọc giá trị nằm trong dấu ".
    /// </summary>
    private static string ReadQuotedValue(
        string expression,
        ref int position)
    {
        position++;

        var result = new StringBuilder();

        while (position < expression.Length)
        {
            var current = expression[position];

            if (current == '\\')
            {
                if (position + 1 >= expression.Length)
                {
                    throw new BusinessException("Filter expression có escape không hợp lệ.");
                }

                var next = expression[position + 1];

                result.Append(next);

                position += 2;

                continue;
            }

            if (current == '"')
            {
                position++;

                return result.ToString();
            }

            result.Append(current);

            position++;
        }

        throw new BusinessException("Filter expression có chuỗi chưa đóng dấu \".");
    }
}