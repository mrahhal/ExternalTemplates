using System;
using System.Collections.Concurrent;
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
		private static ICoreGenerator _coreGenerator;
		private static ConcurrentDictionary<string, IHtmlString> _cachedContent =
			new ConcurrentDictionary<string, IHtmlString>();
		private static string[] _defaultGroups = new string[] { "~" };
		private static string _templatesDirectory;

		static Generator()
		{
			_appBasePathProvider = Resolve<IApplicationBasePathProvider>();
			_options = Resolve<IGeneratorOptions>();
			_coreGenerator = Resolve<ICoreGenerator>();
		}

		public static IHtmlString Generate()
		{
			return Generate(_defaultGroups);
		}

		public static IHtmlString Generate(params string[] groups)
		{
			if (groups == null)
			{
				throw new ArgumentNullException(nameof(groups));
			}
			if (groups.Length == 0)
			{
				groups = _defaultGroups;
			}

			switch (_options.CacheKind)
			{
				case CacheKind.RemoteOnly:
					if (HttpContext.Current.IsDebuggingEnabled)
					{
						return GenerateNew(groups);
					}
					else
					{
						return GenerateFromCache(groups);
					}

				case CacheKind.Always:
					return GenerateFromCache(groups);

				case CacheKind.Never:
				default:
					return GenerateNew(groups);
			}
		}

		private static IHtmlString GenerateFromCache(string[] groups)
		{
			groups = _coreGenerator.NormalizeGroups(GetTemplatesDirectory(), groups);
			var sb = new StringBuilder();
			foreach (var group in groups)
			{
				var content = _cachedContent.GetOrAdd(group, (g) =>
				{
					return GenerateCore(g);
				});
				sb.Append(content.ToString());
			}
			return new HtmlString(sb.ToString());
		}

		private static IHtmlString GenerateNew(string[] groups)
		{
			groups = _coreGenerator.NormalizeGroups(GetTemplatesDirectory(), groups);
			var sb = new StringBuilder();
			foreach (var group in groups)
			{
				sb.Append(GenerateCore(group).ToString());
			}
			return new HtmlString(sb.ToString());
		}

		private static IHtmlString GenerateCore(string group)
		{
			var templatesDirectory = GetTemplatesDirectory();
			var templateFiles = _coreGenerator.GetFilesInGroup(templatesDirectory, group);

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