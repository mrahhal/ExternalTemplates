using System.Collections.Generic;
using System.Linq;
using ExternalTemplates.Tests.Fakes;
using Microsoft.AspNet.Hosting;
using Microsoft.Dnx.Runtime;
using Moq;
using Xunit;

namespace ExternalTemplates.Tests
{
	public class GeneratorTests
	{
		[Fact]
		public void Generate()
		{
			// Arrange
			var hostingEnvironment = CreateHostingEnvironmentMock("C:/wwwroot/");
			var options = new GeneratorOptions();
			var filesProvider = CreateFilesProviderMock();
			var coreGenerator = new CoreGenerator(options);
			var generator = new Generator(hostingEnvironment.Object, options, filesProvider.Object, coreGenerator);

			// Act
			var result = generator.Generate();

			// Assert
			Assert.Equal(
				CreateScriptTag("foo-tmpl", "foo") + CreateScriptTag("bar-tmpl", "bar"),
				result.ToString());
		}

		[Fact]
		public void Generate_WithCacheKind_Always()
		{
			// Arrange
			var hostingEnvironment = CreateHostingEnvironmentMock("C:/wwwroot/");
			hostingEnvironment.Setup(h => h.EnvironmentName).Returns(EnvironmentName.Production);
			var options = new GeneratorOptions() { CacheKind = CacheKind.Always };
			var filesProvider = CreateFilesProviderMock();
			var coreGenerator = new CoreGenerator(options);
			var generator = new Generator(hostingEnvironment.Object, options, filesProvider.Object, coreGenerator);

			// Act
			var result = generator.Generate();
			var result2 = generator.Generate();

			// Assert
			Assert.Same(result, result2);
		}

		[Fact]
		public void Generate_WithCacheKind_Never()
		{
			// Arrange
			var hostingEnvironment = CreateHostingEnvironmentMock("C:/wwwroot/");
			hostingEnvironment.Setup(h => h.EnvironmentName).Returns(EnvironmentName.Development);
			var options = new GeneratorOptions() { CacheKind = CacheKind.Never };
			var filesProvider = CreateFilesProviderMock();
			var coreGenerator = new CoreGenerator(options);
			var generator = new Generator(hostingEnvironment.Object, options, filesProvider.Object, coreGenerator);

			// Act
			var result = generator.Generate();
			var result2 = generator.Generate();

			// Assert
			Assert.NotSame(result, result2);
		}

		[Fact]
		public void Generate_WithCacheKind_RemoteOnly_InDevelopment()
		{
			// Arrange
			var hostingEnvironment = CreateHostingEnvironmentMock("C:/wwwroot/");
			hostingEnvironment.Setup(h => h.EnvironmentName).Returns(EnvironmentName.Development);
			var options = new GeneratorOptions() { CacheKind = CacheKind.RemoteOnly };
			var filesProvider = CreateFilesProviderMock();
			var coreGenerator = new CoreGenerator(options);
			var generator = new Generator(hostingEnvironment.Object, options, filesProvider.Object, coreGenerator);

			// Act
			var result = generator.Generate();
			var result2 = generator.Generate();

			// Assert
			Assert.NotSame(result, result2);
		}

		[Fact]
		public void Generate_WithCacheKind_RemoteOnly_InProduction()
		{
			// Arrange
			var hostingEnvironment = CreateHostingEnvironmentMock("C:/wwwroot/");
			hostingEnvironment.Setup(h => h.EnvironmentName).Returns(EnvironmentName.Production);
			var options = new GeneratorOptions() { CacheKind = CacheKind.RemoteOnly };
			var filesProvider = CreateFilesProviderMock();
			var coreGenerator = new CoreGenerator(options);
			var generator = new Generator(hostingEnvironment.Object, options, filesProvider.Object, coreGenerator);

			// Act
			var result = generator.Generate();
			var result2 = generator.Generate();

			// Assert
			Assert.Same(result, result2);
		}

		private Mock<IHostingEnvironment> CreateHostingEnvironmentMock(string basePath)
		{
			var hostingEnvironment = new Mock<IHostingEnvironment>();
			hostingEnvironment.Setup(ae => ae.WebRootPath).Returns(basePath);
			return hostingEnvironment;
		}

		private Mock<IFilesProvider> CreateFilesProviderMock()
		{
			var provider = new Mock<IFilesProvider>();

			provider
				.Setup(p => p.EnumerateFilesInDirectory(It.IsAny<string>()))
				.Returns(() => new List<FileContext>
				{
					new FakeFileContext("C:/wwwroot/Content/templates/foo.tmpl.html", "foo"),
					new FakeFileContext("C:/wwwroot/Content/templates/bar.tmpl.html", "bar"),
					new FakeFileContext("C:/wwwroot/Content/templates/baz.html", "baz"),
				});

			return provider;
		}

		private string CreateScriptTag(string id, string content)
		{
			return $"<script type=\"text/html\" id=\"{id}\">{content}</script>";
		}
	}
}