using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjChatBot.Models;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;


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
            // 獲取所有資料
            var contentList = _context.Menus.ToList(); 
            // 確保有六個預設的選單項目
            var defaultMenuNames = new List<string> { "1", "2", "3", "4", "5", "6" };

            foreach (var menuName in defaultMenuNames)
            {
                if (!contentList.Any(m => m.Name == menuName))
                {
                    contentList.Add(new Menu { Name = menuName, ImageFileName = null, TextContent = null });
                }
            }

            return View(contentList);
        }


        // 上傳圖片與文字
        [HttpPost]
        public async Task<IActionResult> Upload(string menuName, IFormFile imageFile, string textContent)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                var fileName = Path.GetRandomFileName().Replace(".", "") + Path.GetExtension(imageFile.FileName);

                using (var stream = new FileStream(Path.Combine(uploadPath, fileName), FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                var menu = _context.Menus.FirstOrDefault(m => m.Name == menuName);
                if (menu != null)
                {
                    menu.ImageFileName = fileName;
                    menu.TextContent = textContent;
                    _context.Update(menu);
                }
                else
                {
                    menu = new Menu
                    {
                        Name = menuName,
                        ImageFileName = fileName,
                        TextContent = textContent
                    };
                    _context.Add(menu);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Menu");

        }


        // 顯示圖片
        public IActionResult ShowImage(int id)
        {
            var content = _context.Menus.FirstOrDefault(c => c.Id == id);
            if (content != null && !string.IsNullOrEmpty(content.ImageFileName))
            {
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", content.ImageFileName);
                var imageFileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
                var mimeType = GetMimeType(content.ImageFileName);

                return File(imageFileStream, mimeType);
            }

            return NotFound();
        }

        private string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }

        // 刪除圖片與對應的資料庫記錄
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var content = await _context.Menus.FindAsync(id);
            if (content != null)
            {
                // 刪除對應的圖片文件
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", content.ImageFileName);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // 刪除資料庫中的記錄
                _context.Menus.Remove(content);
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
