using GraphApiBasics.Model;

namespace GraphApiBasics;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        // Accessing configuration values
        var key1 = Configuration["GraphClientConfig:Authority"];
        var key2 = Configuration["GraphClientConfig:ClientId"];

        // Registering configuration section as a strongly typed class
        services.Configure<GraphSecretOptions>(Configuration.GetSection("GraphClientConfig"));

        // Add other services
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
