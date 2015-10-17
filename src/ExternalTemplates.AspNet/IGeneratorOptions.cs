namespace ExternalTemplates
{
	/// <summary>
	/// Options for <see cref="IGenerator"/>.
	/// </summary>
	public interface IGeneratorOptions
	{
		/// <summary>
		/// Gets the extension of the templates.
		/// </summary>
		string Extension { get; }

		/// <summary>
		/// Gets the path relative to the web root where the templates are stored.
		/// </summary>
		string VirtualPath { get; }
	}
}