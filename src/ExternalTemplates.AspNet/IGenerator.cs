using Microsoft.AspNet.Mvc.Rendering;

namespace ExternalTemplates
{
	/// <summary>
	/// Finds external templates and generates a combination of script tags from them.
	/// </summary>
	public interface IGenerator
	{
		/// <summary>
		/// Generates the top level templates.
		/// This equivalent to calling <see cref="Generate(string[])"/> with a "~" argument.
		/// </summary>
		/// <returns>
		/// The generated combination of script tags from the external templates.
		/// </returns>
		HtmlString Generate();

		/// <summary>
		/// Generates the templates in the specified groups.
		/// </summary>
		/// <param name="groups">The groups to look in for templates.</param>
		/// <returns>
		/// The generated combination of script tags from the external templates.
		/// </returns>
		HtmlString Generate(params string[] groups);
	}
}