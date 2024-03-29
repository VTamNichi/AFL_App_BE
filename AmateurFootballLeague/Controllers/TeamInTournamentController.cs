﻿using AmateurFootballLeague.IServices;
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
    [Route("api/v1/team-in-tournaments")]
    [ApiController]
    public class TeamInTournamentController : ControllerBase
    {
        private readonly ITeamInTournamentService _teamInTournamentService;
        private readonly ITournamentService _tournamentService;
        private readonly ITeamService _teamService;
        private readonly ITeamInMatchService _teamInMatchService;
        private readonly IMapper _mapper;

        public TeamInTournamentController(ITeamInTournamentService teamInTournamentService, ITournamentService tournamentService, ITeamService teamService, ITeamInMatchService teamInMatchService, IMapper mapper)
        {
            _teamInTournamentService = teamInTournamentService;
            _tournamentService = tournamentService;
            _teamService = teamService;
            _teamInMatchService = teamInMatchService;
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
                if (!String.IsNullOrEmpty(status))
                {
                    teamInTournamentList = teamInTournamentList.Where(s => s.Status == status);
                }

                if (orderBy == TeamInTournamentFieldEnum.Id)
                {
                    teamInTournamentList = teamInTournamentList.OrderBy(tnm => tnm.Id);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        teamInTournamentList = teamInTournamentList.OrderByDescending(tnm => tnm.Id);
                    }
                }
                if (orderBy == TeamInTournamentFieldEnum.Point)
                {
                    teamInTournamentList = teamInTournamentList.OrderBy(tnm => tnm.Point);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        teamInTournamentList = teamInTournamentList.OrderByDescending(tnm => tnm.Point);
                        //var listTeam = teamInTournamentList.ToList();
                        //var listCompare = teamInTournamentList.ToList();
                        //List<TeamInTournament> listSort = new List<TeamInTournament>();
                        //for (int i = 0; i < listTeam.Count(); i++)
                        //{
                        //    TeamInTournament tit = listCompare[0];
                        //    for (int j = 0; j < listCompare.Count(); j++)
                        //    {
                        //        if (listCompare[j].Point >= tit.Point)
                        //        {
                        //            if (listCompare[j].Point > tit.Point)
                        //            {
                        //                tit = listCompare[j];
                        //            }
                        //            else
                        //            {
                        //                if (listCompare[j].DifferentPoint >= tit.DifferentPoint)
                        //                {
                        //                    if (listCompare[j].DifferentPoint > tit.DifferentPoint)
                        //                    {
                        //                        tit = listCompare[j];
                        //                    }
                        //                    else
                        //                    {
                        //                        if (listCompare[j].TotalRedCard <= tit.TotalRedCard)
                        //                        {
                        //                            if (listCompare[j].TotalRedCard < tit.TotalRedCard)
                        //                            {
                        //                                tit = listCompare[j];
                        //                            }
                        //                            else
                        //                            {
                        //                                if (listCompare[j].TotalYellowCard <= tit.TotalYellowCard)
                        //                                {
                        //                                    if (listCompare[j].TotalYellowCard < tit.TotalYellowCard)
                        //                                    {
                        //                                        tit = listCompare[j];
                        //                                    }
                        //                                }
                        //                            }
                        //                        }
                        //                    }
                        //                }
                        //            }

                        //        }

                        //    }
                        //    listSort.Add(tit);
                        //    listCompare.Remove(tit);
                        //}
                        //int countListRank = teamInTournamentList.Count();

                        //var rankingTeamListPaging = listSort.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                        //List<TeamInTournamentVM> listRankingTeamVM = new();
                        //listRankingTeamVM = _mapper.Map<List<TeamInTournamentVM>>(rankingTeamListPaging);

                        //var rankingListTeam = new TeamInTournamentListVM
                        //{
                        //    TeamInTournaments = listRankingTeamVM,
                        //    CountList = countListRank,
                        //    CurrentPage = pageIndex,
                        //    Size = limit
                        //};

                        //return Ok(rankingListTeam);
                    }
                }
                if (orderBy == TeamInTournamentFieldEnum.DifferentPoint)
                {
                    teamInTournamentList = teamInTournamentList.OrderBy(tnm => tnm.DifferentPoint);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        teamInTournamentList = teamInTournamentList.OrderByDescending(tnm => tnm.DifferentPoint);
                    }
                }

                int countList = teamInTournamentList.Count();

                teamInTournamentList = teamInTournamentList.Skip((pageIndex - 1) * limit).Take(limit);
                var teamInTournamentListPaging = teamInTournamentList.Join(_teamService.GetList(), tit => tit.Team, t => t, (tit, t) => new TeamInTournament
                {
                    Id = tit.Id,
                    Point = tit.Point,
                    WinScoreNumber = tit.WinScoreNumber,
                    LoseScoreNumber = tit.LoseScoreNumber,
                    DifferentPoint = tit.DifferentPoint,
                    TotalYellowCard = tit.TotalYellowCard,
                    TotalRedCard = tit.TotalRedCard,
                    WinTieBreak = tit.WinTieBreak,
                    GroupName = tit.GroupName,
                    Status = tit.Status,
                    StatusInTournament = tit.StatusInTournament,
                    TournamentId = tit.TournamentId,
                    TeamId = tit.TeamId,
                    Team = new Team
                    {
                        Id = t.Id,
                        TeamName = t.TeamName,
                        TeamAvatar = t.TeamAvatar,
                        TeamPhone = t.TeamPhone,
                        TeamGender = t.TeamGender
                    }
                }).OrderByDescending(o => o.StatusInTournament).OrderByDescending(o => o.Point).ThenByDescending(o => o.DifferentPoint).ThenByDescending(o => o.WinScoreNumber).ThenBy(o => o.TotalRedCard).ThenBy(o => o.TotalYellowCard).ThenByDescending(o => o.WinTieBreak);

                List<TeamInTournamentVM> listTeamInTournamentVM = new();
                listTeamInTournamentVM = _mapper.Map<List<TeamInTournamentVM>>(teamInTournamentListPaging);
                foreach (var teamInTournamentVM in listTeamInTournamentVM)
                {
                    teamInTournamentVM.numberOfMatch = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == teamInTournamentVM.Id && s.Result != null && s.WinTieBreak == null).Count();
                    teamInTournamentVM.numberOfWin = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == teamInTournamentVM.Id && s.Result > 1 && s.WinTieBreak == null).Count();
                    teamInTournamentVM.numberOfLose = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == teamInTournamentVM.Id && s.Result < 1 && s.WinTieBreak == null).Count();
                    teamInTournamentVM.numberOfDraw = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == teamInTournamentVM.Id && s.Result == 1 && s.WinTieBreak == null).Count();
                }

                var teamInTournamentListResponse = new TeamInTournamentListVM
                {
                    TeamInTournaments = listTeamInTournamentVM,
                    CountList = countList,
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
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<TeamInTournamentVM>> CreateTeamInTournament([FromBody] TeamInTournamentCM model)
        {
            TeamInTournament teamInTournament = new();
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
                DateTime date = DateTime.Now.AddHours(7);
                IQueryable<TeamInTournament> checkTeam = _teamInTournamentService.GetList().Join(_tournamentService.GetList(), tit => tit.Tournament, t => t, (tit, t) => new { tit, t })
                    .Where(t => t.t.TournamentEndDate > date)
                    .Join(_teamService.GetList(), titt => titt.tit.Team, team => team, (titt, team) => new { titt, team })
                    .Where(t => t.team.Id == model.TeamId).Select(t => new TeamInTournament
                    {
                        Id = t.titt.tit.Id,
                        Status = t.titt.tit.Status,
                        StatusInTournament = t.titt.tit.StatusInTournament
                    })
                    .Where(tit => tit.StatusInTournament != "Bị loại" && tit.Status == "Tham gia");

                //if (checkTeam.Count() > 0)
                //{
                //    return BadRequest(new
                //    {
                //        message = "Đội của bạn đang tham gia một giải đấu khác"
                //    });
                //}
                
                teamInTournament.TournamentId = model.TournamentId;
                teamInTournament.TeamId = model.TeamId;
                teamInTournament.Point = String.IsNullOrEmpty(model.Point.ToString()) ? 0 : model.Point;
                teamInTournament.WinScoreNumber = String.IsNullOrEmpty(model.WinScoreNumber.ToString()) ? 0 : model.WinScoreNumber;
                teamInTournament.LoseScoreNumber = String.IsNullOrEmpty(model.LoseScoreNumber.ToString()) ? 0 : model.LoseScoreNumber;
                teamInTournament.DifferentPoint = teamInTournament.WinScoreNumber - teamInTournament.LoseScoreNumber;
                teamInTournament.TotalYellowCard = String.IsNullOrEmpty(model.TotalYellowCard.ToString()) ? 0 : model.TotalYellowCard;
                teamInTournament.TotalRedCard = String.IsNullOrEmpty(model.TotalRedCard.ToString()) ? 0 : model.TotalRedCard;
                teamInTournament.WinTieBreak = String.IsNullOrEmpty(model.WinTieBreak.ToString()) ? 0 : model.WinTieBreak;
                teamInTournament.Status = String.IsNullOrEmpty(model.Status) ? "" : model.Status;
                teamInTournament.StatusInTournament = "Trong giải";
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
        /// <response code="201">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not team in tournament</response>
        /// <response code="500">Failed to save request</response>
        /// 

        [HttpPut("card")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdateTotalCard(TeamInTournamentUMR model)
        {
            try
            {
                TeamInTournament teamInTournament = await _teamInTournamentService.GetByIdAsync(model.Id);
                if (teamInTournament == null)
                {
                    return NotFound("Không tìm thấy đội bóng trong giải đấu với id là " + model.Id);
                }
                teamInTournament.TotalRedCard = model.TotalRedCard;
                teamInTournament.TotalYellowCard = model.TotalYellowCard;
                bool isUpdate = await _teamInTournamentService.UpdateAsync(teamInTournament);
                if (isUpdate)
                {
                    return Ok("Cập nhật thẻ thành công");
                }
                return BadRequest("Cập nhật thẻ thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Produces("application/json")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<TeamInTournamentVM>> UpdateTeamInTournament([FromBody] TeamInTournamentUM model)
        {
            try
            {
                TeamInTournament teamInTournament = await _teamInTournamentService.GetByIdAsync(model.Id);
                if (teamInTournament == null)
                {
                    return NotFound("Không tìm thấy đội bóng trong giải đấu với id là " + model.Id);
                }
                if (!String.IsNullOrEmpty(model.TournamentId.ToString()) && model.TournamentId != 0)
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync(model.TournamentId!.Value);
                    if (tournament == null)
                    {
                        return BadRequest("Giải đấu không tồn tại");
                    }
                    else
                    {
                        teamInTournament.TournamentId = model.TournamentId;
                    }
                    int teamNum = _teamInTournamentService.CountTeamInATournament(model.TournamentId!.Value);
                    if(teamNum == tournament.FootballTeamNumber && model.Status == "Tham gia")
                    {
                        return BadRequest("Giải đấu đã đủ đội");
                    }    
                }


                if (!String.IsNullOrEmpty(model.TeamId.ToString()) && model.TeamId != 0)
                {
                    Team team = await _teamService.GetByIdAsync(model.TeamId!.Value);
                    if (team == null)
                    {
                        return BadRequest("Đội bóng không tồn tại");
                    }
                    else
                    {
                        teamInTournament.TeamId = model.TeamId;
                    }
                }
                DateTime date = DateTime.Now.AddHours(7);
                IQueryable<TeamInTournament> checkTeam = _teamInTournamentService.GetList().Join(_tournamentService.GetList(), tit => tit.Tournament, t => t, (tit, t) => new { tit, t })
                    .Where(t => t.t.TournamentEndDate > date)
                    .Join(_teamService.GetList(), titt => titt.tit.Team, team => team, (titt, team) =>new {titt,team})
                    .Where(t=> t.team.Id == model.TeamId).Select(t => new TeamInTournament
                    {
                        Id = t.titt.tit.Id,
                        Status = t.titt.tit.Status,
                        StatusInTournament = t.titt.tit.StatusInTournament
                    })
                    .Where(tit => tit.StatusInTournament != "Bị loại" && tit.Status == "Tham gia");
                //if (checkTeam.Count() > 0)
                //{
                //    return BadRequest(new
                //    {
                //        message = "Đội của bạn đang tham gia một giải đấu khác"
                //    });
                //}


                teamInTournament.Point = String.IsNullOrEmpty(model.Point.ToString()) && model.Point != 0 ? teamInTournament.Point : model.Point;
                teamInTournament.WinScoreNumber = String.IsNullOrEmpty(model.WinScoreNumber.ToString()) || model.WinScoreNumber == 0 ? teamInTournament.WinScoreNumber : model.WinScoreNumber;
                teamInTournament.LoseScoreNumber = String.IsNullOrEmpty(model.LoseScoreNumber.ToString()) || model.LoseScoreNumber == 0 ? teamInTournament.LoseScoreNumber : model.LoseScoreNumber;
                teamInTournament.DifferentPoint = teamInTournament.WinScoreNumber - teamInTournament.LoseScoreNumber;
                teamInTournament.TotalYellowCard = String.IsNullOrEmpty(model.TotalYellowCard.ToString()) || model.TotalYellowCard == 0 ? teamInTournament.TotalYellowCard : model.TotalYellowCard;
                teamInTournament.TotalRedCard = String.IsNullOrEmpty(model.TotalRedCard.ToString()) || model.TotalRedCard == 0 ? teamInTournament.TotalRedCard : model.TotalRedCard;
                teamInTournament.Status = String.IsNullOrEmpty(model.Status) ? teamInTournament.Status : model.Status;
                teamInTournament.StatusInTournament = String.IsNullOrEmpty(model.StatusInTournament) ? teamInTournament.StatusInTournament : model.StatusInTournament;
                bool isUpdate = await _teamInTournamentService.UpdateAsync(teamInTournament);
                if (isUpdate)
                {
                    return CreatedAtAction("GetTeamInTournamentById", new { id = teamInTournament.Id }, _mapper.Map<TeamInTournamentVM>(teamInTournament));
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
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        /// <summary>Delete team in tournament By Tournament Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("delete-by-tournament-id")]
        [Produces("application/json")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> DeleteByTournamrntId(int tournamentId)
        {
            try
            {
                List<TeamInTournament> listTeamInTournament = _teamInTournamentService.GetList().Where(t => t.TournamentId == tournamentId).ToList();
                foreach (TeamInTournament teamInTournament in listTeamInTournament)
                {
                    await _teamInTournamentService.DeleteAsync(teamInTournament);
                }
                return Ok("Xóa đội bóng trong giải đấu thành công");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update score a team in tournament by tournament id</summary>
        /// <response code="201">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not team in tournament</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut("update-score")]
        [Produces("application/json")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<TeamInTournamentVM>> UpdateTeamInTournamentByTournamentId(int tournamentId)
        {
            try
            {
                var teamInMatchList = _teamInMatchService.GetList().Where(tim => tim.Match!.TournamentId == tournamentId && tim.TeamInTournamentId.HasValue).GroupBy(tim => tim.TeamInTournamentId).Select(g => new { titID = g.Key, sumScore = g.Sum(s => s.TeamScore), sumScoreLose = g.Sum(s => s.TeamScoreLose), sumYellow = g.Sum(s => s.YellowCardNumber), sumRed = g.Sum(s => s.RedCardNumber), sumPoint = g.Sum(s => s.Result), sumTiebreak = g.Sum(s => s.WinTieBreak)});
                foreach (var tim in teamInMatchList.ToList())
                {
                    TeamInTournament tit = await _teamInTournamentService.GetByIdAsync(tim.titID!.Value);
                    tit.WinScoreNumber = tim.sumScore;
                    tit.LoseScoreNumber = tim.sumScoreLose;
                    tit.DifferentPoint = tim.sumScore - tim.sumScoreLose;
                    tit.TotalYellowCard = tim.sumYellow;
                    tit.TotalRedCard = tim.sumRed;
                    tit.Point = tim.sumPoint;
                    tit.WinTieBreak = tim.sumTiebreak;
                    await _teamInTournamentService.UpdateAsync(tit);
                }

                return Ok("Cập nhật điểm số đội bóng trong giải đấu thành công");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
