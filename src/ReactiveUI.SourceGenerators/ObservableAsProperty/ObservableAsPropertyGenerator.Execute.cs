// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ReactiveUI.SourceGenerators.Extensions;
using ReactiveUI.SourceGenerators.Helpers;
using ReactiveUI.SourceGenerators.Input.Models;
using ReactiveUI.SourceGenerators.Models;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static ReactiveUI.SourceGenerators.Diagnostics.DiagnosticDescriptors;

namespace ReactiveUI.SourceGenerators;

/// <summary>
/// ReactiveGenerator.
/// </summary>
/// <seealso cref="Microsoft.CodeAnalysis.IIncrementalGenerator" />
public partial class ObservableAsPropertyGenerator
{
    /// <summary>
    /// A container for all the logic for <see cref="ReactiveCommandGenerator"/>.
    /// </summary>
    internal static class Execute
    {
        /// <summary>
        /// Gets the <see cref="MemberDeclarationSyntax"/> instance for the input field.
        /// </summary>
        /// <param name="propertyInfo">The input <see cref="PropertyInfo"/> instance to process.</param>
        /// <returns>The generated <see cref="MemberDeclarationSyntax"/> instance for <paramref name="propertyInfo"/>.</returns>
        internal static ImmutableArray<MemberDeclarationSyntax> GetPropertySyntax(PropertyInfo propertyInfo)
        {
            // Get the property type syntax
            TypeSyntax propertyType = IdentifierName(propertyInfo.TypeNameWithNullabilityAnnotations);

            string getterFieldIdentifierName;

            // In case the backing field is exactly named "value", we need to add the "this." prefix to ensure that comparisons and assignments
            // with it in the generated setter body are executed correctly and without conflicts with the implicit value parameter.
            if (propertyInfo.FieldName == "value")
            {
                // We only need to add "this." when referencing the field in the setter (getter and XML docs are not ambiguous)
                getterFieldIdentifierName = "value";
            }
            else if (SyntaxFacts.GetKeywordKind(propertyInfo.FieldName) != SyntaxKind.None ||
                     SyntaxFacts.GetContextualKeywordKind(propertyInfo.FieldName) != SyntaxKind.None)
            {
                // If the identifier for the field could potentially be a keyword, we must escape it.
                // This usually happens if the annotated field was escaped as well (eg. "@event").
                // In this case, we must always escape the identifier, in all cases.
                getterFieldIdentifierName = $"@{propertyInfo.FieldName}";
            }
            else
            {
                getterFieldIdentifierName = propertyInfo.FieldName;
            }

            var getterArrowExpression = ArrowExpressionClause(ParseExpression($"{getterFieldIdentifierName} = {getterFieldIdentifierName + "Helper"}.Value"));

            // Construct the generated property as follows:
            //
            // /// <inheritdoc cref="<FIELD_NAME>"/>
            // [global::System.CodeDom.Compiler.GeneratedCode("...", "...")]
            // [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
            // <FORWARDED_ATTRIBUTES>
            // public <FIELD_TYPE><NULLABLE_ANNOTATION?> <PROPERTY_NAME>
            // {
            //     get => <FIELD_NAME>;
            // }
            return
                [FieldDeclaration(VariableDeclaration(ParseTypeName($"ReactiveUI.ObservableAsPropertyHelper<{propertyType}>")))
                .AddDeclarationVariables(VariableDeclarator(getterFieldIdentifierName + "Helper"))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservableAsPropertyGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservableAsPropertyGenerator).Assembly.GetName().Version.ToString()))))))
                    .WithOpenBracketToken(Token(TriviaList(Comment($"/// <inheritdoc cref=\"{propertyInfo.FieldName + "Helper"}\"/>")), SyntaxKind.OpenBracketToken, TriviaList())))
                    .AddModifiers(
                        Token(SyntaxKind.PrivateKeyword),
                        Token(SyntaxKind.ReadOnlyKeyword)),
                PropertyDeclaration(propertyType, Identifier(propertyInfo.PropertyName))
                .AddAttributeLists(
                    AttributeList(SingletonSeparatedList(
                        Attribute(IdentifierName("global::System.CodeDom.Compiler.GeneratedCode"))
                        .AddArgumentListArguments(
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservableAsPropertyGenerator).FullName))),
                            AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(typeof(ObservableAsPropertyGenerator).Assembly.GetName().Version.ToString()))))))
                    .WithOpenBracketToken(Token(TriviaList(Comment($"/// <inheritdoc cref=\"{getterFieldIdentifierName}\"/>")), SyntaxKind.OpenBracketToken, TriviaList())),
                    AttributeList(SingletonSeparatedList(Attribute(IdentifierName("global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage")))))
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                    .WithExpressionBody(getterArrowExpression)
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)))];
        }

        internal static bool GetFieldInfoFromClass(
            FieldDeclarationSyntax fieldSyntax,
            IFieldSymbol fieldSymbol,
            SemanticModel semanticModel,
            CancellationToken token,
            [NotNullWhen(true)] out PropertyInfo? propertyInfo,
            out ImmutableArray<DiagnosticInfo> diagnostics)
        {
            using var builder = ImmutableArrayBuilder<DiagnosticInfo>.Rent();

            // Get the property type and name
            var typeNameWithNullabilityAnnotations = fieldSymbol.Type.GetFullyQualifiedNameWithNullabilityAnnotations();
            var fieldName = fieldSymbol.Name;
            var propertyName = GetGeneratedPropertyName(fieldSymbol);

            // Check for name collisions
            if (fieldName == propertyName)
            {
                builder.Add(
                    ReactivePropertyNameCollisionError,
                    fieldSymbol,
                    fieldSymbol.ContainingType,
                    fieldSymbol.Name);

                propertyInfo = null;
                diagnostics = builder.ToImmutable();

                // If the generated property would collide, skip generating it entirely. This makes sure that
                // users only get the helpful diagnostic about the collision, and not the normal compiler error
                // about a definition for "Property" already existing on the target type, which might be confusing.
                return false;
            }

            token.ThrowIfCancellationRequested();

            // Get the nullability info for the property
            GetNullabilityInfo(
                fieldSymbol,
                semanticModel,
                out var isReferenceTypeOrUnconstraindTypeParameter,
                out var includeMemberNotNullOnSetAccessor);

            token.ThrowIfCancellationRequested();

            propertyInfo = new PropertyInfo(
                typeNameWithNullabilityAnnotations,
                fieldName,
                propertyName,
                isReferenceTypeOrUnconstraindTypeParameter,
                includeMemberNotNullOnSetAccessor);

            diagnostics = builder.ToImmutable();

            return true;
        }

        /// <summary>
        /// Gets the nullability info on the generated property.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <param name="semanticModel">The <see cref="SemanticModel"/> instance for the current run.</param>
        /// <param name="isReferenceTypeOrUnconstraindTypeParameter">Whether the property type supports nullability.</param>
        /// <param name="includeMemberNotNullOnSetAccessor">Whether <see cref="MemberNotNullAttribute"/> should be used on the setter.</param>
        private static void GetNullabilityInfo(
            IFieldSymbol fieldSymbol,
            SemanticModel semanticModel,
            out bool isReferenceTypeOrUnconstraindTypeParameter,
            out bool includeMemberNotNullOnSetAccessor)
        {
            // We're using IsValueType here and not IsReferenceType to also cover unconstrained type parameter cases.
            // This will cover both reference types as well T when the constraints are not struct or unmanaged.
            // If this is true, it means the field storage can potentially be in a null state (even if not annotated).
            isReferenceTypeOrUnconstraindTypeParameter = !fieldSymbol.Type.IsValueType;

            // This is used to avoid nullability warnings when setting the property from a constructor, in case the field
            // was marked as not nullable. Nullability annotations are assumed to always be enabled to make the logic simpler.
            // Consider this example:
            //
            // partial class MyViewModel : ReactiveObject
            // {
            //    public MyViewModel()
            //    {
            //        Name = "Bob";
            //    }
            //
            //    [ObservableAsProperty]
            //    private string name;
            // }
            //
            // The [MemberNotNull] attribute is needed on the setter for the generated Name property so that when Name
            // is set, the compiler can determine that the name backing field is also being set (to a non null value).
            // Of course, this can only be the case if the field type is also of a type that could be in a null state.
            includeMemberNotNullOnSetAccessor =
                isReferenceTypeOrUnconstraindTypeParameter &&
                fieldSymbol.Type.NullableAnnotation != NullableAnnotation.Annotated &&
                semanticModel.Compilation.HasAccessibleTypeWithMetadataName("System.Diagnostics.CodeAnalysis.MemberNotNullAttribute");
        }

        /// <summary>
        /// Get the generated property name for an input field.
        /// </summary>
        /// <param name="fieldSymbol">The input <see cref="IFieldSymbol"/> instance to process.</param>
        /// <returns>The generated property name for <paramref name="fieldSymbol"/>.</returns>
        private static string GetGeneratedPropertyName(IFieldSymbol fieldSymbol)
        {
            var propertyName = fieldSymbol.Name;

            if (propertyName.StartsWith("m_"))
            {
                propertyName = propertyName.Substring(2);
            }
            else if (propertyName.StartsWith("_"))
            {
                propertyName = propertyName.TrimStart('_');
            }

            return $"{char.ToUpper(propertyName[0], CultureInfo.InvariantCulture)}{propertyName.Substring(1)}";
        }
    }
}
