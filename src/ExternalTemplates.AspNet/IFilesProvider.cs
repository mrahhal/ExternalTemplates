using System.Collections.Generic;

namespace ExternalTemplates
{
	public interface IFilesProvider
	{
		IEnumerable<string> EnumerateDirectories(string directory);

		IEnumerable<FileContext> EnumerateFilesInDirectory(string directory);
	}
}