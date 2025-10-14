namespace ConFlux.DTOs
{
    public class OpenAiDtos
    {
        public class QuestionRequest
        {
            public string Question { get; set; } = string.Empty;
        }

        public class AnswerResponse
        {
            public string Answer { get; set; } = string.Empty;
        }
    }
}
