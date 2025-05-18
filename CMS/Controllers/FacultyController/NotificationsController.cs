using CMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers.FacultyController
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly CmsproContext _context;
        private readonly IConfiguration _configuration;


        public NotificationsController(CmsproContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("unpaidStudentsByDept/{deptId}")]
        public async Task<IActionResult> GetUnpaidStudentsByDept(int deptId)
        {
            var students = await _context.Students
                .Where(s => s.DeptId == deptId)
                .Where(s => !_context.StudentFees.Any(f => f.StudentId == s.StudentId))
                .Select(s => new { s.StudentId, s.StudentName })
                .ToListAsync();

            return Ok(students);
        }

        [HttpPost("sendNotificationToUnpaid/{deptId}")]
        public async Task<IActionResult> SendNotificationToUnpaid(int deptId)
        {
            var unpaidStudents = await _context.Students
                .Where(s => s.DeptId == deptId)
                .Where(s => !_context.StudentFees.Any(f => f.StudentId == s.StudentId))
                .ToListAsync();

            var newNotifications = new List<Notification>();

            foreach (var student in unpaidStudents)
            {
                bool alreadyNotified = await _context.Notifications
                    .AnyAsync(n => n.StudentId == student.StudentId &&
                                   n.Message.Contains("unpaid fees") &&
                                   n.SentDate.Date == DateTime.Now.Date);

                if (!alreadyNotified)
                {
                    newNotifications.Add(new Notification
                    {
                        StudentId = student.StudentId,
                        Message = "You have unpaid fees. Please complete your payment.",
                        SentDate = DateTime.Now,
                        IsRead = false
                    });
                }
            }

            if (newNotifications.Any())
            {
                await _context.Notifications.AddRangeAsync(newNotifications);
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true, message = "Notifications sent individually." });
        }


        [HttpGet("notifications/{studentId}")]
        public async Task<IActionResult> GetNotifications(int studentId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.StudentId == studentId && !n.IsRead)
                .OrderByDescending(n => n.SentDate)
                .ToListAsync();

            return Ok(notifications);
        }

        [HttpPost("markAllAsRead/{studentId}")]
        public async Task<IActionResult> MarkAllAsRead(int studentId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.StudentId == studentId && !n.IsRead)
                .ToListAsync();

            if (!notifications.Any())
                return Ok(new { success = true, message = "No unread notifications." });

            foreach (var note in notifications)
            {
                note.IsRead = true;
            }

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "All notifications marked as read." });
        }


    }
}
