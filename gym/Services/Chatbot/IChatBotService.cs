namespace gym.Services.Chatbot
{
    public interface IChatBotService
    {
        Task<string> GetAnswerAsync(string message);
    }
}
