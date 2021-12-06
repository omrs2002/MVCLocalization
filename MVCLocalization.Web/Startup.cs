using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using MVCLocalization.Web.Configuration;
using System.Globalization;

namespace MVCLocalization.Web
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddDataAnnotationsLocalization()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,opts => opts.ResourcesPath = "Resources")
                .AddMvcLocalization(options =>{options.ResourcesPath = "Resources";});
                

            services.AddLocalization(options =>{options.ResourcesPath = "Resources";});
            // lists some of the widely used cultures.

            var supportedCulturesStr = new[] { "ar-SA", "en-GB" };

            List<CultureInfo> supportedCultures = new()
            {
                new CultureInfo("ar-SA"),
                new CultureInfo("en-GB")
            };

            // add the request localization middleware in the ConfigureServices method
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(supportedCultures[1]);
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;

                //options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
                //{
                //    await Task.CompletedTask;
                //    // My custom request culture logic
                //    return new ProviderCultureResult(supportedCulturesStr[0]);
                 //}));
                options.FallBackToParentUICultures = true;
            });


            services.Configure<Keys>(Configuration.GetSection("Keys"));

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            if (options != null)
                app.UseRequestLocalization(options.Value);


            app.UseRequestCulture();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }


    }
}
