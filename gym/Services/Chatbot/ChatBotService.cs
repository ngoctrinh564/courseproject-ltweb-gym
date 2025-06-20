using gym.Data;
using gym.Models.Chatbot;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;

namespace gym.Services.Chatbot
{
    public class ChatBotService : IChatBotService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAIConfig _config;
        private readonly GymContext _dbContext;

        public ChatBotService(HttpClient httpClient, IOptions<OpenAIConfig> config, GymContext dbContext)
        {
            _httpClient = httpClient;
            _config = config.Value;
            _dbContext = dbContext;

            _httpClient.BaseAddress = new Uri(_config.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.ApiKey}");
        }

        public async Task<string> GetAnswerAsync(string message)
        {
            // Trích xuất dữ liệu Package thành văn bản để đưa làm context
            var packages = await _dbContext.Packages.ToListAsync();
            var contextList = packages.Select(p => $"Gói tập: {p.Name}, Loại: {p.Type}, Giá: {p.Price} VNĐ, Thời hạn: {p.DurationInDays} ngày. Mô tả: {p.Description}");
            var context = string.Join("\n", contextList);

            var prompt = $"Dựa trên thông tin các gói tập dưới đây, hãy trả lời câu hỏi của người dùng một cách tự nhiên và chính xác.\n\nThông tin gói tập:\n{context}\n\nCâu hỏi: {message}";

            var fullUrl = $"{_config.BaseUrl.TrimEnd('/')}/chat/completions";
            var response = await _httpClient.PostAsJsonAsync(fullUrl, new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "system", content = "Bạn là trợ lý AI thông minh, hãy trả lời rõ ràng." },
                    new { role = "user", content = prompt }
                }
            });

            var json = await response.Content.ReadAsStringAsync();

            // 👇 Debug nếu lỗi
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Lỗi khi gọi OpenAI API: {response.StatusCode} \nNội dung: {json}");
            }

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement
                      .GetProperty("choices")[0]
                      .GetProperty("message")
                      .GetProperty("content")
                      .GetString();

        }
    }
}
