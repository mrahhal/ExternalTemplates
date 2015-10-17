using System.IO;
using System.Text;

namespace ExternalTemplates.Tests.Fakes
{
	public class FakeFileContext : FileContext
	{
		private Stream _stream;

		public FakeFileContext(string path, string content)
			: base(path)
		{
			_stream = new MemoryStream(
				Encoding.ASCII.GetBytes(content));
		}

		public override Stream OpenStream()
		{
			return _stream;
		}
	}
}