using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/reports")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IUserService _userService;
        private readonly ITournamentService _tournamentService;
        private readonly ITeamService _teamService;
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;

        public ReportController(IReportService reportService, IUserService userService, ITournamentService tournamentService, ITeamService teamService, ICommentService commentService, IMapper mapper)
        {
            _reportService = reportService;
            _userService = userService;
            _tournamentService = tournamentService;
            _teamService = teamService;
            _commentService = commentService;
            _mapper = mapper;
        }

        /// <summary>Get list report</summary>
        /// <returns>List report</returns>
        /// <response code="200">Returns list report</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<ReportListVM> GetListReport(
            [FromQuery(Name = "reason")] string? reason,
            [FromQuery(Name = "user-id")] int? userId,
            [FromQuery(Name = "comment-id")] int? commentId,
            [FromQuery(Name = "team-id")] int? teamId,
            [FromQuery(Name = "tournament-id")] int? tourId,
            [FromQuery(Name = "order-by")] ReportFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<Report> reportList = _reportService.GetList();
                if (!String.IsNullOrEmpty(reason))
                {
                    reportList = reportList.Where(s => s.Reason.ToUpper().Contains(reason.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(userId.ToString()))
                {
                    reportList = reportList.Where(s => s.UserId == userId);
                }
                if (!String.IsNullOrEmpty(commentId.ToString()))
                {
                    reportList = reportList.Where(s => s.CommentId == commentId);
                }
                if (!String.IsNullOrEmpty(teamId.ToString()))
                {
                    reportList = reportList.Where(s => s.TeamId == teamId);
                }
                if (!String.IsNullOrEmpty(tourId.ToString()))
                {
                    reportList = reportList.Where(s => s.TournamentId == tourId);
                }

                if (orderBy == ReportFieldEnum.Reason)
                {
                    reportList = reportList.OrderBy(rp => rp.Reason);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        reportList = reportList.OrderByDescending(rp => rp.Reason);
                    }
                }
                else if (orderBy == ReportFieldEnum.DateReport)
                {
                    reportList = reportList.OrderBy(rp => rp.DateReport);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        reportList = reportList.OrderByDescending(rp => rp.DateReport);
                    }
                }
                else if (orderBy == ReportFieldEnum.UserId)
                {
                    reportList = reportList.OrderBy(rp => rp.UserId);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        reportList = reportList.OrderByDescending(rp => rp.UserId);
                    }
                }
                else if (orderBy == ReportFieldEnum.CommentId)
                {
                    reportList = reportList.OrderBy(rp => rp.CommentId);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        reportList = reportList.OrderByDescending(rp => rp.CommentId);
                    }
                }
                else if (orderBy == ReportFieldEnum.TeamId)
                {
                    reportList = reportList.OrderBy(rp => rp.TeamId);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        reportList = reportList.OrderByDescending(rp => rp.TeamId);
                    }
                }
                else if (orderBy == ReportFieldEnum.TournamentId)
                {
                    reportList = reportList.OrderBy(rp => rp.TournamentId);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        reportList = reportList.OrderByDescending(rp => rp.TournamentId);
                    }
                }
                else
                {
                    reportList = reportList.OrderBy(rp => rp.Id);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        reportList = reportList.OrderByDescending(rp => rp.Id);
                    }
                }

                int countList = reportList.Count();

                List<Report> reportListPaging = reportList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                ReportListVM reportListResponse = new ReportListVM
                {
                    Reports = _mapper.Map<List<ReportVM>>(reportListPaging),
                    CountList = countList,
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(reportListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get report by id</summary>
        /// <returns>Return the report with the corresponding id</returns>
        /// <response code="200">Returns the report with the specified id</response>
        /// <response code="404">No report found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<ReportVM>> GetReportById(int id)
        {
            try
            {
                Report currentReport = await _reportService.GetByIdAsync(id);
                if (currentReport != null)
                {
                    return Ok(_mapper.Map<ReportVM>(currentReport));
                }
                return NotFound("Không thể tìm thấy báo cáo với id là " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new report</summary>
        /// <response code="201">Created new report successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<ReportVM>> CreateReport([FromBody] ReportCM model)
        {
            try
            {
                Report report = new Report();

                User user = await _userService.GetByIdAsync(model.UserId);
                if(user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }

                if (!String.IsNullOrEmpty(model.CommentId.ToString()) && model.CommentId != 0)
                {
                    Comment comment = await _commentService.GetByIdAsync(model.CommentId.Value);
                    if(comment == null)
                    {
                        return NotFound("Không tìm thấy bình luận");
                    }
                    else
                    {
                        report.CommentId = comment.Id;
                    }
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
                        report.TeamId = team.Id;
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
                        report.TournamentId = tournament.Id;
                    }
                }
                report.UserId = user.Id;
                report.Reason = String.IsNullOrEmpty(model.Reason) ? "" : model.Reason;
                report.DateReport = DateTime.Now.AddHours(7);
                Report reportCreated = await _reportService.AddAsync(report);
                if (reportCreated != null)
                {
                    return CreatedAtAction("GetReportById", new { id = reportCreated.Id }, _mapper.Map<ReportVM>(reportCreated));
                }
                return BadRequest("Tạo báo cáo mới thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a report</summary>
        /// <response code="200">Update report successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<ReportVM>> UpdateReport([FromBody] ReportUM model)
        {
            try
            {
                Report report = await _reportService.GetByIdAsync(model.Id);
                if(report == null)
                {
                    return NotFound("Không tìm thấy báo cáo");
                }

                report.Reason = String.IsNullOrEmpty(model.Reason) ? report.Reason : model.Reason;
                if (!String.IsNullOrEmpty(model.UserId.ToString()) && model.UserId != 0)
                {
                    User user = await _userService.GetByIdAsync(model.UserId.Value);
                    if (user != null)
                    {
                        report.UserId = user.Id;
                    }
                }
                if (!String.IsNullOrEmpty(model.CommentId.ToString()) && model.CommentId != 0)
                {
                    Comment comment = await _commentService.GetByIdAsync(model.CommentId.Value);
                    if (comment != null)
                    {
                        report.CommentId = comment.Id;
                    }
                }
                if (!String.IsNullOrEmpty(model.TeamId.ToString()) && model.TeamId != 0)
                {
                    Team team = await _teamService.GetByIdAsync(model.TeamId.Value);
                    if (team != null)
                    {
                        report.TeamId = team.Id;
                    }
                }
                if (!String.IsNullOrEmpty(model.TournamentId.ToString()) && model.TournamentId != 0)
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync(model.TournamentId.Value);
                    if (tournament != null)
                    {
                        report.TournamentId = tournament.Id;
                    }
                }

                bool isUpdated = await _reportService.UpdateAsync(report);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<ReportVM>(report));
                }
                return BadRequest("Cập nhật báo cáo thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete report By Id</summary>
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
                Report currentReport = await _reportService.GetByIdAsync(id);
                if (currentReport == null)
                {
                    return NotFound("Không thể tìm thấy vai trò với id là " + id);
                }

                bool isDeleted = await _reportService.DeleteAsync(currentReport);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Xóa báo cáo thành công"
                    });
                }
                return BadRequest("Xóa báo cáo thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
