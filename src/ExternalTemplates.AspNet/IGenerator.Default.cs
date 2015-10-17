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
		private IFilesProvider _filesProvider;
		private ICoreGenerator _coreGenerator;

		public Generator(
			IApplicationEnvironment appEnvironment,
			IGeneratorOptions options,
			IFilesProvider filesProvider,
			ICoreGenerator coreGenerator)
		{
			_appEnvironment = appEnvironment;
			_options = options;
			_filesProvider = filesProvider;
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
				var templateName = file.Name.Remove(file.Name.Length - _options.Extension.Length);
				using (var stream = file.OpenStream())
				{
					sb.Append(
						_coreGenerator.GenerateScriptTagFromStream(
							stream,
							templateName));
				}
			}

			return new HtmlString(sb.ToString());
		}

		private string GetTemplatesDirectory()
		{
			return Path.Combine(
				_appEnvironment.ApplicationBasePath,
				_options.VirtualPath);
		}

		private FileContext[] GetTemplateFiles(string directory)
		{
			return _filesProvider.EnumerateFilesInDirectory(directory)
				.Where(f => f.Name.EndsWith(_options.Extension))
				.ToArray();
		}
	}
}