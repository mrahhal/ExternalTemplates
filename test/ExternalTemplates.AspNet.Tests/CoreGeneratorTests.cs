using System.IO;
using System.Text;
using ExternalTemplates.Tests.Fakes;
using Moq;
using Xunit;

namespace ExternalTemplates.Tests
{
	public class CoreGeneratorTests
	{
		[Fact]
		public void GenerateScriptTagFromStream()
		{
			// Arrange
			var options = new GeneratorOptions();
			var coreGenerator = new CoreGenerator(options, new Mock<IFilesProvider>().Object);
			var stream = GetStreamForContent("foo");

			// Act
			var tag = coreGenerator.GenerateScriptTagFromStream(stream, "bar");

			// Assert
			Assert.Equal(
				@"<script type=""text/html"" id=""bar-tmpl"">foo</script>",
				tag);
		}

		[Fact]
		public void GetFilesInGroup_InRoot()
		{
			// Arrange
			var templatesDir = "C:/wwwroot/templates";
			var options = new GeneratorOptions();
			var filesProvider = new Mock<IFilesProvider>();
			filesProvider
				.Setup(p => p.EnumerateFilesInDirectory(templatesDir))
				.Returns(new[] { new FakeFileContext("foo.tmpl.html", "foo") });
			var coreGenerator = new CoreGenerator(options, filesProvider.Object);

			// Act
			var files = coreGenerator.GetFilesInGroup(templatesDir, "~");

			// Arrange
			Assert.True(files[0].Name == "foo.tmpl.html");
		}

		[Fact]
		public void GetFilesInGroup_InGroup()
		{
			// Arrange
			var templatesDir = "C:/wwwroot/templates";
			var group = Path.Combine(templatesDir, "somegroup");
			var options = new GeneratorOptions();
			var filesProvider = new Mock<IFilesProvider>();
			filesProvider
				.Setup(p => p.EnumerateFilesInDirectory(group))
				.Returns(new[] { new FakeFileContext("foo.tmpl.html", "foo") });
			var coreGenerator = new CoreGenerator(options, filesProvider.Object);

			// Act
			var files = coreGenerator.GetFilesInGroup(templatesDir, "somegroup");

			// Arrange
			Assert.True(files[0].Name == "foo.tmpl.html");
		}

		[Theory]
		[InlineData("~")]
		[InlineData("somegroup")]
		[InlineData("~ somegroup")]
		public void NormalizeGroups_WithNormalGroups(string concatGroups)
		{
			// Arrange
			var groups = concatGroups.Split(' ');
			var templatesDir = "C:/wwwroot/templates";
			var options = new GeneratorOptions();
			var filesProvider = new Mock<IFilesProvider>();
			var coreGenerator = new CoreGenerator(options, filesProvider.Object);

			// Act
			var result = coreGenerator.NormalizeGroups(templatesDir, groups);

			// Arrange
			Assert.Same(groups, result);
		}

		[Fact]
		public void NormalizeGroups_WithEmptyGroup()
		{
			// Arrange
			var templatesDir = "C:/wwwroot/templates";
			var group = Path.Combine(templatesDir, "somegroup");
			var options = new GeneratorOptions();
			var filesProvider = new Mock<IFilesProvider>();
			filesProvider
				.Setup(p => p.EnumerateDirectories(templatesDir))
				.Returns(new[] { "somegroup" });
			var coreGenerator = new CoreGenerator(options, filesProvider.Object);

			// Act
			var result = coreGenerator.NormalizeGroups(templatesDir, new[] { "" });

			// Arrange
			Assert.True(result.Length == 2);
			Assert.True(result[0] == "~");
			Assert.True(result[1] == "somegroup");
		}

		private Stream GetStreamForContent(string content)
		{
			var bytes = Encoding.ASCII.GetBytes(content);
			return new MemoryStream(bytes);
		}
	}
}