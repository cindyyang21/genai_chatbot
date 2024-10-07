namespace prjChatBot.Models
{
    public class HomePageViewModel
    {
        public IEnumerable<ProductCard> ProductCards { get; set; }
        public IEnumerable<InitialMessage> InitialMessages { get; set; }
        public IEnumerable<Menu> Menus { get; set; }
        public IEnumerable<ChatbotIcon> ChatbotIcons { get; set; }
        public IEnumerable<CloseIcon> CloseIcons { get; set; }
        public IEnumerable<RefreshIcon> RefreshIcons { get; set; }
        public BotName BotNames { get; set; } // 單一名稱
        public IEnumerable<ColorSelection> ColorSelections { get; set; } // 保持多個顏色選擇
    }
}
