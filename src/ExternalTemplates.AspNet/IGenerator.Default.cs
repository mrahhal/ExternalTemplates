using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Dnx.Runtime;

namespace ExternalTemplates
{
	/// <summary>
	/// Default generator.
	/// </summary>
	public class Generator : IGenerator
	{
		private IHostingEnvironment _hostingEnvironment;
		private IGeneratorOptions _options;
		private IFilesProvider _filesProvider;
		private ICoreGenerator _coreGenerator;
		private HtmlString _cachedContent;
		private static string _templatesDirectory;

		public Generator(
			IHostingEnvironment hostingEnvironment,
			IGeneratorOptions options,
			IFilesProvider filesProvider,
			ICoreGenerator coreGenerator)
		{
			_hostingEnvironment = hostingEnvironment;
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
			switch (_options.CacheKind)
			{
				case CacheKind.RemoteOnly:
					if (_hostingEnvironment.IsDevelopment())
					{
						return GenerateCore();
					}
					else
					{
						return GenerateFromCache();
					}

				case CacheKind.Always:
					return GenerateFromCache();

				case CacheKind.Never:
					return GenerateCore();
				default:
			}
		}

		private HtmlString GenerateFromCache()
		{
			return _cachedContent ?? (_cachedContent = GenerateCore());
		}

		private HtmlString GenerateCore()
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
			return _templatesDirectory ??
				(_templatesDirectory = Path.Combine(
					_hostingEnvironment.WebRootPath,
					_options.VirtualPath));
		}
	}
}