﻿using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CommunityToolkit.Maui.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UseCommunityToolkitInitializationAnalyzer : DiagnosticAnalyzer
{
	public const string DiagnosticId = "MCT001";

	const string category = "Initialization";

	static readonly LocalizableString title = new LocalizableResourceString(nameof(Resources.InitializationErrorTitle), Resources.ResourceManager, typeof(Resources));
	static readonly LocalizableString messageFormat = new LocalizableResourceString(nameof(Resources.InitalizationMessageFormat), Resources.ResourceManager, typeof(Resources));
	static readonly LocalizableString description = new LocalizableResourceString(nameof(Resources.InitializationErrorMessage), Resources.ResourceManager, typeof(Resources));

	static readonly DiagnosticDescriptor rule = new(DiagnosticId, title, messageFormat, category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: description);

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = [rule];

	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();
		context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.GenericName);
	}

	static void AnalyzeNode(SyntaxNodeAnalysisContext context)
	{
		GenericNameSyntax genericNameSyntax = (GenericNameSyntax)context.Node;

		if (genericNameSyntax.Identifier.Text == "UseMauiApp")
		{
			if (genericNameSyntax.Parent is SyntaxNode parentSyntaxNode) 
			{
				foreach (SyntaxNode child in parentSyntaxNode.ChildNodes())
				{
					if (child is ExpressionSyntax expressionSyntax) 
					{

					}
				}
			}
		}

		//var expressionStatement = (ExpressionStatementSyntax)context.Node;
		//var root = expressionStatement.SyntaxTree.GetRoot();

		//var methodDeclarationWithoutWhiteSpace = GetAllMethodDelcarationsWithoutWhiteSpace(root);

		//if (methodDeclarationWithoutWhiteSpace.Contains(".UseMauiApp<") && !methodDeclarationWithoutWhiteSpace.Contains(".UseMauiCommunityToolkit("))
		//{
		//	var expression = GetInvocationExpressionSyntax(expressionStatement);
		//	var diagnostic = Diagnostic.Create(rule, expression.GetLocation());
		//	context.ReportDiagnostic(diagnostic);
		//}
	}

	static string GetAllMethodDelcarationsWithoutWhiteSpace(SyntaxNode root)
	{
		var stringBuilder = new StringBuilder();
		foreach (var methodDeclaration in root.DescendantNodes().OfType<MethodDeclarationSyntax>())
		{
			stringBuilder.Append(methodDeclaration.ToString().Where(static c => !char.IsWhiteSpace(c)).ToArray());
		}

		return stringBuilder.ToString();
	}

	static InvocationExpressionSyntax GetInvocationExpressionSyntax(SyntaxNode parent)
	{
		foreach (var child in parent.ChildNodes())
		{
			if (child is InvocationExpressionSyntax expressionSyntax)
			{
				return expressionSyntax;
			}
			else
			{
				var expression = GetInvocationExpressionSyntax(child);

				if (expression is not null)
				{
					return expression;
				}
			}
		}
		throw new InvalidOperationException("Wow, this shouldn't happen, please open a bug here: https://github.com/CommunityToolkit/Maui/issues/new/choose");
	}
}