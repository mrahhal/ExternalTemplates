using System.Collections.Generic;
using System.IO;
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
			var hostingEnvironment = CreateHostingEnvironmentMock("C:/wwwroot");
			var options = new GeneratorOptions();
			var filesProvider = CreateFilesProviderMock();
			var coreGenerator = new CoreGenerator(options, filesProvider.Object);
			var generator = new Generator(hostingEnvironment.Object, options, coreGenerator);

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
			var hostingEnvironment = CreateHostingEnvironmentMock("C:/wwwroot");
			hostingEnvironment.Setup(h => h.EnvironmentName).Returns(EnvironmentName.Production);
			var options = new GeneratorOptions() { CacheKind = CacheKind.Always };
			var filesProvider = CreateFilesProviderMock();
			var coreGenerator = new Mock<ICoreGenerator>();
			coreGenerator
				.Setup(g => g.NormalizeGroups(It.IsAny<string>(), It.IsAny<string[]>()))
				.Returns<string, string[]>((r1, r2) => r2);
			coreGenerator
				.Setup(g => g.GetFilesInGroup(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(() => new[] { new FakeFileContext("foo.tmpl.html", "bar") });
			var generator = new Generator(hostingEnvironment.Object, options, coreGenerator.Object);

			// Act
			var result = generator.Generate();
			var result2 = generator.Generate();

			// Assert
			coreGenerator
				.Verify(g => g.GenerateScriptTagFromStream(It.IsAny<Stream>(), It.IsAny<string>()), Times.Once());
		}

		[Fact]
		public void Generate_WithCacheKind_Never()
		{
			// Arrange
			var hostingEnvironment = CreateHostingEnvironmentMock("C:/wwwroot");
			hostingEnvironment.Setup(h => h.EnvironmentName).Returns(EnvironmentName.Development);
			var options = new GeneratorOptions() { CacheKind = CacheKind.Never };
			var filesProvider = CreateFilesProviderMock();
			var coreGenerator = new CoreGenerator(options, filesProvider.Object);
			var generator = new Generator(hostingEnvironment.Object, options, coreGenerator);

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
			var hostingEnvironment = CreateHostingEnvironmentMock("C:/wwwroot");
			hostingEnvironment.Setup(h => h.EnvironmentName).Returns(EnvironmentName.Development);
			var options = new GeneratorOptions() { CacheKind = CacheKind.RemoteOnly };
			var filesProvider = CreateFilesProviderMock();
			var coreGenerator = new CoreGenerator(options, filesProvider.Object);
			var generator = new Generator(hostingEnvironment.Object, options, coreGenerator);

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
			var hostingEnvironment = CreateHostingEnvironmentMock("C:/wwwroot");
			hostingEnvironment.Setup(h => h.EnvironmentName).Returns(EnvironmentName.Production);
			var options = new GeneratorOptions() { CacheKind = CacheKind.RemoteOnly };
			var filesProvider = CreateFilesProviderMock();
			var coreGenerator = new Mock<ICoreGenerator>();
			coreGenerator
				.Setup(g => g.NormalizeGroups(It.IsAny<string>(), It.IsAny<string[]>()))
				.Returns<string, string[]>((r1, r2) => r2);
			coreGenerator
				.Setup(g => g.GetFilesInGroup(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(() => new[] { new FakeFileContext("foo.tmpl.html", "bar") });
			var generator = new Generator(hostingEnvironment.Object, options, coreGenerator.Object);

			// Act
			var result = generator.Generate();
			var result2 = generator.Generate();

			// Assert
			coreGenerator
				.Verify(g => g.GenerateScriptTagFromStream(It.IsAny<Stream>(), It.IsAny<string>()), Times.Once());
		}

		[Fact]
		public void Generate_Group()
		{
			// Arrange
			var hostingEnvironment = CreateHostingEnvironmentMock("C:/wwwroot");
			var options = new GeneratorOptions();
			var templatesDirectory = Path.Combine(hostingEnvironment.Object.WebRootPath, options.VirtualPath);
			var groupDirectory = Path.Combine(templatesDirectory, "somegroup");
			var filesProvider = CreateFilesProviderMock();
			filesProvider
				.Setup(p => p.EnumerateFilesInDirectory(templatesDirectory))
				.Returns(new[] { new FakeFileContext(Path.Combine(templatesDirectory, "bar.tmpl.html"), "bar") });
			filesProvider
				.Setup(p => p.EnumerateFilesInDirectory(groupDirectory))
				.Returns(new[] { new FakeFileContext(Path.Combine(groupDirectory, "foo.tmpl.html"), "foo") });
			var coreGenerator = new CoreGenerator(options, filesProvider.Object);
			var generator = new Generator(hostingEnvironment.Object, options, coreGenerator);

			// Act
			var result = generator.Generate("somegroup");

			// Assert
			Assert.Equal(
				CreateScriptTag("foo-tmpl", "foo"),
				result.ToString());
		}

		[Fact]
		public void Generate_All()
		{
			// Arrange
			var hostingEnvironment = CreateHostingEnvironmentMock("C:/wwwroot");
			var options = new GeneratorOptions();
			var templatesDirectory = Path.Combine(hostingEnvironment.Object.WebRootPath, options.VirtualPath);
			var groupDirectory = Path.Combine(templatesDirectory, "somegroup");
			var filesProvider = CreateFilesProviderMock();
			filesProvider
				.Setup(p => p.EnumerateFilesInDirectory(templatesDirectory))
				.Returns(new[] { new FakeFileContext(Path.Combine(templatesDirectory, "bar.tmpl.html"), "bar") });
			filesProvider
				.Setup(p => p.EnumerateFilesInDirectory(groupDirectory))
				.Returns(new[] { new FakeFileContext(Path.Combine(groupDirectory, "foo.tmpl.html"), "foo") });
			filesProvider
				.Setup(p => p.EnumerateDirectories(templatesDirectory))
				.Returns(new[] { "somegroup" });
			var coreGenerator = new CoreGenerator(options, filesProvider.Object);
			var generator = new Generator(hostingEnvironment.Object, options, coreGenerator);

			// Act
			var result = generator.Generate("");

			// Assert
			Assert.Equal(
				CreateScriptTag("bar-tmpl", "bar") + CreateScriptTag("foo-tmpl", "foo"),
				result.ToString());
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