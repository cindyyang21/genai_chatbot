using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjChatBot.Models;


namespace prjChatBot.Controllers
{
    public class AdminController : Controller
    {
        private readonly GeoDbContext _context;

        public AdminController(GeoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Welcome()
        {
            return View();
        }
        public IActionResult Icon()
        {
            return View();
        }
        public IActionResult Card()
        {
            return View();
        }
        public IActionResult Menu()
        {
            return View();
        }
        public IActionResult MenuCreate()
        {
            return View();
        }
        public IActionResult Statics()
        {
            return View();
        }
        public IActionResult Feedback()
        {
            return View();
        }

    }
}
