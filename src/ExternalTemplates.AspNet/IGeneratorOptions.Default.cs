namespace ExternalTemplates
{
	/// <summary>
	/// Default generator options.
	/// </summary>
	public class GeneratorOptions : IGeneratorOptions
	{
		/// <summary>
		/// Gets the extension of the templates.
		/// Default is ".tmpl.html".
		/// </summary>
		public string Extension { get; set; }

		/// <summary>
		/// Gets the path relative to the web root where the templates are stored.
		/// Default is "/Content/templates".
		/// </summary>
		public string VirtualPath { get; set; }
	}
}