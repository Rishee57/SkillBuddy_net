using Microsoft.AspNetCore.Mvc;
using SkillBuddy.ui.Services;

namespace SkillBuddy.ui.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestEmailController : ControllerBase
    {
        private readonly EmailPublisher _emailPublisher;

        public TestEmailController(EmailPublisher emailPublisher)
        {
            _emailPublisher = emailPublisher;
        }

        [HttpPost("publish-welcome")]
        public async Task<IActionResult> PublishWelcomeEmail([FromQuery] string toEmail, [FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(toEmail) || string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("toEmail and name are required.");
            }

            var emailMessage = new EmailMessage
            {
                To = toEmail,
                Subject = "Welcome to SkillBuddy!",
                Body = $"Hello {name},\n\nWelcome to SkillBuddy! We are excited to have you on board as a testing purpose feature.\n\nBest regards,\nThe SkillBuddy Team"
            };

            await _emailPublisher.PublishEmailAsync(emailMessage);

            return Ok(new { Message = "Welcome email published to queue successfully!" });
        }
        // Example endpoint to get connection details as JSON
        [HttpGet("connection-info")]
        public IActionResult GetRabbitMqConnectionInfo()
        {
            // Get the JSON string from our new method
            var jsonProperties = _emailPublisher.GetConnectionPropertiesJson();

            // Return it as a proper JSON content response
            return Content(jsonProperties, "application/json");
        }
    }
}
