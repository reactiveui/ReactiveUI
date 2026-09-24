// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace ReactiveUI;

/// <summary>
/// Rewrites and validates expression trees used by ReactiveUI binding infrastructure, normalizing
/// supported constructs into a consistent shape.
/// </summary>
/// <remarks>
/// <para>
/// This rewriter intentionally supports a constrained set of expression node types. Unsupported shapes
/// are rejected with actionable exceptions to help callers correct their expressions.
/// </para>
/// <para>
/// Supported rewrites include:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="ExpressionType.ArrayIndex"/> is rewritten into an indexer access (<c>get_Item</c> / <c>Item</c>).</description></item>
/// <item><description><see cref="ExpressionType.Call"/> to a special-name indexer method (<c>get_Item</c>) is rewritten into an <see cref="IndexExpression"/>.</description></item>
/// <item><description><see cref="ExpressionType.ArrayLength"/> is rewritten into member access to <c>Length</c>.</description></item>
/// <item><description><see cref="ExpressionType.Convert"/> is stripped.</description></item>
/// </list>
/// <para>
/// Index expressions are only supported when all indices are constants.
/// </para>
/// <para>
/// The rewriter does not derive from <see cref="ExpressionVisitor"/>: resolving indexer and length properties
/// reflects over types known only at runtime, and a <see cref="ExpressionVisitor"/> override cannot declare that
/// requirement because the base members do not.
/// </para>
/// </remarks>
public sealed class ExpressionRewriter
{
    /// <summary>The trimming requirement shared by every member that reflects over runtime-only types.</summary>
    private const string RequiresUnreferencedCodeMessage =
        "Expression rewriting uses reflection over runtime types (e.g., Item/Length) which may be removed by trimming.";

    /// <summary>Visits the specified expression node and rewrites supported shapes into their normalized form.</summary>
    /// <param name="node">The expression node to visit.</param>
    /// <returns>The rewritten expression.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="node"/> uses an unsupported node type or shape.</exception>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    public Expression Visit(Expression? node)
    {
        ArgumentExceptionHelper.ThrowIfNull(node);

        return node.NodeType switch
        {
            ExpressionType.ArrayIndex => VisitBinary((BinaryExpression)node),
            ExpressionType.ArrayLength or ExpressionType.Convert => VisitUnary((UnaryExpression)node),
            ExpressionType.Call => VisitMethodCall((MethodCallExpression)node),
            ExpressionType.Index => VisitIndex((IndexExpression)node),
            ExpressionType.MemberAccess => VisitMember((MemberExpression)node),
            ExpressionType.Parameter or ExpressionType.Constant => node,
            _ => throw CreateUnsupportedNodeException(node)
        };
    }

    /// <summary>Creates a consistent exception for unsupported node types, including additional context for binary expressions.</summary>
    /// <param name="node">The unsupported node.</param>
    /// <returns>An exception to throw.</returns>
    private static NotSupportedException CreateUnsupportedNodeException(Expression node)
    {
        const int MessageBuilderInitialCapacity = 96;
        StringBuilder sb = new(MessageBuilderInitialCapacity);
        _ = sb.Append("Unsupported expression of type '")
            .Append(node.NodeType)
            .Append("' ")
            .Append(node)
            .Append('.');

        if (node is BinaryExpression be)
        {
            _ = sb.Append(" Did you meant to use expressions '")
                .Append(be.Left)
                .Append("' and '")
                .Append(be.Right)
                .Append("'?");
        }

        return new(sb.ToString());
    }

    /// <summary>Returns the indexer property (<c>Item</c>) for the specified type.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>The resolved indexer property.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no indexer property can be found.</exception>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    private static PropertyInfo GetItemProperty(Type type)
    {
        var property = type.GetRuntimeProperty("Item");
        return property ?? throw new InvalidOperationException("Could not find a valid indexer property named 'Item'.");
    }

    /// <summary>Returns the <c>Length</c> property for the specified type.</summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>The resolved length property.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no length property can be found.</exception>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    private static PropertyInfo GetLengthProperty(Type type)
    {
        var property = type.GetRuntimeProperty("Length");
        return property
               ?? throw new InvalidOperationException("Could not find valid information for the array length operator.");
    }

    /// <summary>Determines whether all expressions in the provided collection are constant expressions.</summary>
    /// <param name="expressions">The argument list.</param>
    /// <returns><see langword="true"/> if all arguments are constants; otherwise <see langword="false"/>.</returns>
    private static bool AllConstant(ReadOnlyCollection<Expression> expressions)
    {
        for (var i = 0; i < expressions.Count; i++)
        {
            if (expressions[i] is not ConstantExpression)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>Visits a <see cref="BinaryExpression"/> representing an array or indexer access and rewrites it as an appropriate expression tree node.</summary>
    /// <param name="node">The binary expression node to visit. Must represent an array or indexer access with a constant index.</param>
    /// <returns>An <see cref="Expression"/> that represents the rewritten array or indexer access.</returns>
    /// <exception cref="NotSupportedException">Thrown if the right side of the binary expression is not a constant expression.</exception>
    /// <remarks>For array types, it produces an <see cref="Expression.ArrayAccess(Expression, IEnumerable{Expression})"/>; for other
    /// types with indexers, it produces an <see cref="Expression.MakeIndex"/> using the type's indexer property.</remarks>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    private IndexExpression VisitBinary(BinaryExpression node)
    {
        if (node.Right is not ConstantExpression)
        {
            throw new NotSupportedException("Array index expressions are only supported with constants.");
        }

        var instance = Visit(node.Left);
        var index = (ConstantExpression)Visit(node.Right);

        return instance.Type.IsArray
            ? Expression.ArrayAccess(instance, index)
            : Expression.MakeIndex(instance, GetItemProperty(instance.Type), [index]);
    }

    /// <summary>Visits a <see cref="UnaryExpression"/> node, stripping conversions and rewriting array length accesses.</summary>
    /// <param name="node">The unary expression node to visit. Must not be null and must have a valid operand.</param>
    /// <returns>An <see cref="Expression"/> representing the rewritten unary expression.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="node"/> does not have a valid operand.</exception>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    private Expression VisitUnary(UnaryExpression node)
    {
        if (node.Operand is null)
        {
            throw new ArgumentException("Could not find a valid operand for the node.", nameof(node));
        }

        switch (node.NodeType)
        {
            case ExpressionType.Convert:
                return Visit(node.Operand);
            case ExpressionType.ArrayLength:
                {
                    var operand = Visit(node.Operand);
                    var lengthProperty = GetLengthProperty(operand.Type);

                    return Expression.MakeMemberAccess(operand, lengthProperty);
                }

            default:
                return node.Update(Visit(node.Operand));
        }
    }

    /// <summary>
    /// Visits a method call expression representing an indexer access and rewrites it as an index expression if all
    /// arguments are constant.
    /// </summary>
    /// <param name="node">The method call expression to visit. Must represent an indexer access with constant arguments and a non-null
    /// object.</param>
    /// <returns>An expression representing the rewritten indexer access.</returns>
    /// <exception cref="NotSupportedException">Thrown if the method call does not represent an indexer access with all constant arguments.</exception>
    /// <exception cref="ArgumentException">Thrown if the method call does not target a valid object instance.</exception>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    private IndexExpression VisitMethodCall(MethodCallExpression node)
    {
        if (!node.Method.IsSpecialName || !AllConstant(node.Arguments))
        {
            throw new NotSupportedException("Index expressions are only supported with constants.");
        }

        if (node.Object is null)
        {
            throw new ArgumentException("The method call does not point towards an object.", nameof(node));
        }

        var instance = Visit(node.Object);

        var args = VisitArgumentList(node.Arguments);

        return Expression.MakeIndex(instance, GetItemProperty(instance.Type), args);
    }

    /// <summary>Validates that index expressions only use constant arguments, then visits the receiver and arguments.</summary>
    /// <param name="node">The index expression.</param>
    /// <returns>The visited (and potentially rewritten) index expression.</returns>
    /// <exception cref="NotSupportedException">Thrown when any index argument is not a constant.</exception>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    private IndexExpression VisitIndex(IndexExpression node)
    {
        if (!AllConstant(node.Arguments))
        {
            throw new NotSupportedException("Index expressions are only supported with constants.");
        }

        return node.Update(Visit(node.Object), VisitArgumentList(node.Arguments));
    }

    /// <summary>Visits the receiver of a member access.</summary>
    /// <param name="node">The member expression.</param>
    /// <returns>The visited (and potentially rewritten) member expression.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the member is static and so has no receiver.</exception>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MemberExpression VisitMember(MemberExpression node) => node.Update(Visit(node.Expression));

    /// <summary>Visits a method argument list without LINQ allocations.</summary>
    /// <param name="arguments">The argument list to visit.</param>
    /// <returns>A visited argument array suitable for <see cref="Expression.MakeIndex(Expression, PropertyInfo, IEnumerable{Expression})"/>.</returns>
    [RequiresUnreferencedCode(RequiresUnreferencedCodeMessage)]
    private Expression[] VisitArgumentList(ReadOnlyCollection<Expression> arguments)
    {
        var count = arguments.Count;
        if (count == 0)
        {
            return [];
        }

        var visited = new Expression[count];
        for (var i = 0; i < count; i++)
        {
            visited[i] = Visit(arguments[i]);
        }

        return visited;
    }
}
