using CMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers.StudentController
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly CmsproContext _context;
        private readonly IConfiguration _configuration;

        public FeedbackController(CmsproContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpGet("GetFeedBack")]
        public async Task<IActionResult> GetFeedBack()
        {
            var feedbacks = await _context.Feedbacks.Select(feedback => new Feedback
            {
                FeedbackId = feedback.FeedbackId,
                Name = feedback.Name,
                Email = feedback.Email,
                Message = feedback.Message,
                Timestamp = feedback.Timestamp,
            }).ToListAsync();

            return Ok(new
            {
                success = true,
                message = "FeedBack fetch successfully.",
                Feedback = feedbacks,
            });
        }


        [HttpPost("AddFeedback")]
        public async Task<IActionResult> AddFeedback([FromBody] Feedback feedback)
        {

            var addfeedback = new Feedback
            {
                Name = feedback.Name,
                Email = feedback.Email,
                Message = feedback.Message,
                Timestamp = DateTime.Now,
            };

            _context.Feedbacks.Add(addfeedback);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                message = "FeedBack Added successfully.",
                Feedback = feedback,
            });
        }

        [HttpPost("UpdateFeedBack/{id}")]
        public async Task<IActionResult> UpdateFeedBack(int id, [FromBody] Feedback feedback)
        {
            var updatefeedback = await _context.Feedbacks.FindAsync(id);
            if (updatefeedback == null)
            {
                return NotFound($"Feedback with Id {id} Not Found");
            }
            updatefeedback.Name = feedback.Name;
            updatefeedback.Email = feedback.Email;
            updatefeedback.Message = feedback.Message;
            updatefeedback.Timestamp = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Error Updating Department: {ex.Message}");
            }
        }

        [HttpDelete("deleteFeedback")]
        public async Task<IActionResult> deleteFeedback(int id)
        {
            var deletefeedback = await _context.Feedbacks.FindAsync(id);
            if (deletefeedback == null)
            {
                return NotFound($"Feedback with Id {id} Not Found");
            }
            try
            {
                _context.Feedbacks.Remove(deletefeedback);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Feedback deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error Deleting Department: {ex.Message}");
            }
        }

    }
}
