using System.Collections.Generic;
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