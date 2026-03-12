using Microsoft.AspNetCore.Mvc;

namespace MovieTicketAPI.API.Controllers
{
    public class HallsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
