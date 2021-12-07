using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using MVCLocalization.Web.Configuration;
using MVCLocalization.Web.Middlewares;
using MVCLocalization.Web.Services;
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
            services.AddHttpLogging(logging =>
            {
                //Write your code to configure the HttpLogging middleware here
                logging.LoggingFields = HttpLoggingFields.All;
                logging.RequestHeaders.Add("My-Request-Header");
                logging.ResponseHeaders.Add("My-Response-Header");
                logging.MediaTypeOptions.AddText("application/javascript");

                //Maximum request body size to log, in bytes. Defaults to 32 KB.
                logging.RequestBodyLogLimit = 4096;
                logging.ResponseBodyLogLimit = 4096;


            });

            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddDataAnnotationsLocalization()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts => opts.ResourcesPath = "Resources")
                .AddMvcLocalization(options => { options.ResourcesPath = "Resources"; });

            services.AddHttpClient("GitHub", httpClient =>
            {
                httpClient.BaseAddress = new Uri("https://api.github.com/");
                httpClient.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/vnd.github.v3+json");
                httpClient.DefaultRequestHeaders.Add(HeaderNames.UserAgent, "HttpRequestsSample");
            }
            );

            //Add Typed client:
            services.AddHttpClient<IGitHubService, GitHubService>();

            services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
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
            else
            {
                //app.UseExceptionHandler("/Errors");
                app.UseStatusCodePagesWithRedirects("~/Errors?httpStatusCode={0}")
                   .UseExceptionHandler("/Errors")
                   .UseHsts();
            }



            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseHttpLogging();

            //My Middleware:
            app.UseCorrelationMiddleware();

            //app.UseCorrelationId();

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
