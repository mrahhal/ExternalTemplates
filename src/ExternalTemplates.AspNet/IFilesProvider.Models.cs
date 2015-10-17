using System;
using System.IO;

namespace ExternalTemplates
{
	public class FileContext
	{
		private string _path;

		public FileContext(string path)
		{
			if (string.IsNullOrWhiteSpace(path))
				throw new ArgumentException(nameof(path));

			_path = path;
			Name = Path.GetFileName(_path);
		}

		public virtual string Name { get; private set; }

		public virtual Stream OpenStream()
		{
			return new FileStream(_path, FileMode.Open, FileAccess.Read);
		}
	}
}