using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjChatBot.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;


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


        // 顯示歡迎訊息
        public IActionResult Welcome()
        {
            var messages = _context.InitialMessages.OrderByDescending(m => m.CreatedAt).ToList();  // 確保是 List<T>
            return View(messages);
        }


        // 顯示新增或編輯歡迎訊息的表單
        [HttpGet]
        public IActionResult WelcomeCreate()
        {
            return View();
        }

        // 提交歡迎訊息
        [HttpPost]
        public IActionResult WelcomeCreate(InitialMessage newMessage)
        {
            if (ModelState.IsValid)
            {
                newMessage.CreatedAt = DateTime.UtcNow;
                _context.InitialMessages.Add(newMessage);  // 新增新訊息到資料庫
                _context.SaveChanges();
                return RedirectToAction("Welcome");  // 重新導向到訊息列表
            }

            return View(newMessage);
        }


        // 編輯現有的歡迎訊息
        [HttpGet]
        public IActionResult WelcomeEdit(int id)
        {
            var message = _context.InitialMessages.FirstOrDefault(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }
            return View(message);
        }

        [HttpPost]
        public IActionResult WelcomeEdit(InitialMessage updatedMessage)
        {
            if (ModelState.IsValid)
            {
                updatedMessage.CreatedAt = DateTime.UtcNow;
                _context.InitialMessages.Update(updatedMessage);  // 更新訊息
                _context.SaveChanges();
                return RedirectToAction("Welcome");  // 重新導向到訊息列表
            }

            return View(updatedMessage);
        }

        [HttpPost]
        public IActionResult WelcomeDelete(int id)
        {
            // 找到要刪除的訊息
            var message = _context.InitialMessages.Find(id);
            if (message == null)
            {
                return NotFound();
            }

            // 從資料庫中刪除
            _context.InitialMessages.Remove(message);
            _context.SaveChanges();

            // 刪除後重定向回列表頁面
            return RedirectToAction("Welcome");
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
                // 處理圖片縮放
                ResizeImage(filePath, 500, 500);

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

                        // 將圖片縮放至 500x500
                        ResizeImage(filePath, 500, 500);

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
                // 準備保存路徑
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath); // 如果資料夾不存在，創建資料夾
                }

                var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                var fileName = Path.GetRandomFileName().Replace(".", "") + fileExtension; // 保留原始文件扩展名
                var filePath = Path.Combine(uploadPath, fileName);

                using (var image = await SixLabors.ImageSharp.Image.LoadAsync(imageFile.OpenReadStream()))
                {
                    // 設定最大寬度和高度
                    int maxWidth = 400;
                    int maxHeight = 400;

                    // 計算縮放比例
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(maxWidth, maxHeight),
                        Mode = ResizeMode.Max // 保持比例縮放
                    }));

                    // 根據文件擴展名保存文件
                    if (fileExtension == ".png")
                    {
                        // 保留透明背景並保存為 PNG
                        await image.SaveAsync(filePath, new PngEncoder());
                    }
                    else
                    {
                        // 保存為 JPEG 或其他格式
                        await image.SaveAsync(filePath, new JpegEncoder { Quality = 90 });
                    }
                }

                // 更新或創建菜單
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
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath); // 如果目錄不存在，則創建目錄
                }

                var fileName = Path.GetRandomFileName().Replace(".", "") + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadPath, fileName);

                // 使用 ImageSharp 打開圖片文件
                using (var image = await SixLabors.ImageSharp.Image.LoadAsync(imageFile.OpenReadStream()))
                {
                    // 設定目標寬度和高度 (例如 400x400)
                    int targetWidth = 400;
                    int targetHeight = 400;

                    // 使用 ImageSharp 進行圖片縮放
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size(targetWidth, targetHeight),
                        Mode = ResizeMode.Max // 保持圖片比例，不超出目標尺寸
                    }));

                    // 檢查圖片的擴展名並決定保存格式
                    if (Path.GetExtension(imageFile.FileName).ToLower() == ".png")
                    {
                        // 保存為 PNG，保持透明背景
                        await image.SaveAsync(filePath, new PngEncoder());
                    }
                    else
                    {
                        // 保存為 JPEG，設置質量
                        await image.SaveAsync(filePath, new JpegEncoder { Quality = 90 });
                    }

                    // 刪除舊圖片文件
                    var oldImagePath = Path.Combine(uploadPath, content.ImageFileName);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    // 更新圖片文件名到資料庫
                    content.ImageFileName = fileName;
                }
            }

            // 更新文字內容
            content.TextContent = textContent;
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

        // 修改 ResizeImage 方法來使用 ImageSharp
        private void ResizeImage(string imagePath, int width, int height)
        {
            using (var image = SixLabors.ImageSharp.Image.Load(imagePath))
            {
                // 縮放圖片至指定的寬高
                image.Mutate(x => x.Resize(width, height));

                // 保存為 JPEG 格式
                image.Save(imagePath, new JpegEncoder());
            }
        }

        

    }
}
