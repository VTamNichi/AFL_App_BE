using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
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
        private readonly IFootballPlayerService _footballPlayerService;
        private readonly IMapper _mapper;

        public ReportController(IReportService reportService, IUserService userService, ITournamentService tournamentService, ITeamService teamService, IFootballPlayerService footballPlayerService, IMapper mapper)
        {
            _reportService = reportService;
            _userService = userService;
            _tournamentService = tournamentService;
            _teamService = teamService;
            _footballPlayerService = footballPlayerService;
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
            [FromQuery(Name = "football-player-id")] int? footballPlayerId,
            [FromQuery(Name = "team-id")] int? teamId,
            [FromQuery(Name = "tournament-id")] int? tourId,
            [FromQuery(Name = "status")] string? status,
            [FromQuery(Name = "report-type")] ReportType? reportType,
            [FromQuery(Name = "order-by")] ReportFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<Report> reportList = _reportService.GetList().
                    Join(_userService.GetList(), rp => rp.User, u => u, (rp, u) => new { rp, u }).
                    GroupJoin(_footballPlayerService.GetList(), rpu => rpu.rp.FootballPlayer, fp => fp, (rpu, fp) => new { rpu, fp }).
                    SelectMany(rpufp => rpufp.fp.DefaultIfEmpty(), (rpufp, fp) => new { rpufp, fp }).
                    GroupJoin(_teamService.GetList(), rpufpteam => rpufpteam.rpufp.rpu.rp.Team, team => team, (rpufpteam, team) => new { rpufpteam, team }).
                    SelectMany(rpufpteams => rpufpteams.team.DefaultIfEmpty(), (rpufpteams, team) => new { rpufpteams, team }).
                    GroupJoin(_tournamentService.GetList(), rpfpteamstour => rpfpteamstour.rpufpteams.rpufpteam.rpufp.rpu.rp.Tournament, tour => tour, (rpfpteamstour, tour) => new { rpfpteamstour, tour }).
                    SelectMany(rpfpteamstours => rpfpteamstours.tour.DefaultIfEmpty(), (rpfpteamstours, tour) => new Report
                    {
                        Id = rpfpteamstours.rpfpteamstour.rpufpteams.rpufpteam.rpufp.rpu.rp.Id,
                        Reason = rpfpteamstours.rpfpteamstour.rpufpteams.rpufpteam.rpufp.rpu.rp.Reason,
                        DateReport = rpfpteamstours.rpfpteamstour.rpufpteams.rpufpteam.rpufp.rpu.rp.DateReport,
                        UserId = rpfpteamstours.rpfpteamstour.rpufpteams.rpufpteam.rpufp.rpu.rp.UserId,
                        FootballPlayerId = rpfpteamstours.rpfpteamstour.rpufpteams.rpufpteam.rpufp.rpu.rp.FootballPlayerId,
                        TeamId = rpfpteamstours.rpfpteamstour.rpufpteams.rpufpteam.rpufp.rpu.rp.TeamId,
                        TournamentId = rpfpteamstours.rpfpteamstour.rpufpteams.rpufpteam.rpufp.rpu.rp.TournamentId,
                        Status = rpfpteamstours.rpfpteamstour.rpufpteams.rpufpteam.rpufp.rpu.rp.Status,
                        User = rpfpteamstours.rpfpteamstour.rpufpteams.rpufpteam.rpufp.rpu.u,
                        FootballPlayer = rpfpteamstours.rpfpteamstour.rpufpteams.rpufpteam.fp,
                        Team = rpfpteamstours.rpfpteamstour.team,
                        Tournament = tour
                    });

                if (!String.IsNullOrEmpty(reason))
                {
                    reportList = reportList.Where(s => s.Reason!.ToUpper().Contains(reason.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(userId.ToString()))
                {
                    reportList = reportList.Where(s => s.UserId == userId);
                }
                if (!String.IsNullOrEmpty(footballPlayerId.ToString()))
                {
                    reportList = reportList.Where(s => s.FootballPlayerId == footballPlayerId);
                }
                if (!String.IsNullOrEmpty(teamId.ToString()))
                {
                    reportList = reportList.Where(s => s.TeamId == teamId);
                }
                if (!String.IsNullOrEmpty(tourId.ToString()))
                {
                    reportList = reportList.Where(s => s.TournamentId == tourId);
                }
                if (!String.IsNullOrEmpty(status))
                {
                    reportList = reportList.Where(s => s.Status!.ToUpper().Contains(status.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(reportType.ToString()))
                {
                    if (reportType == ReportType.FootballPlayer)
                    {
                        reportList = reportList.Where(s => s.FootballPlayerId > 0);
                    }
                    else if (reportType == ReportType.Team)
                    {
                        reportList = reportList.Where(s => s.TeamId > 0);
                    }
                    else
                    {
                        reportList = reportList.Where(s => s.TournamentId > 0);
                    }
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
                else if (orderBy == ReportFieldEnum.FootballPlayerId)
                {
                    reportList = reportList.OrderBy(rp => rp.FootballPlayerId);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        reportList = reportList.OrderByDescending(rp => rp.FootballPlayerId);
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

                ReportListVM reportListResponse = new()
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

        /// <summary>Get list report group</summary>
        /// <returns>List report group by</returns>
        /// <response code="200">Returns list report group by</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("group-by")]
        [Produces("application/json")]
        public async Task<ActionResult> GetListReportGroup(
            [FromQuery(Name = "report-type")] ReportType? reportType,
            [FromQuery(Name = "status")] string? status,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<Report> reportList = _reportService.GetList();
                if (!String.IsNullOrEmpty(status))
                {
                    reportList = reportList.Where(s => s.Status!.ToUpper().Contains(status.Trim().ToUpper()));
                }

                if (!String.IsNullOrEmpty(reportType.ToString()))
                {
                    List<ReportGroupBy> listReportGrb = new();
                    if (reportType == ReportType.FootballPlayer)
                    {
                        reportList = reportList.Where(rp => rp.FootballPlayerId > 0);
                        var listGrB = reportList.GroupBy(rp => rp.FootballPlayerId).Select(g => new { footballPlayerId = g.Key, count = g.Count() });
                        int countList = listGrB.Count();
                        listGrB = listGrB.Skip((pageIndex - 1) * limit).Take(limit);
                        var listGrbResponse = listGrB.ToList();
                        foreach (var response in listGrbResponse)
                        {
                            FootballPlayer fp = await _footballPlayerService.GetByIdAsync(response.footballPlayerId!.Value);
                            FootballPlayerReportVM fpVM = _mapper.Map<FootballPlayerReportVM>(fp);
                            fpVM.CountReport = response.count;
                            ReportGroupBy rpGrp = new()
                            {
                                FootballPlayerReportVM = fpVM,
                            };
                            listReportGrb.Add(rpGrp);
                        }

                        ListReportGroupBy reportListResponse = new()
                        {
                            Reports = listReportGrb,
                            CountList = countList,
                            CurrentPage = pageIndex,
                            Size = limit
                        };
                        return Ok(reportListResponse);
                    }
                    else if (reportType == ReportType.Team)
                    {
                        reportList = reportList.Where(rp => rp.TeamId > 0);
                        var listGrB = reportList.GroupBy(rp => rp.TeamId).Select(g => new { teamId = g.Key, count = g.Count() });
                        int countList = listGrB.Count();
                        listGrB = listGrB.Skip((pageIndex - 1) * limit).Take(limit);
                        var listGrbResponse = listGrB.ToList();
                        foreach (var response in listGrbResponse)
                        {
                            Team team = await _teamService.GetByIdAsync(response.teamId!.Value);
                            TeamReportVM teamVM = _mapper.Map<TeamReportVM>(team);
                            teamVM.CountReport = response.count;
                            ReportGroupBy rpGrp = new()
                            {
                                TeamReportVM = teamVM,
                            };
                            listReportGrb.Add(rpGrp);
                        }

                        ListReportGroupBy reportListResponse = new()
                        {
                            Reports = listReportGrb,
                            CountList = countList,
                            CurrentPage = pageIndex,
                            Size = limit
                        };
                        return Ok(reportListResponse);
                    }
                    else
                    {
                        reportList = reportList.Where(rp => rp.TournamentId > 0);
                        var listGrB = reportList.GroupBy(rp => rp.TournamentId).Select(g => new { tournamentId = g.Key, count = g.Count() });
                        int countList = listGrB.Count();
                        listGrB = listGrB.Skip((pageIndex - 1) * limit).Take(limit);
                        var listGrbResponse = listGrB.ToList();
                        foreach (var response in listGrbResponse)
                        {
                            Tournament tour = await _tournamentService.GetByIdAsync(response.tournamentId!.Value);
                            TournamentReportVM tourVM = _mapper.Map<TournamentReportVM>(tour);
                            tourVM.CountReport = response.count;
                            ReportGroupBy rpGrp = new()
                            {
                                TournamentReportVM = tourVM,
                            };
                            listReportGrb.Add(rpGrp);
                        }

                        ListReportGroupBy reportListResponse = new()
                        {
                            Reports = listReportGrb,
                            CountList = countList,
                            CurrentPage = pageIndex,
                            Size = limit
                        };
                        return Ok(reportListResponse);
                    }
                }
                return BadRequest("Chưa chọn loại báo cáo");
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
                    User user = await _userService.GetByIdAsync(currentReport.UserId!.Value);
                    currentReport.User = user;
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
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ReportVM>> CreateReport([FromBody] ReportCM model)
        {
            try
            {
                Report report = new();

                User user = await _userService.GetByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }

                if (!String.IsNullOrEmpty(model.FootballPlayerId.ToString()) && model.FootballPlayerId != 0)
                {
                    FootballPlayer footballPlayer = await _footballPlayerService.GetByIdAsync(model.FootballPlayerId!.Value);
                    if (footballPlayer == null)
                    {
                        return NotFound("Không tìm thấy cầu thủ");
                    }
                    else
                    {
                        report.FootballPlayerId = footballPlayer.Id;
                    }
                    IQueryable<Report> reportListAll = _reportService.GetList().Where(rp => rp.UserId == model.UserId && rp.FootballPlayerId == model.FootballPlayerId && rp.DateReport!.Value.Date.CompareTo(DateTime.Now.AddHours(7).Date) == 0);
                    if (reportListAll.Count() > 0)
                    {
                        return BadRequest("Mỗi ngày chỉ được báo cáo cầu thủ một lần");
                    }
                }
                if (!String.IsNullOrEmpty(model.TeamId.ToString()) && model.TeamId != 0)
                {
                    Team team = await _teamService.GetByIdAsync(model.TeamId!.Value);
                    if (team == null)
                    {
                        return NotFound("Không tìm thấy đội bóng");
                    }
                    else
                    {
                        report.TeamId = team.Id;
                    }
                    IQueryable<Report> reportListAll = _reportService.GetList().Where(rp => rp.UserId == model.UserId && rp.TeamId == model.TeamId && rp.DateReport!.Value.Date.CompareTo(DateTime.Now.AddHours(7).Date) == 0);
                    if (reportListAll.Count() > 0)
                    {
                        return BadRequest("Mỗi ngày chỉ được báo cáo đội bóng một lần");
                    }
                }
                if (!String.IsNullOrEmpty(model.TournamentId.ToString()) && model.TournamentId != 0)
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync(model.TournamentId!.Value);
                    if (tournament == null)
                    {
                        return NotFound("Không tìm thấy giải đấu");
                    }
                    else
                    {
                        report.TournamentId = tournament.Id;
                    }
                    IQueryable<Report> reportList = _reportService.GetList().Where(rp => rp.UserId == model.UserId && rp.TournamentId == model.TournamentId);
                    if(reportList.Count() > 0)
                    {
                        return BadRequest("Mỗi giải chỉ được báo cáo một lần");
                    }
                }
                report.Status = String.IsNullOrEmpty(model.Status) ? "Chưa duyệt" : model.Status;
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
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ReportVM>> UpdateReport([FromBody] ReportUM model)
        {
            try
            {
                Report report = await _reportService.GetByIdAsync(model.Id);
                if (report == null)
                {
                    return NotFound("Không tìm thấy báo cáo");
                }

                report.Reason = String.IsNullOrEmpty(model.Reason) ? report.Reason : model.Reason;
                if (!String.IsNullOrEmpty(model.UserId.ToString()) && model.UserId != 0)
                {
                    User user = await _userService.GetByIdAsync(model.UserId!.Value);
                    if (user != null)
                    {
                        report.UserId = user.Id;
                    }
                }
                if (!String.IsNullOrEmpty(model.FootballPlayerId.ToString()) && model.FootballPlayerId != 0)
                {
                    FootballPlayer footballPlayer = await _footballPlayerService.GetByIdAsync(model.FootballPlayerId!.Value);
                    if (footballPlayer != null)
                    {
                        report.FootballPlayerId = footballPlayer.Id;
                    }
                }
                if (!String.IsNullOrEmpty(model.TeamId.ToString()) && model.TeamId != 0)
                {
                    Team team = await _teamService.GetByIdAsync(model.TeamId!.Value);
                    if (team != null)
                    {
                        report.TeamId = team.Id;
                    }
                }
                if (!String.IsNullOrEmpty(model.TournamentId.ToString()) && model.TournamentId != 0)
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync(model.TournamentId!.Value);
                    if (tournament != null)
                    {
                        report.TournamentId = tournament.Id;
                    }
                }
                report.Status = String.IsNullOrEmpty(model.Status) ? report.Status : model.Status;

                bool isUpdated = await _reportService.UpdateAsync(report);
                if (isUpdated)
                {
                    User user = await _userService.GetByIdAsync(report.UserId!.Value);
                    report.User = user;
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
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
