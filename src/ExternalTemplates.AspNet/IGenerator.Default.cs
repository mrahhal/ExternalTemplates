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
#if DEBUG
					return GenerateCore();
#else
					// Fall to CacheKind.Always when in RELEASE
#endif
				case CacheKind.Always:
					return
						_cachedContent ??
						(_cachedContent = GenerateCore());

				case CacheKind.Never:
					return GenerateCore();
			}
			throw new InvalidOperationException();
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
			return Path.Combine(
				_hostingEnvironment.WebRootPath,
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