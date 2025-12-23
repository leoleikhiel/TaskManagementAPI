using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Services;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // All endpoints require authentication
    public class CalendarController : ControllerBase
    {
        private readonly IGoogleCalendarService _calendarService;
        private readonly ILogger<CalendarController> _logger;

        public CalendarController(
            IGoogleCalendarService calendarService,
            ILogger<CalendarController> logger)
        {
            _calendarService = calendarService;
            _logger = logger;
        }

        // Helper method to get current user ID from JWT
        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        }

        // GET: api/calendar/auth-url
        /// <summary>
        /// Get Google OAuth authorization URL
        /// </summary>
        [HttpGet("auth-url")]
        public async Task<ActionResult<CalendarAuthUrlResponse>> GetAuthUrl()
        {
            try
            {
                var userId = GetUserId();
                var authUrl = await _calendarService.GetAuthorizationUrlAsync(userId);

                return Ok(new CalendarAuthUrlResponse { AuthUrl = authUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate auth URL");
                return StatusCode(500, new { message = "Failed to generate authorization URL" });
            }
        }

        // POST: api/calendar/connect
        /// <summary>
        /// Complete OAuth flow by exchanging authorization code for tokens
        /// </summary>
        [HttpPost("connect")]
        public async Task<ActionResult> ConnectCalendar([FromBody] CalendarAuthRequest request)
        {
            try
            {
                var userId = GetUserId();
                var success = await _calendarService.HandleOAuthCallbackAsync(userId, request.AuthorizationCode);

                if (success)
                {
                    return Ok(new { message = "Calendar connected successfully" });
                }

                return BadRequest(new { message = "Failed to connect calendar" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect calendar");
                return StatusCode(500, new { message = "Failed to connect calendar" });
            }
        }

        // DELETE: api/calendar/disconnect
        /// <summary>
        /// Disconnect Google Calendar
        /// </summary>
        [HttpDelete("disconnect")]
        public async Task<ActionResult> DisconnectCalendar()
        {
            try
            {
                var userId = GetUserId();
                var success = await _calendarService.DisconnectCalendarAsync(userId);

                if (success)
                {
                    return Ok(new { message = "Calendar disconnected successfully" });
                }

                return NotFound(new { message = "No calendar connection found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to disconnect calendar");
                return StatusCode(500, new { message = "Failed to disconnect calendar" });
            }
        }

        // GET: api/calendar/status
        /// <summary>
        /// Get calendar sync status
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult<CalendarSyncStatusResponse>> GetStatus()
        {
            try
            {
                var userId = GetUserId();
                var status = await _calendarService.GetSyncStatusAsync(userId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get sync status");
                return StatusCode(500, new { message = "Failed to get sync status" });
            }
        }

        // POST: api/calendar/sync/task/{taskId}
        /// <summary>
        /// Sync a specific task to Google Calendar
        /// </summary>
        [HttpPost("sync/task/{taskId}")]
        public async Task<ActionResult<TaskSyncResponse>> SyncTask(int taskId)
        {
            try
            {
                var userId = GetUserId();
                var response = await _calendarService.SyncTaskToCalendarAsync(userId, taskId);

                if (response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to sync task {taskId}");
                return StatusCode(500, new { message = "Failed to sync task" });
            }
        }

        // POST: api/calendar/sync/all
        /// <summary>
        /// Sync all tasks to Google Calendar
        /// </summary>
        [HttpPost("sync/all")]
        public async Task<ActionResult<List<TaskSyncResponse>>> SyncAllTasks()
        {
            try
            {
                var userId = GetUserId();
                var responses = await _calendarService.SyncAllTasksAsync(userId);

                var summary = new
                {
                    total = responses.Count,
                    successful = responses.Count(r => r.Success),
                    failed = responses.Count(r => !r.Success),
                    results = responses
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync all tasks");
                return StatusCode(500, new { message = "Failed to sync all tasks" });
            }
        }

        // DELETE: api/calendar/sync/task/{taskId}
        /// <summary>
        /// Delete calendar event for a task
        /// </summary>
        [HttpDelete("sync/task/{taskId}")]
        public async Task<ActionResult> DeleteCalendarEvent(int taskId)
        {
            try
            {
                var userId = GetUserId();
                var success = await _calendarService.DeleteCalendarEventAsync(userId, taskId);

                if (success)
                {
                    return Ok(new { message = "Calendar event deleted successfully" });
                }

                return NotFound(new { message = "Calendar event not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete calendar event for task {taskId}");
                return StatusCode(500, new { message = "Failed to delete calendar event" });
            }
        }

        // GET: api/calendar/oauth/callback
        /// <summary>
        /// OAuth callback - receives authorization code from Google
        /// </summary>
        [HttpGet("oauth/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> OAuthCallback([FromQuery] string code, [FromQuery] string? state, [FromQuery] string? error)
        {
            try
            {
                if (!string.IsNullOrEmpty(error))
                {
                    _logger.LogWarning($"OAuth error: {error}");
                    return Content($@"
                        <html>
                            <head><title>Authorization Failed</title></head>
                            <body style='font-family: Arial; text-align: center; padding-top: 50px;'>
                                <h2>❌ Authorization Failed</h2>
                                <p>{error}</p>
                                <p>You can close this window and try again.</p>
                                <script>setTimeout(() => window.close(), 3000);</script>
                            </body>
                        </html>
                    ", "text/html");
                }

                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest("No authorization code received");
                }

                return Content($@"
                                    <html>
                                    <head><title>Authorization Successful</title></head>
                                    <body style='font-family: Arial; text-align: center; padding-top: 50px;'>
                                        <h2>✅ Authorization Successful!</h2>
                                        <p>Connecting your calendar...</p>
                                        <script>
                                            // Store code in window.opener (parent window)
                                            if (window.opener) {{
                                                window.opener.postMessage({{ 
                                                    type: 'GOOGLE_OAUTH_SUCCESS', 
                                                    code: '{code}' 
                                                }}, '*');
                                            }}
                    
                                            // Close popup after 2 seconds
                                            setTimeout(() => window.close(), 2000);
                                        </script>
                                    </body>
                                    </html>
                                ", "text/html");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OAuth callback failed");
                return Content($@"
                                <html>
                                <head><title>Error</title></head>
                                <body style='font-family: Arial; text-align: center; padding-top: 50px;'>
                                    <h2>❌ Connection Failed</h2>
                                    <p>An error occurred. Please try again.</p>
                                    <p style='color: gray; font-size: 12px;'>{ex.Message}</p>
                                    <script>setTimeout(() => window.close(), 5000);</script>
                                </body>
                                </html>
                            ", "text/html");
            }
        }
    }
}