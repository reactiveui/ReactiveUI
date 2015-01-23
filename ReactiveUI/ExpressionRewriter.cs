using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    internal class ExpressionRewriter : ExpressionVisitor
    {
        public override Expression Visit(Expression node)
        {
            switch (node.NodeType) {
            case ExpressionType.ArrayIndex:
                return this.VisitBinary((BinaryExpression)node);
            case ExpressionType.ArrayLength:
                return this.VisitUnary((UnaryExpression)node);
            case ExpressionType.Call:
                return this.VisitMethodCall((MethodCallExpression)node);
            case ExpressionType.Index:
                return this.VisitIndex((IndexExpression)node);
            case ExpressionType.MemberAccess:
                return this.VisitMember((MemberExpression)node);
            case ExpressionType.Parameter:
                return this.VisitParameter((ParameterExpression)node);
            case ExpressionType.Constant:
                return this.VisitConstant((ConstantExpression)node);
            case ExpressionType.Convert:
                return this.VisitUnary((UnaryExpression)node);
            default:
                throw new NotSupportedException(string.Format("Unsupported expression type: '{0}'", node.NodeType));
            }
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (!(node.Right is ConstantExpression)) {
                throw new NotSupportedException("Array index expressions are only supported with constants.");
            }

            Expression left = this.Visit(node.Left);
            Expression right = this.Visit(node.Right);

            // Translate arrayindex into normal index expression
            return Expression.MakeIndex(left, left.Type.GetRuntimeProperty("Item"), new[] {right });
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.ArrayLength) {
                Expression expression = this.Visit(node.Operand);
                //translate arraylength into normal member expression
                return Expression.MakeMemberAccess(expression, expression.Type.GetRuntimeProperty("Length"));
            } else if (node.NodeType == ExpressionType.Convert) {
                return this.Visit(node.Operand);
            } else {
                return node.Update(this.Visit(node.Operand));
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // Rewrite a method call to an indexer as an index expression
            if (node.Arguments.Any(e => !(e is ConstantExpression)) || !node.Method.IsSpecialName) {
                throw new NotSupportedException("Index expressions are only supported with constants.");
            }

            Expression instance = this.Visit(node.Object);
            IEnumerable<Expression> arguments = this.Visit(node.Arguments);

            // Translate call to get_Item into normal index expression
            return Expression.MakeIndex(instance, instance.Type.GetRuntimeProperty("Item"), arguments);
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            if (node.Arguments.Any(e => !(e is ConstantExpression))) {
                throw new NotSupportedException("Index expressions are only supported with constants.");
            }

            return base.VisitIndex(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            return base.VisitMember(node);
        }
    }
}
