using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace ExternalTemplates
{
	public class FilesProvider : IFilesProvider
	{
		public IEnumerable<FileContext> EnumerateFilesInDirectory(string directory)
		{
			if (string.IsNullOrWhiteSpace(directory))
				throw new ArgumentException(nameof(directory));

			return Directory.EnumerateFiles(directory)
				.Select(f => new FileContext(f));
		}
	}
}