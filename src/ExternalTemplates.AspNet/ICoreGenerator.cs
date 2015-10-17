using System.IO;

namespace ExternalTemplates
{
	public interface ICoreGenerator
	{
		string GenerateScriptTagFromStream(Stream stream, string name);
	}
}