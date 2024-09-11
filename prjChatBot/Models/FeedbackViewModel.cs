namespace prjChatBot.Models
{
    public class FeedbackViewModel
    {
        public List<Feedback> PositiveFeedbacks { get; set; }
        public List<Feedback> NegativeFeedbacks { get; set; }
    }

    public class FeedbackStaticsViewModel
    {
        // 用來存放每個原因及其對應的數量
        public List<ReasonCount> PositiveReasonsCount { get; set; } // 按讚的原因統計
        public List<ReasonCount> NegativeReasonsCount { get; set; } // 按倒讚的原因統計
    }

    public class ReasonCount
    {
        public string Reason { get; set; }  // 回饋原因
        public int Count { get; set; }      // 該原因的次數
    }

}
