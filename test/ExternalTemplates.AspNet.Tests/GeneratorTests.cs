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
			var appEnvironment = CreateAppEnvironmentMock("C:/wwwroot/");
			var options = new GeneratorOptions();
			var filesProvider = CreateFilesProviderMock();
			var coreGenerator = new CoreGenerator(options);
			var generator = new Generator(appEnvironment.Object, options, filesProvider.Object, coreGenerator);

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
			var appEnvironment = CreateAppEnvironmentMock("C:/wwwroot/");
			var options = new GeneratorOptions() { CacheKind = CacheKind.Always };
			var filesProvider = CreateFilesProviderMock();
			var coreGenerator = new CoreGenerator(options);
			var generator = new Generator(appEnvironment.Object, options, filesProvider.Object, coreGenerator);

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
			var appEnvironment = CreateAppEnvironmentMock("C:/wwwroot/");
			var options = new GeneratorOptions() { CacheKind = CacheKind.Never };
			var filesProvider = CreateFilesProviderMock();
			var coreGenerator = new CoreGenerator(options);
			var generator = new Generator(appEnvironment.Object, options, filesProvider.Object, coreGenerator);

			// Act
			var result = generator.Generate();
			var result2 = generator.Generate();

			// Assert
			Assert.NotSame(result, result2);
		}

		private Mock<IHostingEnvironment> CreateAppEnvironmentMock(string basePath)
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