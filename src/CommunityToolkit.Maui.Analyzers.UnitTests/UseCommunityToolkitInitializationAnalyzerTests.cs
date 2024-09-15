using CommunityToolkit.Maui.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace CommunityToolkit.Maui.Analyzers.UnitTests;

public class UseCommunityToolkitInitializationAnalyzerTests
{
	[Fact]
	public void UseCommunityToolkitInitializationAnalyzerId()
	{
		Assert.Equal("MCT001", UseCommunityToolkitInitializationAnalyzer.DiagnosticId);
	}

	[Fact]
	public async Task VerifyWarningIsEmittedAsync()
	{
		CSharpCodeFixTest<UseCommunityToolkitInitializationAnalyzer, UseCommunityToolkitInitializationAnalyzerCodeFixProvider, DefaultVerifier> context = new()
		{
			ReferenceAssemblies = ReferenceAssemblies.Net.Net80
		};

		Type[] types = [
			typeof(MauiApp),
			typeof(MauiAppBuilder),
			typeof(Application),
			typeof(Options),
		];

		foreach (Type type in types)
		{
			context.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(type.Assembly.Location));
		}

		context.TestCode = /*lang=C#-test */"""

			namespace CommunityToolkit.Maui.Sample 
			{
				using Microsoft.Maui;
				using Microsoft.Maui.Hosting;
				using Microsoft.Maui.Controls.Hosting;
				using CommunityToolkit.Maui;
			
				public static class MauiProgram
				{
					public static MauiApp CreateMauiApp()
					{
						var builder = MauiApp.CreateBuilder()
							.UseMauiApp<Microsoft.Maui.Controls.Application>()
							.UseMauiCommunityToolkit();

						return builder.Build();
					}
				}
			}

			namespace CommunityToolkit.Maui
			{
				using System;
				using Microsoft.Maui;
				using Microsoft.Maui.Hosting;
				using CommunityToolkit.Maui.Core;
			
				public static class AppBuilderExtensions
				{
					public static MauiAppBuilder UseMauiCommunityToolkit(this MauiAppBuilder builder, Action<Options>? options = default)
					{
						throw new NotImplementedException("We need the method to exist. It does not have to work.");
					}
				}
			}

			namespace Microsoft.Maui.Hosting
			{
				using System;
				using Microsoft.Maui;
				using Microsoft.Maui.Hosting;
				using System.Diagnostics.CodeAnalysis;
			
				public static class AppBuilderExtensions
				{
					public static MauiAppBuilder UseMauiApp<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApp>(this MauiAppBuilder builder)
						where TApp : class, IApplication
					{
						throw new NotImplementedException("We need the method to exist. It does not have to work.");
					}
				}
			}
			
			""";

		context.FixedCode = /*lang=C#-test */"""
			class TYPE1 { }
			class TYPE2 { }
			""";

		await context.RunAsync();
	}
}