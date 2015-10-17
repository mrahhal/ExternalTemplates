using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Dnx.Runtime;

namespace ExternalTemplates
{
	/// <summary>
	/// Default generator.
	/// </summary>
	public class Generator : IGenerator
	{
		private IApplicationEnvironment _appEnvironment;
		private IGeneratorOptions _options;
		private ICoreGenerator _coreGenerator;

		public Generator(
			IApplicationEnvironment appEnvironment,
			IGeneratorOptions options,
			ICoreGenerator coreGenerator)
		{
			_appEnvironment = appEnvironment;
			_options = options;
			_coreGenerator = coreGenerator;
		}

		/// <summary>
		/// Generates the templates.
		/// </summary>
		/// <returns>
		/// The generated combination of script tags from the external templates.
		/// </returns>
		public HtmlString Generate()
		{
			var templatesDirectory = GetTemplatesDirectory();
			var templateFiles = GetTemplateFiles(templatesDirectory);

			var sb = new StringBuilder();
			foreach (var file in templateFiles)
			{
				var fileName = Path.GetFileName(file);
				var templateName = fileName.Remove(fileName.Length - _options.Extension.Length);
				sb.Append(
					_coreGenerator.GenerateScriptTagFromStream(new FileStream(file, FileMode.Open, FileAccess.Read),
					templateName));
			}

			return new HtmlString(sb.ToString());
		}

		private string GetTemplatesDirectory()
		{
			return Path.Combine(
				_appEnvironment.ApplicationBasePath,
				_options.VirtualPath);
		}

		private string[] GetTemplateFiles(string directory)
		{
			return Directory.EnumerateFiles(directory)
				.Where(f => f.EndsWith(_options.Extension))
				.ToArray();
		}
	}
}