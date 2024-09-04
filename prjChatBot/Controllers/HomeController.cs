using Microsoft.AspNetCore.Mvc;
using prjChatBot.Models;
using System.Diagnostics;

namespace prjChatBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly GeoDbContext _context;

        public HomeController(GeoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var productCards = _context.ProductCards.ToList(); // ����d�����
            var menus = _context.Menus.ToList(); // ��������

            var viewModel = new HomePageViewModel
            {
                ProductCards = productCards,
                Menus = menus
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
