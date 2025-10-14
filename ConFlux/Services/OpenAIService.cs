using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace ConfluxApp.Services
{
    public class OpenAIService
    {
        private readonly HttpClient _httpClient;

        public OpenAIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetAnswerAsync(string question)
        {
            var requestBody = new
            {
                model = "gpt-3.5-turbo",
                messages = new[]
                {
                    new { role = "user", content = question }
                }
            };

            var response = await _httpClient.PostAsJsonAsync("chat/completions", requestBody);

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Greška: {response.StatusCode}");

            var result = await response.Content.ReadFromJsonAsync<OpenAiResponse>();
            return result?.Choices?.FirstOrDefault()?.Message?.Content ?? "Nema odgovora.";
        }
    }

    // modeli za OpenAI JSON odgovor
    public class OpenAiResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice>? Choices { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("message")]
        public Message? Message { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}
