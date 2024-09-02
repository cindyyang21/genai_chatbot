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

        // 後台上傳圖片與文字
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile imageFile, string textContent)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                using (var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                var content = new Menu
                {
                    ImagePath = "/images/" + fileName,
                    TextContent = textContent
                };

                _context.Menus.Add(content);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Menu");
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
