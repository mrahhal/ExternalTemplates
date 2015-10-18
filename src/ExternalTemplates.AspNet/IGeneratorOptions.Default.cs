using System;

namespace ExternalTemplates
{
	/// <summary>
	/// Default generator options.
	/// </summary>
	public class GeneratorOptions : IGeneratorOptions
	{
		/// <summary>
		/// Gets the path relative to the web root where the templates are stored.
		/// Default is "Content/templates".
		/// </summary>
		public string VirtualPath { get; set; } = "Content/templates";

		/// <summary>
		/// Gets the extension of the templates.
		/// Default is ".tmpl.html".
		/// </summary>
		public string Extension { get; set; } = ".tmpl.html";

		/// <summary>
		/// Gets the post string to add to the end of the script tag's id following its name.
		/// Default is "-tmpl".
		/// </summary>
		public string PostString { get; set; } = "-tmpl";
	}
}