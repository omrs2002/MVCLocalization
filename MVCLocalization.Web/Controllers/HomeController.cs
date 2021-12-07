using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using MVCLocalization.Web.Configuration;
using MVCLocalization.Web.Models;
using MVCLocalization.Web.Services;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace MVCLocalization.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;

        private readonly IHttpClientFactory _httpClient;
        private readonly IGitHubService _GetHubService;
        private readonly ILogger<HomeController> _logger;
        private readonly IOptions<Keys> _mykeys;

        public HomeController(ILogger<HomeController> logger,
         IStringLocalizer<HomeController> localizer,
         IHttpClientFactory httpClientFactory,
         IGitHubService GetHubService,
         IOptions<Keys> mykeys
         )
        {
            _logger = logger;
            _localizer = localizer;
            _mykeys = mykeys;
            _httpClient = httpClientFactory;
            _GetHubService = GetHubService;
        }

        public IEnumerable<GitHubBranch> GitHubBranches { get; set; }

        [HttpGet]
        public async Task<IEnumerable<GitHubBranch>> OnGet()
        {
            string serviceUrl = "https://api.github.com/repos/dotnet/AspNetCore.Docs/branches";

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, serviceUrl)
            {
                Headers =
                {
                    { HeaderNames.Accept, "application/vnd.github.v3+json" },
                    { HeaderNames.UserAgent, "HttpRequestsSample" },
                }
            };

            var httpClient = _httpClient.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                GitHubBranches = await JsonSerializer.DeserializeAsync<IEnumerable<GitHubBranch>>(contentStream);
            }

            return GitHubBranches;
        }


        [HttpGet]
        public async Task<IEnumerable<GitHubBranch>> OnGetHubNamed()
        {
            var httpClient = _httpClient.CreateClient("GitHub");
            var httpResponseMessage = await httpClient.GetAsync("repos/dotnet/AspNetCore.Docs/branches");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                GitHubBranches = await JsonSerializer.DeserializeAsync<IEnumerable<GitHubBranch>>(contentStream);
            }

            return GitHubBranches;
        }

        [HttpGet]
        public async Task<IEnumerable<GitHubBranch>> OnGetHubTyped()
        {
            var hubs = await _GetHubService.GetAspNetCoreDocsBranchesAsync();
            return hubs;
        }


        public IActionResult Index()
        {
            ViewData["Title"] = _localizer["Title"].Value;
            Console.WriteLine(Environment.NewLine + "loged user name : " + _mykeys.Value.Password + ", msg from home controller" + Environment.NewLine);
            return View();
        }

        public IActionResult ChangeCalture()
        {
            string culture = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
            bool isArabic = culture.ToLower().Contains("ar");
            string change_to = isArabic ? "en-GB" : "ar-SA";
            //Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(change_to);
            //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(change_to);

            //return RedirectToAction("Index");

            Thread.CurrentThread.CurrentCulture = new CultureInfo(change_to);
            CultureInfo.CurrentCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            Response.Cookies.Append(
                   CookieRequestCultureProvider.DefaultCookieName,
                   CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(Thread.CurrentThread.CurrentCulture)),
                   new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
               );
            //if (string.IsNullOrEmpty(returnUrl))
            return Redirect("Index");
            //else
            //{
            //    returnUrl = Util.RemoveQueryStringByKey(returnUrl, "culture");
            //    return Redirect(returnUrl);
            //}
        }
        public IActionResult Privacy()
        {
            ViewBag.Title = _localizer["Policy"].Value;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}