using System;
using System.IO;

namespace ExternalTemplates
{
	public interface ICoreGenerator
	{
		string GenerateScriptTagFromStream(Stream stream, string name);

		FileContext[] GetFilesInGroup(string tempaltesDir, string group);

		string[] NormalizeGroups(string templatesDirectory, string[] groups);
	}
}