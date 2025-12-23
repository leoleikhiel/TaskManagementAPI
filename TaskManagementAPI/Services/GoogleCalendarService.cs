using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Data;
using Google.Apis.Auth.OAuth2.Responses;
using TaskManagementAPI.DTOs;
using TaskManagementAPI.Models;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Calendar.v3.Data;

namespace TaskManagementAPI.Services
{
    public class GoogleCalendarService : IGoogleCalendarService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GoogleCalendarService> _logger;

        public GoogleCalendarService(ApplicationDbContext context, IConfiguration configuration, ILogger<GoogleCalendarService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> HasValidTokenAsync(int userId)
        {
            var token = await _context.GoogleCalendarTokens
                .FirstOrDefaultAsync(t => t.UserId == userId);
            
            if (token == null) return false;

            if (token.IsExpired)
            {
                await RefreshAccessTokenAsync(userId);
                token = await _context.GoogleCalendarTokens
                    .FirstOrDefaultAsync(t => t.UserId == userId);
            }

            return token != null && !token.IsExpired;
        }

        public async Task<bool> RefreshAccessTokenAsync(int userId)
        {
            var token = await _context.GoogleCalendarTokens
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (token == null || string.IsNullOrEmpty(token.RefreshToken))
            {
                _logger.LogWarning($"No refresh token available for user {userId}");
                return false;
            }

            try
            {
                var flow = CreateAuthorizationFlow();

                var newToken = await flow.RefreshTokenAsync(
                    userId.ToString(),
                    token.RefreshToken,
                    CancellationToken.None
                );

                token.AccessToken = newToken.AccessToken;

                var expiresIn = (newToken.ExpiresInSeconds ?? 3600) - 30;
                token.TokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn);
                token.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully refreshed token for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to refresh token for user {userId}");
                return false;
            }
        }

        public async Task<string> GetAuthorizationUrlAsync(int userId)
        {
            var flow = CreateAuthorizationFlow();
            var authUrl = flow.CreateAuthorizationCodeRequest(
                _configuration["Google:RedirectUri"]!
            ).Build();
            return authUrl.ToString();
        }

        public async Task<bool> HandleOAuthCallbackAsync(int userId, string authorizationCode)
        {
            try
            {
                var flow = CreateAuthorizationFlow();

                var tokenResponse = await flow.ExchangeCodeForTokenAsync(
                    userId.ToString(),
                    authorizationCode,
                    _configuration["Google:RedirectUri"]!,
                    CancellationToken.None
                );

                var existingToken = await _context.GoogleCalendarTokens
                    .FirstOrDefaultAsync(t => t.UserId == userId);

                var expiresIn = (tokenResponse.ExpiresInSeconds ?? 3600) - 30;

                if (existingToken != null)
                {
                    existingToken.AccessToken = tokenResponse.AccessToken;
                    existingToken.RefreshToken = tokenResponse.RefreshToken ?? existingToken.RefreshToken;
                    existingToken.TokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn);
                    existingToken.UpdatedAt = DateTime.UtcNow;

                    _logger.LogInformation($"Updated calendar token for user {userId}");
                } 
                else
                {
                    var newToken = new GoogleCalendarToken
                    {
                        UserId = userId,
                        AccessToken = tokenResponse.AccessToken,
                        RefreshToken = tokenResponse.RefreshToken ?? string.Empty,
                        TokenExpiry = DateTime.UtcNow.AddSeconds(expiresIn),
                        CalendarId = "primary"
                    };

                    _context.GoogleCalendarTokens.Add(newToken);

                    _logger.LogInformation($"Created calendar token for user {userId}");
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to handle OAuth callback for user {userId}");
                return false;
            }
        }

        public async Task<TaskSyncResponse> SyncTaskToCalendarAsync(int userId, int taskId)
        {
            try
            {
                if (!await HasValidTokenAsync(userId))
                {
                    return new TaskSyncResponse
                    {
                        TaskId = taskId,
                        Success = false,
                        Message = "Calendar not connected or token expired"
                    };
                }

                var task = await _context.Tasks
                    .Include(t => t.CalendarSync)
                    .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

                if (task == null)
                {
                    return new TaskSyncResponse
                    {
                        TaskId = taskId,
                        Success = false,
                        Message = "Task not found"
                    };
                }

                if (task.DueDate == null && task.ScheduledDate == null)
                {
                    return new TaskSyncResponse
                    {
                        TaskId = taskId,
                        Success = false,
                        Message = "Task has no due date or scheduled date"
                    };
                }

                var service = await GetCalendarServiceAsync(userId);
                var calendarToken = await _context.GoogleCalendarTokens.FirstAsync(t => t.UserId == userId);

                Event calendarEvent;
                if (task.IsSyncedToCalendar && !string.IsNullOrEmpty(task.GoogleEventId))
                {
                    calendarEvent = await UpdateEventAsync(service, task, calendarToken.CalendarId ?? "primary");
                } 
                else
                {
                    calendarEvent = await CreateEventAsync(service, task, calendarToken.CalendarId ?? "primary");
                }

                task.IsSyncedToCalendar = true;
                task.GoogleEventId = calendarEvent.Id;
                task.LastCalendarSync = DateTime.UtcNow;

                if (task.CalendarSync == null)
                {
                    task.CalendarSync = new TaskCalendarSync
                    {
                        TaskId = taskId,
                        GoogleEventId = calendarEvent.Id,
                        UserId = userId,
                        SyncStatus = SyncStatus.Synced
                    };
                }
                else
                {
                    task.CalendarSync.GoogleEventId = calendarEvent.Id;
                    task.CalendarSync.LastSyncedAt = DateTime.UtcNow;
                    task.CalendarSync.SyncStatus = SyncStatus.Synced;
                    task.CalendarSync.ErrorMessage = null;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully synced task {taskId} to calendar event {calendarEvent.Id}");

                return new TaskSyncResponse
                {
                    TaskId = taskId,
                    GoogleEventId = calendarEvent.Id,
                    Success = true,
                    Message = "Task synced successfully",
                    LastSynced = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to sync task {taskId} for user {userId}");

                var task = await _context.Tasks
                    .Include(t => t.CalendarSync)
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                if (task?.CalendarSync != null)
                {
                    task.CalendarSync.SyncStatus = SyncStatus.Failed;
                    task.CalendarSync.ErrorMessage = ex.Message;
                    await _context.SaveChangesAsync();
                }

                return new TaskSyncResponse
                {
                    TaskId = taskId,
                    Success = false,
                    Message = $"Sync failed: {ex.Message}"
                };
            }
        }

        public async Task<bool> DeleteCalendarEventAsync(int userId, int taskId)
        {
            try
            {
                var task = await _context.Tasks
                    .Include(t => t.CalendarSync)
                    .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

                if (task == null || !task.IsSyncedToCalendar || string.IsNullOrEmpty(task.GoogleEventId))
                {
                    return false;
                }

                var service = await GetCalendarServiceAsync(userId);
                var calendarToken = await _context.GoogleCalendarTokens.FirstAsync(t => t.UserId == userId);

                await service.Events.Delete(calendarToken.CalendarId ?? "primary", task.GoogleEventId).ExecuteAsync();

                task.IsSyncedToCalendar = false;
                task.GoogleEventId = string.Empty;
                task.LastCalendarSync = null;

                if (task.CalendarSync != null)
                {
                    _context.TaskCalendarSyncs.Remove(task.CalendarSync);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Deleted calendar event for task {taskId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete calendar event for task {taskId}");
                return false;
            }
        }

        public async Task<List<TaskSyncResponse>> SyncAllTasksAsync(int userId)
        {
            var tasks = await _context.Tasks
                .Where(t => t.UserId == userId && !t.IsCompleted)
                .Where(t => t.DueDate != null || t.ScheduledDate != null)
                .ToListAsync();

            var responses = new List<TaskSyncResponse>();

            foreach (var task in tasks)
            {
                var response = await SyncTaskToCalendarAsync(userId, task.Id);
                responses.Add(response);
            }

            _logger.LogInformation($"Synced {responses.Count(r => r.Success)}/{tasks.Count}");

            return responses;
        }

        public async Task<CalendarSyncStatusResponse> GetSyncStatusAsync(int userId)
        {
            var hasToken = await HasValidTokenAsync(userId);

            var syncRecords = await _context.TaskCalendarSyncs
                .Where(s => s.UserId == userId)
                .ToListAsync();

            return new CalendarSyncStatusResponse
            {
                IsConnected = hasToken,
                LastSync = syncRecords.Any() ? syncRecords.Max(s => s.LastSyncedAt) : null,
                SyncedTasksCount = syncRecords.Count(s => s.SyncStatus == SyncStatus.Synced),
                PendingTasksCount = syncRecords.Count(s => s.SyncStatus == SyncStatus.Pending),
                FailedTasksCount = syncRecords.Count(s => s.SyncStatus == SyncStatus.Failed)
            };
        }

        public async Task<bool> DisconnectCalendarAsync(int userId)
        {
            var token = await _context.GoogleCalendarTokens
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (token == null)
            {
                return false;
            }

            _context.GoogleCalendarTokens.Remove(token);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Disconnected calendar for user {userId}");
            return true;
        }

        public async Task<TaskSyncResponse> UpdateCalendarEventAsync(int userId, int taskId)
        {
            return await SyncTaskToCalendarAsync(userId, taskId);
        }

        private GoogleAuthorizationCodeFlow CreateAuthorizationFlow()
        {
            return new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = _configuration["Google:ClientId"]!,
                        ClientSecret = _configuration["Google:ClientSecret"]!
                    },
                    Scopes = _configuration.GetSection("Google:Scopes").Get<string[]>()!
                }
            );
        }

        private async Task<CalendarService> GetCalendarServiceAsync(int userId)
        {
            var token = await _context.GoogleCalendarTokens
                .FirstOrDefaultAsync(t => t.UserId == userId);

            if (token == null)
            {
                throw new InvalidOperationException("User has no calendar token");
            }

            var credential = new UserCredential(
                CreateAuthorizationFlow(),
                userId.ToString(),
                new TokenResponse
                {
                    AccessToken = token.AccessToken,
                    RefreshToken = token.RefreshToken,
                    ExpiresInSeconds = (long)(token.TokenExpiry -  DateTime.UtcNow).TotalSeconds
                }
            );

            return new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Task Management API"
            });
        } 

        private async Task<Event> CreateEventAsync(CalendarService service, TaskItem task, string calendarId)
        {
            var eventDate = task.ScheduledDate ?? task.DueDate;

            if (!eventDate.HasValue)
            {
                throw new InvalidOperationException("Task must have a ScheduledDate or DueDate");
            }

            var timeZone = _configuration["Calendar:TimeZone"] ?? "UTC";

            bool isAllDay = eventDate.Value.TimeOfDay == TimeSpan.Zero;

            var calendarEvent = new Event
            {
                Summary = task.Title ?? "Untitled Task",
                Description = task.Description
            };

            if (isAllDay)
            {
                calendarEvent.Start = new EventDateTime
                {
                    Date = eventDate.Value.ToString("yyyy-MM-dd"),
                    TimeZone = timeZone
                };

                calendarEvent.End = new EventDateTime
                {
                    Date = eventDate.Value.AddDays(1).ToString("yyyy-MM-dd"),
                    TimeZone = timeZone
                };
            }
            else
            {
                calendarEvent.Start = new EventDateTime
                {
                    DateTimeDateTimeOffset = eventDate.Value,
                    TimeZone = timeZone
                };

                calendarEvent.End = new EventDateTime
                {
                    DateTimeDateTimeOffset = eventDate.Value.AddHours(1),
                    TimeZone = timeZone
                };
            }

            calendarEvent.Reminders = new Event.RemindersData
            {
                UseDefault = true
            };

            var request = service.Events.Insert(calendarEvent, calendarId);
            var createdEvent = await request.ExecuteAsync();

            _logger.LogInformation($"Created calendar event {createdEvent.Id} for task {task.Id}");

            return createdEvent;
        }

        private async Task<Event> UpdateEventAsync(CalendarService service, TaskItem task, string calendarId)
        {
            var existingEvent = await service.Events
                .Get(calendarId, task.GoogleEventId!)
                .ExecuteAsync();

            var eventStartTime = task.ScheduledDate ?? task.DueDate!.Value;
            var eventEndTime = eventStartTime.AddHours(1);

            existingEvent.Summary = task.Title;
            existingEvent.Description = task.Description;
            existingEvent.Start = new EventDateTime
            {
                DateTimeDateTimeOffset = eventStartTime,
                TimeZone = _configuration["Calendar:TimeZone"] ?? "UTC"
            };
            existingEvent.End = new EventDateTime
            {
                DateTimeDateTimeOffset = eventEndTime,
                TimeZone = _configuration["Calendar:TimeZone"] ?? "UTC"
            };

            var request = service.Events.Update(existingEvent, calendarId, task.GoogleEventId!);
            var updatedEvent = await request.ExecuteAsync();

            _logger.LogInformation($"Updated calendar event {updatedEvent.Id} for task {task.Id}");

            return updatedEvent;
        }
    }
}
