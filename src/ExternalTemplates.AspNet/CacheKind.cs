namespace ExternalTemplates
{
	/// <summary>
	/// Represents how to cache generated content.
	/// </summary>
	public enum CacheKind
	{
		/// <summary>
		/// Cache only when in production.
		/// </summary>
		RemoteOnly,

		/// <summary>
		/// Always cache.
		/// </summary>
		Always,

		/// <summary>
		/// Never cache.
		/// </summary>
		Never,
	}
}