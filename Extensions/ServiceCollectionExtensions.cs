namespace AIResumeMatcher.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services into the DI container.
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // TODO: Register services here, e.g.:
        // services.AddScoped<IResumeMatcherService, ResumeMatcherService>();

        return services;
    }
}
