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
            var contentList = _context.Menus.ToList(); // 獲取所有資料
            return View(contentList); // 傳遞資料到視圖
        }
        public IActionResult MenuCreate()
        {
            return View();
        }

        // 上傳圖片與文字
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile imageFile, string textContent)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using (var stream = imageFile.OpenReadStream())
                {
                    using (var image = Image.FromStream(stream))
                    {
                        // 設定目標大小 (例如 800x600)
                        int targetWidth = 800;
                        int targetHeight = 600;

                        // 計算比例並調整大小
                        var resizedImage = new Bitmap(targetWidth, targetHeight);
                        using (var graphics = Graphics.FromImage(resizedImage))
                        {
                            graphics.CompositingQuality = CompositingQuality.HighQuality;
                            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            graphics.SmoothingMode = SmoothingMode.HighQuality;
                            graphics.DrawImage(image, 0, 0, targetWidth, targetHeight);
                        }

                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        var fileName = Path.GetRandomFileName().Replace(".", "") + Path.GetExtension(imageFile.FileName);
                        var filePath = Path.Combine(uploadPath, fileName);

                        resizedImage.Save(filePath, ImageFormat.Jpeg); // 儲存圖片

                        var content = new Menu
                        {
                            ImageFileName = fileName,
                            TextContent = textContent,
                            CreatedAt = DateTime.Now
                        };

                        _context.Menus.Add(content);
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction("Menu");
            }

            ModelState.AddModelError("ImageFile", "請選擇一張圖片");
            return View();

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
