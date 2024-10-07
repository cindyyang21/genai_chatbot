using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prjChatBot.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.PixelFormats;


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


        // 顯示名稱的動作
        public IActionResult Header()
        {
            // 從資料庫中獲取第一筆 BotName 資料
            var botName = _context.BotNames.FirstOrDefault();

            // 從資料庫中獲取最新一筆 ColorSelection 資料
            var latestColor = _context.ColorSelections.OrderByDescending(c => c.Id).FirstOrDefault();

            // 將 BotName 和 ColorSelection 資料包裝到 ViewModel 中傳遞給視圖
            var viewModel = new HomePageViewModel
            {
                BotNames = botName, // 單一物件而非集合
                ColorSelections = latestColor != null ? new List<ColorSelection> { latestColor } : new List<ColorSelection>()
            };

            return View(viewModel);
        }

        // GET: 新增名稱的頁面
        public IActionResult HeaderCreate()
        {
            return View();
        }

        // POST: 提交新增的名稱
        [HttpPost]
        public IActionResult HeaderCreate(BotName botName)
        {
            if (ModelState.IsValid)
            {
                _context.BotNames.Add(botName); // 將新名稱加入資料庫
                _context.SaveChanges(); // 儲存變更
                return RedirectToAction("Header"); // 新增後返回首頁
            }
            return View(botName);
        }

        // GET: 顯示更新名稱的頁面
        public IActionResult UpdateName(int id)
        {
            var botName = _context.BotNames.Find(id); // 從資料庫查找對應的 BotName
            if (botName == null)
            {
                // 如果找不到資料，返回到主頁，並顯示錯誤消息
                TempData["ErrorMessage"] = "無法找到對應的名稱";
                return RedirectToAction("Header");
            }

            return View(botName); // 將 BotName 資料傳遞給視圖
        }


        [HttpPost]
        public IActionResult UpdateName(BotName botName)
        {
            if (ModelState.IsValid)
            {
                var entity = _context.BotNames.Find(botName.Id);
                if (entity != null)
                {
                    entity.Name = botName.Name; // 更新名稱
                    _context.SaveChanges(); // 儲存變更

                    TempData["SuccessMessage"] = "名稱已成功更新";
                }
                else
                {
                    TempData["ErrorMessage"] = "更新失敗，找不到對應的記錄";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "更新失敗，表單資料無效";
            }

            return RedirectToAction("Header"); // 更新後返回首頁
        }


        // POST: 提交選擇的顏色
        [HttpPost]
        public IActionResult ColorPicker(string colorCode)
        {
            if (!string.IsNullOrEmpty(colorCode))
            {
                var colorSelection = new ColorSelection
                {
                    ColorCode = colorCode
                };

                _context.ColorSelections.Add(colorSelection);
                _context.SaveChanges();

                TempData["Success"] = "顏色已成功儲存!";
                return RedirectToAction("Header");
            }

            TempData["Error"] = "請選擇一個顏色!";
            return View("Header");
        }


        public IActionResult Integration()
        {
            // 獲取聊天機器人嵌入的 URL，假設聊天機器人的 URL 是 "/Home/Bot"
            var chatbotEmbedUrl = Url.Action("Bot", "Home", null, Request.Scheme);

            // 傳遞 URL 到視圖中
            ViewBag.ChatbotEmbedUrl = chatbotEmbedUrl;
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

        [HttpGet]
        [Route("Admin/GetInitialMessages")]
        public IActionResult GetInitialMessages()
        {
            // 使用依賴注入的方式獲取 DbContext
            var initialMessages = _context.InitialMessages
                .Select(m => new { m.Message }) // 只選取 Message 欄位
                .ToList();

            // 將結果以 JSON 格式返回給前端
            return Json(initialMessages);
        }


        public IActionResult Icon()
        {
            // 從資料庫中獲取多個 icon 的資料
            var model = new HomePageViewModel
            {
                ChatbotIcons = _context.ChatbotIcons.ToList(),   // 假設有多個 ChatbotIcons 資料
                CloseIcons = _context.CloseIcons.ToList(),       // 假設有多個 CloseIcons 資料
                RefreshIcons = _context.RefreshIcons.ToList()    // 假設有多個 RefreshIcons 資料
            };

            return View(model);
        }

        public IActionResult ChatbotIconCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChatbotIconCreate(ChatbotIcon model, IFormFile imageFile)
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
                // 如果是PNG，處理透明背景
                if (Path.GetExtension(imageFile.FileName).ToLower() == ".png")
                {
                    using (var image = Image.Load<Rgba32>(filePath))  // 使用 Rgba32 保留透明度
                    {
                        // 縮放圖片
                        image.Mutate(x => x.Resize(60, 60));

                        // 再次保存處理過的 PNG 圖片，保持背景透明
                        image.Save(filePath);  // ImageSharp 會根據擴展名自動保存為 PNG
                    }
                }

                model.Picture = fileName;
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "新增成功";
                return RedirectToAction(nameof(Icon));
            }

            return View(model);
        }




        // 顯示 ChatbotIconEdit 表單
        public async Task<IActionResult> ChatbotIconEdit(int id)
        {
            var chatbotIcon = await _context.ChatbotIcons.FindAsync(id);
            if (chatbotIcon == null)
            {
                return NotFound();
            }
            return View(chatbotIcon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChatbotIconEdit(int id, ChatbotIcon chatbotIcon, IFormFile imageFile)
        {
            if (id != chatbotIcon.Id)
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

                        // 刪除舊圖片檔案（如果存在）
                        if (!string.IsNullOrEmpty(chatbotIcon.Picture))
                        {
                            var oldFilePath = Path.Combine(uploadsFolder, chatbotIcon.Picture);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // 儲存圖片到伺服器
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // 將圖片縮放至 60x60
                        ResizeImageWithTransparentBackground(filePath, 60, 60);

                        // 更新模型中的Picture
                        chatbotIcon.Picture = uniqueFileName;
                    }

                    // 更新模型中的CreatedAt欄位
                    chatbotIcon.CreatedAt = DateTime.Now;

                    // 更新產品卡資料
                    _context.Update(chatbotIcon);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "無法儲存更改：" + ex.Message);
                    return View(chatbotIcon);
                }

                TempData["Success"] = "編輯成功";
                return RedirectToAction(nameof(Icon));
            }

            // 如果模型驗證失敗，返回View並顯示錯誤
            return View(chatbotIcon);
        }


        // 刪除機器人圖片與對應的資料庫記錄
        [HttpPost]
        public async Task<IActionResult> ChatbotIconDelete(int id)
        {
            var content = await _context.ChatbotIcons.FindAsync(id);
            if (content != null)
            {
                // 刪除對應的圖片文件
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", content.Picture);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // 刪除資料庫中的記錄
                _context.ChatbotIcons.Remove(content);
                await _context.SaveChangesAsync();
            }
            TempData["Success"] = "刪除成功";
            return RedirectToAction("Icon");
        }


        public IActionResult CloseIconCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CloseIconCreate(CloseIcon model, IFormFile imageFile)
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
                // 如果是PNG，處理透明背景
                if (Path.GetExtension(imageFile.FileName).ToLower() == ".png")
                {
                    using (var image = Image.Load<Rgba32>(filePath))  // 使用 Rgba32 保留透明度
                    {
                        // 縮放圖片
                        image.Mutate(x => x.Resize(47, 47));

                        // 再次保存處理過的 PNG 圖片，保持背景透明
                        image.Save(filePath);  // ImageSharp 會根據擴展名自動保存為 PNG
                    }
                }

                model.Picture = fileName;
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "新增成功";
                return RedirectToAction(nameof(Icon));
            }

            return View(model);
        }


        // 顯示 CloseIconEdit 表單
        public async Task<IActionResult> CloseIconEdit(int id)
        {
            var closeIcon = await _context.CloseIcons.FindAsync(id);
            if (closeIcon == null)
            {
                return NotFound();
            }
            return View(closeIcon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseIconEdit(int id, CloseIcon closeIcon, IFormFile imageFile)
        {
            if (id != closeIcon.Id)
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

                        // 刪除舊圖片檔案（如果存在）
                        if (!string.IsNullOrEmpty(closeIcon.Picture))
                        {
                            var oldFilePath = Path.Combine(uploadsFolder, closeIcon.Picture);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // 儲存圖片到伺服器
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // 將圖片縮放至 47x47
                        ResizeImageWithTransparentBackground(filePath, 47, 47);

                        // 更新模型中的Picture
                        closeIcon.Picture = uniqueFileName;
                    }

                    // 更新模型中的CreatedAt欄位
                    closeIcon.CreatedAt = DateTime.Now;

                    // 更新產品卡資料
                    _context.Update(closeIcon);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "無法儲存更改：" + ex.Message);
                    return View(closeIcon);
                }

                TempData["Success"] = "編輯成功";
                return RedirectToAction(nameof(Icon));
            }

            // 如果模型驗證失敗，返回View並顯示錯誤
            return View(closeIcon);
        }


        // 刪除關閉之圖片與對應的資料庫記錄
        [HttpPost]
        public async Task<IActionResult> CloseIconDelete(int id)
        {
            var content = await _context.CloseIcons.FindAsync(id);
            if (content != null)
            {
                // 刪除對應的圖片文件
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", content.Picture);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // 刪除資料庫中的記錄
                _context.CloseIcons.Remove(content);
                await _context.SaveChangesAsync();
            }
            TempData["Success"] = "刪除成功";
            return RedirectToAction("Icon");
        }


        public IActionResult RefreshIconCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RefreshIconCreate(RefreshIcon model, IFormFile imageFile)
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
                // 如果是PNG，處理透明背景
                if (Path.GetExtension(imageFile.FileName).ToLower() == ".png")
                {
                    using (var image = Image.Load<Rgba32>(filePath))  // 使用 Rgba32 保留透明度
                    {
                        // 縮放圖片
                        image.Mutate(x => x.Resize(47, 47));
                        // 再次保存處理過的 PNG 圖片，保持背景透明
                        image.Save(filePath);  // ImageSharp 會根據擴展名自動保存為 PNG
                    }
                }

                model.Picture = fileName;
                _context.Add(model);
                await _context.SaveChangesAsync();
                TempData["Success"] = "新增成功";
                return RedirectToAction(nameof(Icon));
            }

            return View(model);
        }


        // 顯示 RefreshIconEdit 表單
        public async Task<IActionResult> RefreshIconEdit(int id)
        {
            var refreshIcon = await _context.RefreshIcons.FindAsync(id);
            if (refreshIcon == null)
            {
                return NotFound();
            }
            return View(refreshIcon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RefreshIconEdit(int id, RefreshIcon refreshIcon, IFormFile imageFile)
        {
            if (id != refreshIcon.Id)
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

                        // 刪除舊圖片檔案（如果存在）
                        if (!string.IsNullOrEmpty(refreshIcon.Picture))
                        {
                            var oldFilePath = Path.Combine(uploadsFolder, refreshIcon.Picture);
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // 儲存圖片到伺服器
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        // 將圖片縮放至 47x47
                        ResizeImageWithTransparentBackground(filePath, 47, 47);
                        // 更新模型中的Picture
                        refreshIcon.Picture = uniqueFileName;
                    }

                    // 更新模型中的CreatedAt欄位
                    refreshIcon.CreatedAt = DateTime.Now;

                    // 更新產品卡資料
                    _context.Update(refreshIcon);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "無法儲存更改：" + ex.Message);
                    return View(refreshIcon);
                }

                TempData["Success"] = "編輯成功";
                return RedirectToAction(nameof(Icon));
            }

            // 如果模型驗證失敗，返回View並顯示錯誤
            return View(refreshIcon);
        }


        // 刪除關閉之圖片與對應的資料庫記錄
        [HttpPost]
        public async Task<IActionResult> RefreshIconDelete(int id)
        {
            var content = await _context.RefreshIcons.FindAsync(id);
            if (content != null)
            {
                // 刪除對應的圖片文件
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", content.Picture);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                // 刪除資料庫中的記錄
                _context.RefreshIcons.Remove(content);
                await _context.SaveChangesAsync();
            }
            TempData["Success"] = "刪除成功";
            return RedirectToAction("Icon");
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
                TempData["Success"] = "新增成功";
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
                        ResizeImageWithTransparentBackground(filePath, 500, 500);

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

                TempData["Success"] = "編輯成功";
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

        [HttpGet]
        [Route("Admin/GetProductCards")]
        public IActionResult GetProductCards()
        {
            // 從資料庫中抓取所有的卡片資料
            var productCards = _context.ProductCards.ToList();

            // 返回卡片資料作為 JSON 格式
            return Json(productCards);
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

            TempData["Success"] = "上傳成功";
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

            TempData["Success"] = "編輯成功";
            return RedirectToAction("Menu");
        }

        public IActionResult Statics()
        {
            // 從資料庫中抓取所有回饋
            var positiveFeedbacks = _context.Feedbacks.Where(f => f.FeedbackType == "up").ToList();
            var negativeFeedbacks = _context.Feedbacks.Where(f => f.FeedbackType == "down").ToList();

            // 統計按讚的原因及次數
            var positiveReasonsCount = positiveFeedbacks
                .SelectMany(f => f.Reasons)  // 展平原因列表
                .GroupBy(reason => reason == "其他" ? "其他" : reason)  // 統一將「其他」原因歸為「其他」
                .Select(g => new ReasonCount
                {
                    Reason = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // 統計按倒讚的原因及次數
            var negativeReasonsCount = negativeFeedbacks
                .SelectMany(f => f.Reasons)
                 .GroupBy(reason => reason == "其他" ? "其他" : reason)  // 統一將「其他」原因歸為「其他」
                .Select(g => new ReasonCount
                {
                    Reason = g.Key,
                    Count = g.Count()
                })
                .ToList();

            // 將結果存入 ViewModel
            var model = new FeedbackStaticsViewModel
            {
                PositiveReasonsCount = positiveReasonsCount,
                NegativeReasonsCount = negativeReasonsCount
            };

            // 傳遞 ViewModel 給視圖
            return View(model);
        }

        public IActionResult Feedback()
        {
            var positiveFeedbacks = _context.Feedbacks.Where(f => f.FeedbackType == "up").ToList();
            var negativeFeedbacks = _context.Feedbacks.Where(f => f.FeedbackType == "down").ToList();

            var model = new FeedbackViewModel
            {
                PositiveFeedbacks = positiveFeedbacks,
                NegativeFeedbacks = negativeFeedbacks
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitFeedback([FromBody] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.SubmittedAt = DateTime.Now; // 設置提交時間
                if (!string.IsNullOrEmpty(feedback.OtherReason))
                {
                    feedback.Reasons.Add("其他");  // 將「其他原因」歸類為「其他」
                }
                _context.Feedbacks.Add(feedback);     // 將回饋資料保存到資料庫
                _context.SaveChanges();               // 保存更改
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost]
        public IActionResult FeedbackDelete(int id)
        {
            var feedback = _context.Feedbacks.Find(id);
            if (feedback == null)
            {
                return NotFound();
            }
            // 刪除資料庫中的回饋記錄
            _context.Feedbacks.Remove(feedback);
            _context.SaveChanges();
            // 更新圓餅圖數據
            UpdatePieChartData(feedback.FeedbackType, feedback.Reasons);
            // 設置刪除成功的提示消息
            TempData["Success"] = "刪除成功";

            return RedirectToAction("Feedback");  // 返回回饋列表頁面
        }


        private void UpdatePieChartData(string feedbackType, List<string> reasons)
        {
            foreach (var reason in reasons)
            {
                var chartData = feedbackType == "up"
                    ? _context.PieChartData.FirstOrDefault(d => d.Type == "up" && d.Reason == reason)
                    : _context.PieChartData.FirstOrDefault(d => d.Type == "down" && d.Reason == reason);

                if (chartData != null)
                {
                    chartData.Count -= 1;

                    if (chartData.Count <= 0)
                    {
                        _context.PieChartData.Remove(chartData);
                    }
                }
            }

            _context.SaveChanges();
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

        private void ResizeImageWithTransparentBackground(string filePath, int width, int height)
        {
            using (var image = Image.Load<Rgba32>(filePath))
            {
                image.Mutate(x => x.Resize(width, height));

                // 保持PNG透明背景
                image.Save(filePath, new PngEncoder());
            }
        }



    }
}
