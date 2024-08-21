using Microsoft.AspNetCore.Mvc;


namespace prjChatBot.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult MemberAccount()
        {
            return View();
        }
        public IActionResult Feedback()
        {
            return View();
        }
    }
}
