using System;
using System.IO;
using System.Text;

namespace ExternalTemplates
{
	public class CoreGenerator : ICoreGenerator
	{
		private IGeneratorOptions _options;

		public CoreGenerator(IGeneratorOptions options)
		{
			_options = options;
		}

		public string GenerateScriptTagFromStream(Stream stream, string name)
		{
			var sb = new StringBuilder();

			sb
				.Append("<script type=\"text/html\" id=\"")
				.Append(name)
				.Append(_options.PostString)
				.Append("\">");

			using (var reader = new StreamReader(stream))
			{
				sb.Append(reader.ReadToEnd());
			}

			sb.Append("</script>");

			return sb.ToString();
		}
	}
}