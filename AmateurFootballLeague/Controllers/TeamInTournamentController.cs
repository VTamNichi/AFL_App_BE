﻿using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/team-in-tournaments")]
    [ApiController]
    public class TeamInTournamentController : ControllerBase
    {
        private readonly ITeamInTournamentService _teamInTournamentService;
        private readonly ITournamentService _tournamentService;
        private readonly ITeamService _teamService;
        private readonly IMapper _mapper;

        public TeamInTournamentController(ITeamInTournamentService teamInTournamentService, ITournamentService tournamentService, ITeamService teamService, IMapper mapper)
        {
            _teamInTournamentService = teamInTournamentService;
            _tournamentService = tournamentService;
            _teamService = teamService;
            _mapper = mapper;
        }

        /// <summary>Get list team in tournament</summary>
        /// <returns>List team in tournament</returns>
        /// <response code="200">Returns list team in tournament</response>
        /// <response code="404">Not found team in tournament</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<TeamInTournamentListVM> GetListTeamInTournament(
            [FromQuery(Name = "tournament-id")] int? tourId,
            [FromQuery(Name = "team-id")] int? teamId,
            [FromQuery(Name = "point-min")] int? pointMin,
            [FromQuery(Name = "point-max")] int? pointMax,
            [FromQuery(Name = "difference-point-min")] int? differencePointMin,
            [FromQuery(Name = "difference-point-max")] int? differencePointMax,
            [FromQuery(Name = "status")] string? status,
            [FromQuery(Name = "order-by")] TeamInTournamentFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<TeamInTournament> teamInTournamentList = _teamInTournamentService.GetList();
                if (!String.IsNullOrEmpty(tourId.ToString()))
                {
                    teamInTournamentList = teamInTournamentList.Where(s => s.TournamentId == tourId);
                }
                if (!String.IsNullOrEmpty(teamId.ToString()))
                {
                    teamInTournamentList = teamInTournamentList.Where(s => s.TeamId == teamId);
                }
                if (!String.IsNullOrEmpty(pointMin.ToString()))
                {
                    teamInTournamentList = teamInTournamentList.Where(s => s.Point >= pointMin);
                }
                if (!String.IsNullOrEmpty(pointMax.ToString()))
                {
                    teamInTournamentList = teamInTournamentList.Where(s => s.Point <= pointMax);
                }
                if (!String.IsNullOrEmpty(differencePointMin.ToString()))
                {
                    teamInTournamentList = teamInTournamentList.Where(s => s.Point >= differencePointMin);
                }
                if (!String.IsNullOrEmpty(differencePointMax.ToString()))
                {
                    teamInTournamentList = teamInTournamentList.Where(s => s.Point <= differencePointMax);
                }
                if (!String.IsNullOrEmpty(status.ToString()))
                {
                    teamInTournamentList = teamInTournamentList.Where(s => s.Status == status);
                }

                var teamInTournamentListPaging = teamInTournamentList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var teamInTournamentListOrder = new List<TeamInTournament>();
                if (orderBy == TeamInTournamentFieldEnum.Id)
                {
                    teamInTournamentListOrder = teamInTournamentListPaging.OrderBy(tnm => tnm.Id).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        teamInTournamentListOrder = teamInTournamentListPaging.OrderByDescending(tnm => tnm.Id).ToList();
                    }
                }
                if (orderBy == TeamInTournamentFieldEnum.Point)
                {
                    teamInTournamentListOrder = teamInTournamentListPaging.OrderBy(tnm => tnm.Point).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        teamInTournamentListOrder = teamInTournamentListPaging.OrderByDescending(tnm => tnm.Point).ToList();
                    }
                }
                if (orderBy == TeamInTournamentFieldEnum.DifferentPoint)
                {
                    teamInTournamentListOrder = teamInTournamentListPaging.OrderBy(tnm => tnm.DifferentPoint).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        teamInTournamentListOrder = teamInTournamentListPaging.OrderByDescending(tnm => tnm.DifferentPoint).ToList();
                    }
                }

                var teamInTournamentListResponse = new TeamInTournamentListVM
                {
                    TeamInTournaments = _mapper.Map<List<TeamInTournament>, List<TeamInTournamentVM>>(teamInTournamentListOrder),
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(teamInTournamentListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get team in tournament by id</summary>
        /// <returns>Return the team in tournament with the corresponding id</returns>
        /// <response code="200">Returns the team in tournament with the specified id</response>
        /// <response code="404">No team in tournament found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<TeamInTournamentVM>> GetTeamInTournamentById(int id)
        {
            try
            {
                TeamInTournament currentTeamInTournament = await _teamInTournamentService.GetByIdAsync(id);
                if (currentTeamInTournament != null)
                {
                    return Ok(_mapper.Map<TeamInTournamentVM>(currentTeamInTournament));
                }
                return NotFound("Không tìm thấy đội bóng trong giả đấu với id là " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new team in tournament</summary>
        /// <response code="201">Created new team in tournament successfull</response>
        /// <response code="400">Field is not team in tournament or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<TeamInTournamentVM>> CreateTeamInTournament([FromBody] TeamInTournamentCM model)
        {
            TeamInTournament teamInTournament = new TeamInTournament();
            try
            {
                Tournament tournament = await _tournamentService.GetByIdAsync(model.TournamentId);
                if (tournament == null)
                {
                    return BadRequest("Giải đấu không tồn tại");
                }
                Team team = await _teamService.GetByIdAsync(model.TeamId);
                if (team == null)
                {
                    return BadRequest("Đội bóng không tồn tại");
                }
                teamInTournament.TournamentId = model.TournamentId;
                teamInTournament.TeamId = model.TeamId;
                teamInTournament.Point = String.IsNullOrEmpty(model.Point.ToString()) ? 0 : model.Point;
                teamInTournament.DifferentPoint = String.IsNullOrEmpty(model.DifferentPoint.ToString()) ? 0 : model.DifferentPoint;
                teamInTournament.Status = String.IsNullOrEmpty(model.Status.ToString()) ? "" : model.Status;

                TeamInTournament teamInTournamentCreated = await _teamInTournamentService.AddAsync(teamInTournament);
                if (teamInTournamentCreated != null)
                {
                    return CreatedAtAction("GetTeamInTournamentById", new { id = teamInTournamentCreated.Id }, _mapper.Map<TeamInTournamentVM>(teamInTournamentCreated));
                }
                return BadRequest("Thêm đội bóng vào giải đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a team in tournament</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not team in tournament</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<TeamInTournamentVM>> UpdateTeamInTournament([FromBody] TeamInTournamentUM model)
        {
            try
            {
                TeamInTournament teamInTournament = await _teamInTournamentService.GetByIdAsync(model.Id);
                if (teamInTournament == null)
                {
                    return NotFound("Không tìm thấy đội bóng trong giải đấu với id là " + model.Id);
                }
                if (!String.IsNullOrEmpty(model.TournamentId.ToString()))
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync((int)model.TournamentId);
                    if (tournament == null)
                    {
                        return BadRequest("Giải đấu không tồn tại");
                    } 
                    else
                    {
                        teamInTournament.TournamentId = model.TournamentId;
                    }
                }
                if (!String.IsNullOrEmpty(model.TeamId.ToString()))
                {
                    Team team = await _teamService.GetByIdAsync((int)model.TeamId);
                    if (team == null)
                    {
                        return BadRequest("Đội bóng không tồn tại");
                    }
                    else
                    {
                        teamInTournament.TeamId = model.TeamId;
                    }
                }
                
                teamInTournament.Point = String.IsNullOrEmpty(model.Point.ToString()) ? 0 : model.Point;
                teamInTournament.DifferentPoint = String.IsNullOrEmpty(model.DifferentPoint.ToString()) ? 0 : model.DifferentPoint;
                teamInTournament.Status = String.IsNullOrEmpty(model.Status.ToString()) ? "" : model.Status;

                TeamInTournament teamInTournamentCreated = await _teamInTournamentService.AddAsync(teamInTournament);
                if (teamInTournamentCreated != null)
                {
                    return CreatedAtAction("GetTeamInTournamentById", new { id = teamInTournamentCreated.Id }, _mapper.Map<TeamInTournamentVM>(teamInTournamentCreated));
                }
                return BadRequest("Cập nhật đội bóng trong giải đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete team in tournament By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            TeamInTournament currentTeamInTournament = await _teamInTournamentService.GetByIdAsync(id);
            if (currentTeamInTournament == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy đội bóng trong giải đấu với id là " + id
                });
            }
            try
            {
                bool isDeleted = await _teamInTournamentService.DeleteAsync(currentTeamInTournament);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Xóa đội bóng trong giải đấu thành công"
                    });
                }
                return BadRequest("Xóa đội bóng trong giải đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
