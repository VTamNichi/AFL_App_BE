using AmateurFootballLeague.ExternalService;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/notifications")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        private readonly ITournamentService _tournamentService;
        private readonly ITeamService _teamService;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private readonly IPushNotificationService _pushNotificationService;

        public NotificationController(INotificationService notificationService, IUserService userService, ITournamentService tournamentService, ITeamService teamService, IMapper mapper, IRedisService redisService, IPushNotificationService pushNotificationService)
        {
            _notificationService = notificationService;
            _userService = userService;
            _tournamentService = tournamentService;
            _teamService = teamService;
            _mapper = mapper;
            _redisService = redisService;
            _pushNotificationService = pushNotificationService;
        }

        /// <summary>Get list notification</summary>
        /// <returns>List notification</returns>
        /// <response code="200">Returns list notification</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<NotificationListVM> GetListNotification(
            [FromQuery(Name = "content")] string? content,
            [FromQuery(Name = "is-seen")] bool? isSeen,
            [FromQuery(Name = "is-active")] bool? isActive,
            [FromQuery(Name = "user-id")] int? userId,
            [FromQuery(Name = "tournament-id")] int? tourId,
            [FromQuery(Name = "team-id")] int? teamId,
            [FromQuery(Name = "order-by")] NotificationFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<Notification> notificationList = _notificationService.GetList();
                if (!String.IsNullOrEmpty(content))
                {
                    notificationList = notificationList.Where(s => s.Content.ToUpper().Contains(content.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(isSeen.ToString()))
                {
                    notificationList = notificationList.Where(s => s.IsSeen == isSeen);
                }
                if (!String.IsNullOrEmpty(isActive.ToString()))
                {
                    notificationList = notificationList.Where(s => s.IsActive == isActive);
                }
                if (!String.IsNullOrEmpty(userId.ToString()))
                {
                    notificationList = notificationList.Where(s => s.UserId == userId);
                }
                if (!String.IsNullOrEmpty(teamId.ToString()))
                {
                    notificationList = notificationList.Where(s => s.TeamId == teamId);
                }
                if (!String.IsNullOrEmpty(tourId.ToString()))
                {
                    notificationList = notificationList.Where(s => s.TournamentId == tourId);
                }

                if (orderBy == NotificationFieldEnum.Content)
                {
                    notificationList = notificationList.OrderBy(rp => rp.Content);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        notificationList = notificationList.OrderByDescending(rp => rp.Content);
                    }
                }
                else if (orderBy == NotificationFieldEnum.DateCreate)
                {
                    notificationList = notificationList.OrderBy(rp => rp.DateCreate);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        notificationList = notificationList.OrderByDescending(rp => rp.DateCreate);
                    }
                }
                else if (orderBy == NotificationFieldEnum.UserId)
                {
                    notificationList = notificationList.OrderBy(rp => rp.UserId);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        notificationList = notificationList.OrderByDescending(rp => rp.UserId);
                    }
                }
                else if (orderBy == NotificationFieldEnum.TeamId)
                {
                    notificationList = notificationList.OrderBy(rp => rp.TeamId);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        notificationList = notificationList.OrderByDescending(rp => rp.TeamId);
                    }
                }
                else if (orderBy == NotificationFieldEnum.TournamentId)
                {
                    notificationList = notificationList.OrderBy(rp => rp.TournamentId);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        notificationList = notificationList.OrderByDescending(rp => rp.TournamentId);
                    }
                }
                else
                {
                    notificationList = notificationList.OrderBy(rp => rp.Id);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        notificationList = notificationList.OrderByDescending(rp => rp.Id);
                    }
                }

                int countList = notificationList.Count();

                List<Notification> notificationListPaging = notificationList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                NotificationListVM notificationListResponse = new NotificationListVM
                {
                    Notifications = _mapper.Map<List<NotificationVM>>(notificationListPaging),
                    CountList = countList,
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(notificationListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get notification by id</summary>
        /// <returns>Return the notification with the corresponding id</returns>
        /// <response code="200">Returns the notification with the specified id</response>
        /// <response code="404">No notification found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<NotificationVM>> GetNotificationById(int id)
        {
            try
            {
                Notification currentNotification = await _notificationService.GetByIdAsync(id);
                if (currentNotification != null)
                {
                    return Ok(_mapper.Map<NotificationVM>(currentNotification));
                }
                return NotFound("Không thể tìm thấy thông báo với id là " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new notification</summary>
        /// <response code="201">Created new notification successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<NotificationVM>> CreateNotification([FromBody] NotificationCM model)
        {
            try
            {
                Notification notification = new Notification();

                User user = await _userService.GetByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }

                if (!String.IsNullOrEmpty(model.TeamId.ToString()) && model.TeamId != 0)
                {
                    Team team = await _teamService.GetByIdAsync(model.TeamId.Value);
                    if (team == null)
                    {
                        return NotFound("Không tìm thấy đội bóng");
                    }
                    else
                    {
                        notification.TeamId = team.Id;
                    }
                }
                if (!String.IsNullOrEmpty(model.TournamentId.ToString()) && model.TournamentId != 0)
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync(model.TournamentId.Value);
                    if (tournament == null)
                    {
                        return NotFound("Không tìm thấy giải đấu");
                    }
                    else
                    {
                        notification.TournamentId = tournament.Id;
                    }
                }
                notification.UserId = user.Id;
                notification.Content = String.IsNullOrEmpty(model.Content) ? "" : model.Content;
                notification.IsActive = true;
                notification.IsSeen = false;
                notification.DateCreate = DateTime.Now.AddHours(7);

                Notification notificationCreated = await _notificationService.AddAsync(notification);
                if (notificationCreated != null)
                {
                    await _pushNotificationService.SendMessage("Bạn đã nhận được thông báo mới", notification.Content, user.Email, null);
                    return CreatedAtAction("GetNotificationById", new { id = notificationCreated.Id }, _mapper.Map<NotificationVM>(notificationCreated));
                }
                return BadRequest("Tạo thông báo mới thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a notification</summary>
        /// <response code="200">Update notification successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<NotificationVM>> UpdateNotification([FromBody] NotificationUM model)
        {
            try
            {
                Notification notification = await _notificationService.GetByIdAsync(model.Id);
                if (notification == null)
                {
                    return NotFound("Không tìm thấy báo cáo");
                }

                notification.Content = String.IsNullOrEmpty(model.Content) ? notification.Content : model.Content;
                if (!String.IsNullOrEmpty(model.UserId.ToString()) && model.UserId != 0)
                {
                    User user = await _userService.GetByIdAsync(model.UserId.Value);
                    if (user != null)
                    {
                        notification.UserId = user.Id;
                    }
                }
                if (!String.IsNullOrEmpty(model.TeamId.ToString()) && model.TeamId != 0)
                {
                    Team team = await _teamService.GetByIdAsync(model.TeamId.Value);
                    if (team != null)
                    {
                        notification.TeamId = team.Id;
                    }
                }
                if (!String.IsNullOrEmpty(model.TournamentId.ToString()) && model.TournamentId != 0)
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync(model.TournamentId.Value);
                    if (tournament != null)
                    {
                        notification.TournamentId = tournament.Id;
                    }
                }
                notification.IsActive = String.IsNullOrEmpty(model.IsActive.ToString()) ? notification.IsActive : model.IsActive;
                notification.IsSeen = String.IsNullOrEmpty(model.IsSeen.ToString()) ? notification.IsSeen : model.IsSeen;

                bool isUpdated = await _notificationService.UpdateAsync(notification);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<NotificationVM>(notification));
                }
                return BadRequest("Cập nhật báo cáo thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("connection")]
        [Produces("application/json")]
        [AllowAnonymous]
        public async Task<ActionResult> MakeConnection([FromBody] NotificationConnection model)
        {
            try
            {
                bool isSuccess = await _redisService.Set("user:" + model.Email, model.Token, 1440);
                if (isSuccess)
                {
                    return Ok(new
                    {
                        message = "Success"
                    });
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete notification By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            try
            {
                Notification currentNotification = await _notificationService.GetByIdAsync(id);
                if (currentNotification == null)
                {
                    return NotFound("Không thể tìm thấy vai trò với id là " + id);
                }

                bool isDeleted = await _notificationService.DeleteAsync(currentNotification);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Xóa thông báo thành công"
                    });
                }
                return BadRequest("Xóa thông báo thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
