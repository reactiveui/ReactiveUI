// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Text;

namespace ReactiveUI;

/// <summary>
/// Rewrites and validates expression trees used by ReactiveUI binding infrastructure, normalizing
/// supported constructs into a consistent shape.
/// </summary>
/// <remarks>
/// <para>
/// This visitor intentionally supports a constrained set of expression node types. Unsupported shapes
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
/// </remarks>
internal sealed class ExpressionRewriter : ExpressionVisitor
{
    /// <summary>
    /// Visits the specified expression node and rewrites supported shapes into their normalized form.
    /// </summary>
    /// <param name="node">The expression node to visit.</param>
    /// <returns>The rewritten expression.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is <see langword="null"/>.</exception>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="node"/> uses an unsupported node type or shape.</exception>
    public override Expression Visit(Expression? node)
    {
        ArgumentExceptionHelper.ThrowIfNull(node);

        return node.NodeType switch
        {
            ExpressionType.ArrayIndex => VisitBinary((BinaryExpression)node),
            ExpressionType.ArrayLength => VisitUnary((UnaryExpression)node),
            ExpressionType.Call => VisitMethodCall((MethodCallExpression)node),
            ExpressionType.Index => VisitIndex((IndexExpression)node),
            ExpressionType.MemberAccess => VisitMember((MemberExpression)node),
            ExpressionType.Parameter => VisitParameter((ParameterExpression)node),
            ExpressionType.Constant => VisitConstant((ConstantExpression)node),
            ExpressionType.Convert => VisitUnary((UnaryExpression)node),
            _ => throw CreateUnsupportedNodeException(node)
        };
    }

    /// <summary>
    /// Visits a <see cref="BinaryExpression"/> representing an array or indexer access and rewrites it as an
    /// appropriate expression tree node.
    /// </summary>
    /// <remarks>This method supports rewriting array index expressions only when the index is a constant. For
    /// array types, it produces an <see cref="Expression.ArrayAccess"/>; for other types with indexers, it produces an
    /// <see cref="Expression.MakeIndex"/> using the type's indexer property. Reflection is used to access runtime type
    /// information, which may have compatibility implications with trimming and AOT compilation.</remarks>
    /// <param name="node">The binary expression node to visit. Must represent an array or indexer access with a constant index.</param>
    /// <returns>An <see cref="Expression"/> that represents the rewritten array or indexer access.</returns>
    /// <exception cref="NotSupportedException">Thrown if the right side of the binary expression is not a constant expression.</exception>
    [RequiresUnreferencedCode("Expression rewriting uses reflection over runtime types (e.g., Item/Length) which may be removed by trimming.")]
    [RequiresDynamicCode("Expression rewriting uses reflection over runtime types and may not be compatible with AOT compilation.")]
    [SuppressMessage("AOT", "IL3051:'RequiresDynamicCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Third Party Code")]
    [SuppressMessage("Trimming", "IL2046:'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Third Party Code")]
    protected override Expression VisitBinary(BinaryExpression node)
    {
        if (node.Right is not ConstantExpression)
        {
            throw new NotSupportedException("Array index expressions are only supported with constants.");
        }

        var instance = Visit(node.Left);
        var index = (ConstantExpression)Visit(node.Right);

        if (instance.Type.IsArray)
        {
            return Expression.ArrayAccess(instance, index);
        }

        // Translate arrayindex into a normal index expression using the indexer property.
        return Expression.MakeIndex(instance, GetItemProperty(instance.Type), [index]);
    }

    /// <summary>
    /// Visits a <see cref="UnaryExpression"/> node and rewrites it as needed for expression tree processing.
    /// </summary>
    /// <remarks>This method may strip conversion nodes or rewrite array length accesses to ensure expression
    /// chains remain stable. Reflection is used to access runtime type information, which may have compatibility
    /// implications with trimming and AOT compilation.</remarks>
    /// <param name="node">The unary expression node to visit. Must not be null and must have a valid operand.</param>
    /// <returns>An <see cref="Expression"/> representing the rewritten unary expression, or the original node if no rewriting is
    /// required.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="node"/> does not have a valid operand.</exception>
    [RequiresUnreferencedCode("Expression rewriting uses reflection over runtime types (e.g., Item/Length) which may be removed by trimming.")]
    [RequiresDynamicCode("Expression rewriting uses reflection over runtime types and may not be compatible with AOT compilation.")]
    [SuppressMessage("AOT", "IL3051:'RequiresDynamicCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Third Party Code")]
    [SuppressMessage("Trimming", "IL2046:'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Third Party Code")]
    protected override Expression VisitUnary(UnaryExpression node)
    {
        if (node.Operand is null)
        {
            throw new ArgumentException("Could not find a valid operand for the node.", nameof(node));
        }

        if (node.NodeType == ExpressionType.Convert)
        {
            // Strip conversion nodes to keep expression chains stable.
            return Visit(node.Operand);
        }

        if (node.NodeType == ExpressionType.ArrayLength)
        {
            var operand = Visit(node.Operand);
            var lengthProperty = GetLengthProperty(operand.Type);

            return Expression.MakeMemberAccess(operand, lengthProperty);
        }

        return node.Update(Visit(node.Operand));
    }

    /// <summary>
    /// Visits a method call expression representing an indexer access and rewrites it as an index expression if all
    /// arguments are constant.
    /// </summary>
    /// <remarks>This method rewrites method calls that correspond to indexer accesses (such as calls to
    /// 'get_Item') into index expressions, provided that all arguments are constant. Reflection is used to determine
    /// the appropriate indexer property, which may have compatibility implications with trimming and AOT
    /// scenarios.</remarks>
    /// <param name="node">The method call expression to visit. Must represent an indexer access with constant arguments and a non-null
    /// object.</param>
    /// <returns>An expression representing the rewritten indexer access.</returns>
    /// <exception cref="NotSupportedException">Thrown if the method call does not represent an indexer access with all constant arguments.</exception>
    /// <exception cref="ArgumentException">Thrown if the method call does not target a valid object instance.</exception>
    [RequiresUnreferencedCode("Expression rewriting uses reflection over runtime types (e.g., Item/Length) which may be removed by trimming.")]
    [RequiresDynamicCode("Expression rewriting uses reflection over runtime types and may not be compatible with AOT compilation.")]
    [SuppressMessage("AOT", "IL3051:'RequiresDynamicCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Third Party Code")]
    [SuppressMessage("Trimming", "IL2046:'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Third Party Code")]
    protected override Expression VisitMethodCall(MethodCallExpression node)
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

        // Visit arguments explicitly to avoid LINQ allocations.
        var args = VisitArgumentList(node.Arguments);

        return Expression.MakeIndex(instance, GetItemProperty(instance.Type), args);
    }

    /// <summary>
    /// Validates that index expressions only use constant arguments, then defers to the base visitor.
    /// </summary>
    /// <param name="node">The index expression.</param>
    /// <returns>The visited (and potentially rewritten) index expression.</returns>
    /// <exception cref="NotSupportedException">Thrown when any index argument is not a constant.</exception>
    protected override Expression VisitIndex(IndexExpression node)
    {
        if (!AllConstant(node.Arguments))
        {
            throw new NotSupportedException("Index expressions are only supported with constants.");
        }

        return base.VisitIndex(node);
    }

    /// <summary>
    /// Creates a consistent exception for unsupported node types, including additional context for binary expressions.
    /// </summary>
    /// <param name="node">The unsupported node.</param>
    /// <returns>An exception to throw.</returns>
    private static Exception CreateUnsupportedNodeException(Expression node)
    {
        // Preserve prior behavior: include helpful guidance for binary expressions.
        var sb = new StringBuilder(96);
        sb.Append("Unsupported expression of type '")
          .Append(node.NodeType)
          .Append("' ")
          .Append(node)
          .Append('.');

        if (node is BinaryExpression be)
        {
            sb.Append(" Did you meant to use expressions '")
              .Append(be.Left)
              .Append("' and '")
              .Append(be.Right)
              .Append("'?");
        }

        return new NotSupportedException(sb.ToString());
    }

    /// <summary>
    /// Returns the indexer property (<c>Item</c>) for the specified type.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>The resolved indexer property.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no indexer property can be found.</exception>
    private static PropertyInfo GetItemProperty(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type type)
    {
        // NOTE: Using the Type instance preserves trimming annotations; do not reconstruct Type from RuntimeTypeHandle.
        var property = type.GetRuntimeProperty("Item");
        return property ?? throw new InvalidOperationException("Could not find a valid indexer property named 'Item'.");
    }

    /// <summary>
    /// Returns the <c>Length</c> property for the specified type.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>The resolved length property.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no length property can be found.</exception>
    private static PropertyInfo GetLengthProperty(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
        Type type)
    {
        // NOTE: Using the Type instance preserves trimming annotations; do not reconstruct Type from RuntimeTypeHandle.
        var property = type.GetRuntimeProperty("Length");
        return property ?? throw new InvalidOperationException("Could not find valid information for the array length operator.");
    }

    /// <summary>
    /// Determines whether all expressions in the provided collection are constant expressions.
    /// </summary>
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

    /// <summary>
    /// Visits a method argument list without LINQ allocations.
    /// </summary>
    /// <param name="arguments">The argument list to visit.</param>
    /// <returns>A visited argument array suitable for <see cref="Expression.MakeIndex(Expression, PropertyInfo, System.Collections.Generic.IEnumerable{Expression})"/>.</returns>
    private Expression[] VisitArgumentList(ReadOnlyCollection<Expression> arguments)
    {
        var count = arguments.Count;
        if (count == 0)
        {
            return Array.Empty<Expression>();
        }

        var visited = new Expression[count];
        for (var i = 0; i < count; i++)
        {
            visited[i] = Visit(arguments[i]);
        }

        return visited;
    }
}
