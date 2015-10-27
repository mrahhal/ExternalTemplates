using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ExternalTemplates
{
	public static class Generator
	{
		private static IApplicationBasePathProvider _appBasePathProvider;
		private static IGeneratorOptions _options;
		private static IFilesProvider _filesProvider;
		private static ICoreGenerator _coreGenerator;
		private static IHtmlString _cachedContent;
		private static string _templatesDirectory;

		static Generator()
		{
			_appBasePathProvider = Resolve<IApplicationBasePathProvider>();
			_options = Resolve<IGeneratorOptions>();
			_filesProvider = Resolve<IFilesProvider>();
			_coreGenerator = Resolve<ICoreGenerator>();
		}

		public static IHtmlString Generate()
		{
			switch (_options.CacheKind)
			{
				case CacheKind.RemoteOnly:
					if (HttpContext.Current.IsDebuggingEnabled)
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

		private static IHtmlString GenerateFromCache()
		{
			return _cachedContent ?? (_cachedContent = GenerateCore());
		}

		private static IHtmlString GenerateCore()
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

		private static string GetTemplatesDirectory()
		{
			return _templatesDirectory ??
				(_templatesDirectory = Path.Combine(
					_appBasePathProvider.ApplicationBasePath,
					_options.VirtualPath));
		}

		private static T Resolve<T>()
		{
			return DependencyResolver.Current.GetService<T>();
		}
	}
}