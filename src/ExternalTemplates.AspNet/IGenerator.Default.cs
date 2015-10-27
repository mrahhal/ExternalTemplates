using System;
using System.Collections.Concurrent;
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
		private ICoreGenerator _coreGenerator;
		private ConcurrentDictionary<string, HtmlString> _cachedContent =
			new ConcurrentDictionary<string, HtmlString>();
		private string[] _defaultGroups = new string[] { "~" };
		private string _templatesDirectory;

		public Generator(
			IHostingEnvironment hostingEnvironment,
			IGeneratorOptions options,
			ICoreGenerator coreGenerator)
		{
			_hostingEnvironment = hostingEnvironment;
			_options = options;
			_coreGenerator = coreGenerator;
		}


		/// <summary>
		/// Generates the top level templates.
		/// This equivalent to calling <see cref="Generate(string[])"/> with a "~" argument.
		/// </summary>
		/// <returns>
		/// The generated combination of script tags from the external templates.
		/// </returns>
		public HtmlString Generate()
		{
			return Generate(_defaultGroups);
		}

		/// <summary>
		/// Generates the templates in the specified groups.
		/// </summary>
		/// <param name="groups">The groups to look in for templates.</param>
		/// <returns>
		/// The generated combination of script tags from the external templates.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">groups is null.</exception>
		public HtmlString Generate(params string[] groups)
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
					if (_hostingEnvironment.IsDevelopment())
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

		private HtmlString GenerateFromCache(string[] groups)
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

		private HtmlString GenerateNew(string[] groups)
		{
			groups = _coreGenerator.NormalizeGroups(GetTemplatesDirectory(), groups);
			var sb = new StringBuilder();
			foreach (var group in groups)
			{
				sb.Append(GenerateCore(group).ToString());
			}
			return new HtmlString(sb.ToString());
		}

		private HtmlString GenerateCore(string group)
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

		private string GetTemplatesDirectory()
		{
			return _templatesDirectory ??
				(_templatesDirectory = Path.Combine(
					_hostingEnvironment.WebRootPath,
					_options.VirtualPath));
		}
	}
}