using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace MVCLocalization.Web.Controllers
{
    public class ErrorsController : Controller
    {
        public IActionResult Index(HttpStatusCode httpStatusCode)
        {
            switch (httpStatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return View("AccessDenied");

                case HttpStatusCode.NotFound:
                    return View("NotFound");

                case HttpStatusCode.InternalServerError:
                    return View("Error");
                default:
                    return View("Error");
            }
        }
    }
}
