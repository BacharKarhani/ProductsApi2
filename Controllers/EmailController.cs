using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly IEmailRepository _emailRepository;

    public EmailController(IEmailRepository emailRepository)
    {
        _emailRepository = emailRepository;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Recipient) ||
            string.IsNullOrWhiteSpace(request.Subject) ||
            string.IsNullOrWhiteSpace(request.Body))
        {
            return BadRequest(new { error = "All fields (Recipient, Subject, Body) are required." });
        }

        try
        {
            await _emailRepository.SendEmailAsync(request.Recipient, request.Subject, request.Body);
            return Ok(new { message = "Email sent successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An internal server error occurred.", details = ex.Message });
        }
    }

    [HttpGet("hello")]
    public IActionResult GetHello()
    {
        return Ok(new { message = "Hello!" });
    }

}

public class EmailRequest
{
    public string Recipient { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}
