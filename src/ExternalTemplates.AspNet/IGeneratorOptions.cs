namespace ExternalTemplates
{
	/// <summary>
	/// Options for <see cref="IGenerator"/>.
	/// </summary>
	public interface IGeneratorOptions
	{
		/// <summary>
		/// Gets the path relative to the web root where the templates are stored.
		/// </summary>
		string VirtualPath { get; set; }

		/// <summary>
		/// Gets the extension of the templates.
		/// </summary>
		string Extension { get; set; }

		/// <summary>
		/// Gets the post string to add to the end of the script tag's id following its name.
		/// </summary>
		string PostString { get; set; }

		/// <summary>
		/// Gets a value that represents how to cache generated content.
		/// </summary>
		CacheKind CacheKind { get; set; }
	}
}