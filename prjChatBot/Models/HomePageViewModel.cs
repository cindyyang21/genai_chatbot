namespace prjChatBot.Models
{
    public class HomePageViewModel
    {
        public IEnumerable<ProductCard> ProductCards { get; set; }
        public IEnumerable<InitialMessage> InitialMessages { get; set; }
        public IEnumerable<Menu> Menus { get; set; }
    }
}
