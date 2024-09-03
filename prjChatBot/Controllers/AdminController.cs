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
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                var fileName = Path.GetRandomFileName().Replace(".", "") + fileExtension; // 保留原始文件扩展名
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = imageFile.OpenReadStream())
                {
                    using (var image = System.Drawing.Image.FromStream(stream))
                    {
                        // 设置最大宽度和高度
                        int maxWidth = 800;
                        int maxHeight = 600;

                        // 计算比例并调整大小
                        var ratioX = (double)maxWidth / image.Width;
                        var ratioY = (double)maxHeight / image.Height;
                        var ratio = Math.Min(ratioX, ratioY);

                        var newWidth = (int)(image.Width * ratio);
                        var newHeight = (int)(image.Height * ratio);

                        Bitmap resizedImage;

                        if (fileExtension == ".png")
                        {
                            // 对于 PNG 格式，保留透明背景
                            resizedImage = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);
                            using (var graphics = Graphics.FromImage(resizedImage))
                            {
                                graphics.CompositingQuality = CompositingQuality.HighQuality;
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.SmoothingMode = SmoothingMode.HighQuality;
                                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                                graphics.Clear(Color.Transparent); // 保持透明背景
                                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                            }
                            resizedImage.Save(filePath, ImageFormat.Png); // 保存为 PNG 格式
                        }
                        else
                        {
                            // 对于非 PNG 格式，正常处理
                            resizedImage = new Bitmap(newWidth, newHeight);
                            using (var graphics = Graphics.FromImage(resizedImage))
                            {
                                graphics.CompositingQuality = CompositingQuality.HighQuality;
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.SmoothingMode = SmoothingMode.HighQuality;
                                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                            }
                            resizedImage.Save(filePath, ImageFormat.Jpeg); // 保存为 JPEG 格式或其他适当格式
                        }
                    }
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
