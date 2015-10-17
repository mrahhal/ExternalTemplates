using System.Web.Hosting;

namespace ExternalTemplates
{
	public class ApplicationBasePathProvider : IApplicationBasePathProvider
	{
		public string ApplicationBasePath { get { return HostingEnvironment.MapPath("~"); } }
	}
}