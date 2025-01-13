using System.Linq.Expressions;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace VStore.Application.CoreHelper;

public static class CoreHelper
{
    public static string GetTokenFromContext(HttpContext context)
    {
        return context.Request.Headers.Authorization.ToString().Split(" ")[1];
    }

    /// <summary>
    /// Combine two expressions with AndAlso operator (&&)
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Expression<Func<T, bool>> CombineAndAlsoExpressions<T>(Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second)
    {
        return CombineExpressions(first, second, Expression.AndAlso);
    }

    /// <summary>
    /// Combine two expressions with OrElse operator (||)
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Expression<Func<T, bool>> CombineOrExpressions<T>(Expression<Func<T, bool>> first,
        Expression<Func<T, bool>> second)
    {
        return CombineExpressions(first, second, Expression.OrElse);
    }

    private static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2, Func<Expression, Expression, BinaryExpression> combiner)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(combiner(left, right), parameter);
    }

    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> source, bool isDescending,
        Expression<Func<T, object>> sortProperty)
    {
        return isDescending ? source.OrderByDescending(sortProperty) : source.OrderBy(sortProperty);
    }

    public static int ApplyPercentage(this int value, int percentage)
    {
        var result = value * (percentage / 100.0);
        var roundedResult = Math.Round(result);
        return Convert.ToInt32(roundedResult);
    }

    public static bool IsValidFileExtension(this string extension)
    {
        return extension switch
        {
            ".jpg" => true,
            ".jpeg" => true,
            ".png" => true,
            ".gif" => true,
            ".webp" => true,
            ".svg" => true,
            _ => false
        };
    }
}

internal class ReplaceExpressionVisitor(Expression oldValue, Expression newValue) : ExpressionVisitor
{
    public override Expression Visit(Expression node)
    {
        if (node == oldValue)
            return newValue;
        return base.Visit(node);
    }
}