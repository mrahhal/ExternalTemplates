using System;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;

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
				provider => provider.GetRequiredService<IOptions<GeneratorOptions>>().Options);
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
