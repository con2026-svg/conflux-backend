
using ConfluxApp.Services;
using Microsoft.AspNetCore.Mvc;
using static ConFlux.DTOs.OpenAiDtos;

namespace ConfluxApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly OpenAIService _openAI;

        public ChatController(OpenAIService openAI)
        {
            _openAI = openAI;
        }

        [HttpPost]
        public async Task<ActionResult<AnswerResponse>> PostQuestion([FromBody] QuestionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Question))
                return BadRequest(new { message = "Pitanje ne sme biti prazno." });

            var answer = await _openAI.GetAnswerAsync(request.Question);
            return Ok(new AnswerResponse { Answer = answer });
        }
    }
}
