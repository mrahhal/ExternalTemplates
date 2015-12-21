using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;

namespace ExternalTemplates
{
	public static class ServiceExtensions
	{
		/// <summary>
		/// Adds ExternalTemplates services.
		/// </summary>
		public static IServiceCollection AddExternalTemplates(this IServiceCollection services)
		{
			services.AddSingleton<IGeneratorOptions, GeneratorOptions>(
				provider => provider.GetRequiredService<IOptions<GeneratorOptions>>().Value);
			services.AddSingleton<IFilesProvider, FilesProvider>();
			services.AddSingleton<ICoreGenerator, CoreGenerator>();
			services.AddSingleton<IGenerator, Generator>();

			return services;
		}

		/// <summary>
		/// Configures ExternalTemplates.
		/// </summary>
		public static IServiceCollection ConfigureExternalTemplates(
			this IServiceCollection services,
			Action<GeneratorOptions> configure)
		{
			return services.Configure(configure);
		}
	}
}
