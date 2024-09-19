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
            var initialMessages = _context.InitialMessages.OrderBy(m => m.CreatedAt).ToList();
            var productCards = _context.ProductCards.ToList(); // ����d�����
            var menus = _context.Menus.ToList(); // ��������
            var chatbotIcons = _context.ChatbotIcons.ToList(); // ��������
            var closeIcons = _context.CloseIcons.ToList(); // ��������
            var refreshIcons = _context.RefreshIcons.ToList(); // ��������

            var viewModel = new HomePageViewModel
            {
                InitialMessages = initialMessages,
                ProductCards = productCards,
                Menus = menus,
                ChatbotIcons = chatbotIcons,
                CloseIcons = closeIcons,
                RefreshIcons = refreshIcons
            };

            return View(viewModel);
        }

        public IActionResult Bot()
        {
            var initialMessages = _context.InitialMessages.OrderBy(m => m.CreatedAt).ToList();
            var productCards = _context.ProductCards.ToList(); // ����d�����
            var menus = _context.Menus.ToList(); // ��������
            var chatbotIcons = _context.ChatbotIcons.ToList(); // ��������
            var closeIcons = _context.CloseIcons.ToList(); // ��������
            var refreshIcons = _context.RefreshIcons.ToList(); // ��������

            var viewModel = new HomePageViewModel
            {
                InitialMessages = initialMessages,
                ProductCards = productCards,
                Menus = menus,
                ChatbotIcons = chatbotIcons,
                CloseIcons = closeIcons,
                RefreshIcons = refreshIcons
            };

            return View(viewModel);
        }

        public IActionResult test()
        {
            return View();
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
