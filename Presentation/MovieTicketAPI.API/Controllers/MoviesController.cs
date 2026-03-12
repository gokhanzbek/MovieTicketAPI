using Microsoft.AspNetCore.Mvc;

namespace MovieTicketAPI.API.Controllers
{
    public class MoviesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
