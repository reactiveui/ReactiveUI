﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    /// <summary>
    /// Class for simplifying and validating expressions.
    /// </summary>
    internal class ExpressionRewriter : ExpressionVisitor
    {
        public override Expression Visit(Expression? node)
        {
            if (node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            switch (node.NodeType)
            {
                case ExpressionType.ArrayIndex:
                    return VisitBinary((BinaryExpression)node);
                case ExpressionType.ArrayLength:
                    return VisitUnary((UnaryExpression)node);
                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)node);
                case ExpressionType.Index:
                    return VisitIndex((IndexExpression)node);
                case ExpressionType.MemberAccess:
                    return VisitMember((MemberExpression)node);
                case ExpressionType.Parameter:
                    return VisitParameter((ParameterExpression)node);
                case ExpressionType.Constant:
                    return VisitConstant((ConstantExpression)node);
                case ExpressionType.Convert:
                    return VisitUnary((UnaryExpression)node);
                default:
                    var errorMessageBuilder = new StringBuilder($"Unsupported expression of type '{node.NodeType}' {node}.");

                    if (node is BinaryExpression binaryExpression)
                    {
                        errorMessageBuilder.Append(" Did you meant to use expressions '")
                            .Append(binaryExpression.Left)
                            .Append("' and '")
                            .Append(binaryExpression.Right)
                            .Append("'?");
                    }

                    throw new NotSupportedException(errorMessageBuilder.ToString());
            }
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (!(node.Right is ConstantExpression))
            {
                throw new NotSupportedException("Array index expressions are only supported with constants.");
            }

            Expression left = Visit(node.Left);
            Expression right = Visit(node.Right);

            // Translate arrayindex into normal index expression
            return Expression.MakeIndex(left, left.Type.GetRuntimeProperty("Item"), new[] { right });
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.ArrayLength && node.Operand is not null)
            {
                Expression expression = Visit(node.Operand);

                var memberInfo = expression.Type.GetRuntimeProperty("Length");

                if (memberInfo is null)
                {
                    throw new InvalidOperationException("Could not find valid information for the array length operator.");
                }

                // translate arraylength into normal member expression
                return Expression.MakeMemberAccess(expression, memberInfo);
            }
            else if (node.NodeType == ExpressionType.Convert && node.Operand is not null)
            {
                return Visit(node.Operand);
            }
            else if (node.Operand is not null)
            {
                return node.Update(Visit(node?.Operand));
            }
            else
            {
                throw new ArgumentException("Could not find a valid operand for the node.", nameof(node));
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // Rewrite a method call to an indexer as an index expression
            if (node.Arguments.Any(e => !(e is ConstantExpression)) || !node.Method.IsSpecialName)
            {
                throw new NotSupportedException("Index expressions are only supported with constants.");
            }

            if (node.Object is null)
            {
                throw new ArgumentException("The Method call does not point towards an object.", nameof(node));
            }

            Expression instance = Visit(node.Object);
            IEnumerable<Expression> arguments = Visit(node.Arguments);

            // Translate call to get_Item into normal index expression
            return Expression.MakeIndex(instance, instance.Type.GetRuntimeProperty("Item"), arguments);
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            if (node.Arguments.Any(e => !(e is ConstantExpression)))
            {
                throw new NotSupportedException("Index expressions are only supported with constants.");
            }

            return base.VisitIndex(node);
        }
    }
}
