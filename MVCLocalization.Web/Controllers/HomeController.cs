using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MVCLocalization.Web.Models;
using System.Diagnostics;
using System.Globalization;

namespace MVCLocalization.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<HomeController> _localizer;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger,
            IStringLocalizer<HomeController> localizer
            )
        {
            _logger = logger;
            _localizer = localizer; 
        }

        public IActionResult Index()
        {
            ViewData["Title"] = _localizer["Title"].Value;

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