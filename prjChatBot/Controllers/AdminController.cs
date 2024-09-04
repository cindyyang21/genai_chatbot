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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(GeoDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
            var productCards = _context.ProductCards.ToList();
            return View(productCards);
        }

        public IActionResult CardCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CardCreate(ProductCard model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // 保存图片文件
                var fileName = Path.GetRandomFileName().Replace(".", "") + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                model.ImageFileName = fileName;
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Card));
            }

            return View(model);
        }

        public async Task<IActionResult> CardEdit(int id)
        {
            var productCard = await _context.ProductCards.FindAsync(id);
            if (productCard == null)
            {
                return NotFound();
            }
            return View(productCard);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CardEdit(int id, ProductCard productCard, IFormFile imageFile)
        {
            if (id != productCard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 處理圖片上傳邏輯
                    if (imageFile != null)
                    {
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        // 儲存圖片到伺服器
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // 更新模型中的ImageFileName
                        productCard.ImageFileName = uniqueFileName;
                    }

                    // 更新產品卡資料
                    _context.Update(productCard);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "無法儲存更改：" + ex.Message);
                    return View(productCard);
                }

                return RedirectToAction(nameof(Card));
            }

            // 如果模型驗證失敗，返回View並顯示錯誤
            return View(productCard);
        }

        // 刪除圖片與對應的資料庫記錄
        [HttpPost]
        public async Task<IActionResult> CardDelete(int id)
        {
            var content = await _context.ProductCards.FindAsync(id);
            if (content != null)
            {
                // 刪除對應的圖片文件
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", content.ImageFileName);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // 刪除資料庫中的記錄
                _context.ProductCards.Remove(content);
                await _context.SaveChangesAsync();
            }
            TempData["Success"] = "刪除成功";
            return RedirectToAction("Card");
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
        public async Task<IActionResult> MenuUpload(string menuName, IFormFile imageFile, string textContent)
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
                        int maxWidth = 400;
                        int maxHeight = 400;

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
        public async Task<IActionResult> MenuDelete(int id)
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
            TempData["Success"] = "刪除成功";
            return RedirectToAction("Menu");
        }

        // 編輯圖片與對應的資料庫記錄
        [HttpPost]
        public async Task<IActionResult> MenuEdit(int id, IFormFile imageFile, string textContent)
        {
            var content = await _context.Menus.FindAsync(id);
            if (content == null)
            {
                return NotFound();
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                var fileName = Path.GetRandomFileName().Replace(".", "") + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = imageFile.OpenReadStream())
                {
                    using (var image = Image.FromStream(stream))
                    {
                        // 設定目標大小 (例如 400x400)
                        int targetWidth = 400;
                        int targetHeight = 400;

                        // 計算比例以保持圖片的縱橫比
                        var ratioX = (double)targetWidth / image.Width;
                        var ratioY = (double)targetHeight / image.Height;
                        var ratio = Math.Min(ratioX, ratioY);

                        int newWidth = (int)(image.Width * ratio);
                        int newHeight = (int)(image.Height * ratio);

                        Bitmap resizedImage;

                        // 判断图片格式，如果是 PNG，保持透明背景
                        if (image.RawFormat.Equals(ImageFormat.Png))
                        {
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
                            resizedImage.Save(filePath, ImageFormat.Png); // 保存為 PNG 保留透明背景
                        }
                        else
                        {
                            resizedImage = new Bitmap(newWidth, newHeight);
                            using (var graphics = Graphics.FromImage(resizedImage))
                            {
                                graphics.CompositingQuality = CompositingQuality.HighQuality;
                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                graphics.SmoothingMode = SmoothingMode.HighQuality;
                                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
                            }
                            resizedImage.Save(filePath, ImageFormat.Jpeg); // 保存為 JPEG
                        }

                        // 删除旧图片文件
                        var oldImagePath = Path.Combine(uploadPath, content.ImageFileName);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                        content.ImageFileName = fileName; // 更新圖片文件名
                    }
                }
            }

            content.TextContent = textContent; // 更新文字内容
            _context.Update(content);
            await _context.SaveChangesAsync();

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
