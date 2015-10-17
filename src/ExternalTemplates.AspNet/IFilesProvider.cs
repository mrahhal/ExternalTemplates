using System.Collections.Generic;

namespace ExternalTemplates
{
	public interface IFilesProvider
	{
		IEnumerable<FileContext> EnumerateFilesInDirectory(string directory);
	}
}