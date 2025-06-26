namespace WebBanBanh.Models
{
    public class ChatLog
    {
        public int Id { get; set; }
        public string UserMessage { get; set; }
        public string BotResponse { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
