using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExternalTemplates
{
	public class CoreGenerator : ICoreGenerator
	{
		private IGeneratorOptions _options;
		private IFilesProvider _filesProvider;
		private string[] _allGroups;

		public CoreGenerator(IGeneratorOptions options, IFilesProvider filesProvider)
		{
			_options = options;
			_filesProvider = filesProvider;
		}

		public string GenerateScriptTagFromStream(Stream stream, string name)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException(nameof(name));

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

		public FileContext[] GetFilesInGroup(string tempaltesDir, string group)
		{
			if (string.IsNullOrWhiteSpace(group))
			{
				throw new ArgumentException(nameof(group));
			}
			if (string.Equals(group, "~"))
			{
				group = null;
			}
			var dir = tempaltesDir;
			if (group != null)
			{
				dir = Path.Combine(dir, group);
			}
			return _filesProvider
				.EnumerateFilesInDirectory(dir)
				.Where(f => f.Name.EndsWith(_options.Extension))
				.ToArray();
		}

		public string[] NormalizeGroups(string templatesDirectory, string[] groups)
		{
			var first = groups[0];
			if (string.IsNullOrWhiteSpace(first))
			{
				if (_allGroups != null)
				{
					return _allGroups;
				}
				return (_allGroups = GetAllGroups(templatesDirectory).ToArray());
			}
			return groups;
		}

		private IEnumerable<string> GetAllGroups(string templatesDirectory)
		{
			yield return "~";
			foreach (var dir in _filesProvider.EnumerateDirectories(templatesDirectory))
			{
				yield return dir;
			}
		}
	}
}