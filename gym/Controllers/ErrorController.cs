using gym.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace gym.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        public IActionResult Index()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        [Route("Error/404")]
        public IActionResult Error404()
        {
            return View("NotFound");
        }

        [Route("Error/403")]
        public IActionResult Error403()
        {
            return View("AccessDenied");
        }

        [Route("Error/500")]
        public IActionResult Error500()
        {
            return View("ServerError");
        }

        [Route("Error/{statusCode}")]
        public IActionResult GenericError(int statusCode)
        {
            Response.StatusCode = statusCode; // Đảm bảo status code được giữ

            return statusCode switch
            {
                403 => View("AccessDenied"),
                404 => View("NotFound"),
                500 => View("ServerError"),
                _ => View("GenericError", statusCode)
            };
        }
    }
}
