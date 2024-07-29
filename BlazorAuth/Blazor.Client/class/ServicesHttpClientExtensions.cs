public static  class ServicesHttpClientExtensions
{
    public static IServiceCollection AddApiHttpClient(
        this IServiceCollection services)
    {
        _ = services.AddTransient<AuthHttpClientHandler>();
        _ = services.AddHttpClient(
                "Api",
                options =>
                {
                    options.BaseAddress =
                        new Uri("http://localhost:5033/api/");
                    options.Timeout = TimeSpan.FromSeconds(10);
                })
            .AddHttpMessageHandler<AuthHttpClientHandler>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        return services;
    }
}