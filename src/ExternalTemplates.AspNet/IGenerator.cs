using Microsoft.AspNet.Mvc.Rendering;

namespace ExternalTemplates
{
	/// <summary>
	/// Finds external templates and generates a combination of script tags from them.
	/// </summary>
	public interface IGenerator
	{
		/// <summary>
		/// Generates the templates.
		/// </summary>
		/// <returns>
		/// The generated combination of script tags from the external templates.
		/// </returns>
		HtmlString Generate();
	}
}