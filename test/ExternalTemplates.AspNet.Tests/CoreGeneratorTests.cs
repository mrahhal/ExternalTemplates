using System.IO;
using System.Text;
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
			var coreGenerator = new CoreGenerator(options);
			var stream = GetStreamForContent("foo");

			// Act
			var tag = coreGenerator.GenerateScriptTagFromStream(stream, "bar");

			// Assert
			Assert.Equal(
				@"<script type=""text/html"" id=""bar-tmpl"">foo</script>",
				tag);
		}

		private Stream GetStreamForContent(string content)
		{
			var bytes = Encoding.ASCII.GetBytes(content);
			return new MemoryStream(bytes);
		}
	}
}