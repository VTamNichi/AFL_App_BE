﻿using AmateurFootballLeague.Hubs;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TeamInMatchController : ControllerBase
    {
        private readonly ITeamInMatchService _teamInMatch;
        private readonly IMapper _mapper;
        private readonly IMatchService _matchService;
        private readonly ITeamService _teamService;
        private readonly ITeamInTournamentService _teamInTournamentService;
        private readonly ITournamentService _tournamentService;
        private readonly IHubContext<CommentHub> _hubContext;

        public TeamInMatchController(ITeamInMatchService teamInMatch,IMapper mapper, IMatchService matchService,ITournamentService tournamentService,
            ITeamService teamService, ITeamInTournamentService teamInTournamentService, IHubContext<CommentHub> hubContext)
        {
            _teamInMatch = teamInMatch;
            _mapper = mapper;
            _matchService = matchService;
            _teamService = teamService;
            _tournamentService= tournamentService;
            _teamInTournamentService = teamInTournamentService;
            _hubContext = hubContext;
        }

        [HttpGet]
        public ActionResult<List<TeamInMatch>> GetAllTeamInMatchInTournament(int tournamentId ,bool? fullInfo, SortTypeEnum orderType)
        {
            try
            {
                //IQueryable<TeamInMatch> listTeam = _teamInMatch.GetList().Join(_matchService.GetList(), tim => tim.Match, m => m, (tim, m) => new { tim, m })
                //    .Join(_teamService.GetList(), timm => timm.tim.TeamInTournament.Team, team => team, (timm, team) => new TeamInMatch
                //    {
                //        Id = timm.tim.Id,
                //        TeamScore = timm.tim.TeamScore,
                //        TeamScoreLose = timm.tim.TeamScoreLose,
                //        YellowCardNumber = timm.tim.YellowCardNumber,
                //        RedCardNumber = timm.tim.RedCardNumber,
                //        TeamInTournamentId = timm.tim.TeamInTournamentId,
                //        TeamInTournament  = new TeamInTournament
                //        {
                //            Team = team
                //        },
                //        MatchId = timm.tim.MatchId,
                //        Result = timm.tim.Result,
                //        NextTeam = timm.tim.NextTeam,
                //        TeamName = timm.tim.TeamName,
                //        Match = timm.tim.Match,
                //    }).Where(t => t.Match.TournamentId == tournamentId);
                IQueryable<TeamInMatch> listTeam = _teamInMatch.GetList().Join(_matchService.GetList(), tim => tim.Match, m => m, (tim, m) => new { tim, m })
                    //.Join(_teamInTournamentService.GetList(), timt => timt.tim.TeamInTournament, tit => tit, (timt, tit) => new{timt,tit })
    .GroupJoin(_teamService.GetList(), timm => timm.tim.TeamInTournament!.Team, team => team, (timm, team) => new {timm,team})
    .SelectMany(t => t.team.DefaultIfEmpty(),(t, temp) => new TeamInMatch
    {
        Id = t.timm.tim.Id,
        TeamScore = t.timm.tim.TeamScore,
        TeamScoreLose = t.timm.tim.TeamScoreLose,
        YellowCardNumber = t.timm.tim.YellowCardNumber,
        RedCardNumber = t.timm.tim.RedCardNumber,
        TeamInTournamentId = t.timm.tim.TeamInTournamentId,
        TeamInTournament = new TeamInTournament
        {

            Team = temp
        },
        MatchId = t.timm.tim.MatchId,
        Result = t.timm.tim.Result,
        NextTeam = t.timm.tim.NextTeam,
        TeamName = t.timm.tim.TeamName,
        Match = t.timm.tim.Match,
    }).Where(t => t.Match!.TournamentId == tournamentId);
                if (fullInfo == true)
                {
                    listTeam = _teamInMatch.GetList().Join(_matchService.GetList(), tim => tim.Match, m => m, (tim, m) => new { tim, m })
                   .Join(_teamInTournamentService.GetList(), ttim => ttim.tim.TeamInTournament, te => te, (ttim, te) => new { ttim, te })
                   .Join(_tournamentService.GetList(), timt => timt.ttim.m.Tournament, t => t, (timt, t) => new TeamInMatch
                   {
                       Id = timt.ttim.tim.Id,
                       TeamScore = timt.ttim.tim.TeamScore,
                       TeamScoreLose = timt.ttim.tim.TeamScoreLose,
                       YellowCardNumber = timt.ttim.tim.YellowCardNumber,
                       RedCardNumber = timt.ttim.tim.RedCardNumber,
                       TeamInTournamentId = timt.ttim.tim.TeamInTournamentId,
                       MatchId = timt.ttim.m.Id,
                       Result = timt.ttim.tim.Result,
                       NextTeam = timt.ttim.tim.NextTeam,
                       TeamName = timt.ttim.tim.TeamName,
                       Match = timt.ttim.m,
                       TeamInTournament = timt.te
                   }).Where(m => m.Match!.TournamentId == tournamentId);
                }
                if(orderType == SortTypeEnum.DESC)
                {
                    listTeam = listTeam.OrderByDescending(t => t.Id);
                }
                var temInMatch = new List<TeamInMatch>();
                temInMatch = listTeam.ToList();
                if (temInMatch.Count > 0)
                {

                    var teamListResponse = new TeamInMatchMTLV
                    {

                        TeamsInMatch = _mapper.Map<List<TeamInMatch>, List<TeamInMatchMT>>(temInMatch)

                    };
                    return Ok(temInMatch);

                }

                return NotFound("Không tìm thấy đội bóng trong trận đấu");
            }
            catch (Exception ex)
            {
                //return StatusCode(StatusCodes.Status500InternalServerError);
                return BadRequest(ex);
            }
        }

        [HttpGet("matchId")]
        public ActionResult<TeamInMatchMT> GetAllTeamInMatch(int matchId)
        {
            try
            {
                IQueryable<TeamInMatch> listTeam = _teamInMatch.GetList().Join(_matchService.GetList() ,tim => tim.Match, m => m, (tim, m) => new  {tim , m}).
                    Join(_teamInTournamentService.GetList(), titm => titm.tim.TeamInTournament, tit=> tit , (titm, tit)=> new {titm, tit}).
                    Join(_teamService.GetList(), timm => timm.tit.Team, team => team, (timm, team) => new TeamInMatch
                    {
                        Id = timm.titm.tim.Id,
                        TeamScore = timm.titm.tim.TeamScore,
                        TeamScoreLose = timm.titm.tim.TeamScoreLose,
                        YellowCardNumber = timm.titm.tim.YellowCardNumber,
                        RedCardNumber = timm.titm.tim.RedCardNumber,
                        TeamInTournamentId = timm.titm.tim.TeamInTournamentId,
                        TeamInTournament = new TeamInTournament
                        {
                            Id = timm.tit.Id,
                            Point = timm.tit.Point,
                            WinScoreNumber = timm.tit.WinScoreNumber,
                            LoseScoreNumber = timm.tit.LoseScoreNumber,
                            DifferentPoint = timm.tit.DifferentPoint,
                            TotalYellowCard = timm.tit.TotalYellowCard,
                            TotalRedCard = timm.tit.TotalRedCard,
                            Status = timm.tit.Status,
                            StatusInTournament = timm.tit.StatusInTournament,
                            TournamentId = timm.tit.TournamentId,
                            TeamId = timm.tit.TeamId,
                            Team = team
                        },
                        MatchId = timm.titm.tim.MatchId,
                        Result =timm.titm.tim.Result,
                        NextTeam = timm.titm.tim.NextTeam,
                        TeamName = timm.titm.tim.TeamName,
                        Match = timm.titm.tim.Match,
                    }
    ).Where(m => m.MatchId == matchId);
                var temInMatch = new List<TeamInMatch>();
                temInMatch = listTeam.ToList();
                if (temInMatch.Count > 0)
                {

                    var teamListResponse = new TeamInMatchMTLV
                    {

                        TeamsInMatch = _mapper.Map<List<TeamInMatch>, List<TeamInMatchMT>>(temInMatch)

                    };
                    return Ok(teamListResponse);

                }

                return NotFound("Không tìm thấy đội bóng trong trận đấu");

            }
            catch(Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<TeamInMatchVM>> GetById(int id)
        {
            try
            {
                TeamInMatch team = await _teamInMatch.GetByIdAsync(id);

                if (team != null)
                {
                    return Ok(_mapper.Map<TeamInMatchVM>(team));
                }
                return NotFound("Không tìm thấy đội bóng trong trận đấu vơi id là " + id);

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TeamInMatchVM>> CreateTeamInMatch(TeamInMatchCM teamInMatch)
        {
            TeamInMatch team = new();
            try
            {
                var checkTeam = _teamInMatch.GetList().Where(t=> t.MatchId == teamInMatch.MatchId && t.TeamInTournamentId == teamInMatch.TeamInTournamentId);
                if(checkTeam != null)
                {
                    return BadRequest(new
                    {
                        message = "Đội bóng đã tồn tại trong trận đấu"
                    });

                }
                var checkMatch = _teamInMatch.GetList().Where(t => t.MatchId == teamInMatch.MatchId);
                if(checkMatch.Count() == 2)
                {
                    return BadRequest(new
                    {
                        message = "Đã tồn tại 2 đội bóng trong trận đấu này"
                    });
                }
                team.TeamScore = teamInMatch.TeamScore;
                team.TeamScoreLose = teamInMatch.TeamScoreLose;
                team.YellowCardNumber = teamInMatch.YellowCardNumber;
                team.RedCardNumber = teamInMatch.RedCardNumber;
                team.TeamInTournamentId = teamInMatch.TeamInTournamentId;
                team.MatchId = teamInMatch.MatchId;
                //team.Result = 0;
                team.NextTeam = "";
                team.TeamName = teamInMatch.TeamName;
                TeamInMatch created = await _teamInMatch.AddAsync(team);
                if (created != null)
                {
                    return CreatedAtAction("GetById", new { id = created.Id }, _mapper.Map<TeamInMatchVM>(created));
                }
                return BadRequest("Thêm đội bóng vào trận đấu thất bại");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update(TeamInMatchUM teamInMatch, string? room)
        {
            try
            {
                TeamInMatch team = await _teamInMatch.GetByIdAsync(teamInMatch.Id);
                if(team != null)
                {
                    team.TeamScore = teamInMatch.TeamScore;
                    team.TeamScoreLose = teamInMatch.TeamScoreLose;
                    team.YellowCardNumber = teamInMatch.YellowCardNumber;
                    team.RedCardNumber = teamInMatch.RedCardNumber;
                    team.TeamInTournamentId = teamInMatch.TeamInTournamentId;
                    team.Result = teamInMatch.Result;
                    team.NextTeam = teamInMatch.NextTeam;
                    team.TeamName = teamInMatch.TeamName;

                    bool isUpdated =await _teamInMatch.UpdateAsync(team);
                    if (isUpdated)
                    {
                        if (!String.IsNullOrEmpty(room))
                        {
                            TeamInMatch listTeam = _teamInMatch.GetList().Join(_matchService.GetList(), tim => tim.Match, m => m, (tim, m) => new { tim, m }).
                   Join(_teamInTournamentService.GetList(), titm => titm.tim.TeamInTournament, tit => tit, (titm, tit) => new { titm, tit }).
                   Join(_teamService.GetList(), timm => timm.tit.Team, team => team, (timm, team) => new TeamInMatch
                   {
                       Id = timm.titm.tim.Id,
                       TeamScore = timm.titm.tim.TeamScore,
                       TeamScoreLose = timm.titm.tim.TeamScoreLose,
                       YellowCardNumber = timm.titm.tim.YellowCardNumber,
                       RedCardNumber = timm.titm.tim.RedCardNumber,
                       TeamInTournamentId = timm.titm.tim.TeamInTournamentId,
                       TeamInTournament = new TeamInTournament
                       {
                           Id = timm.tit.Id,
                           Point = timm.tit.Point,
                           DifferentPoint = timm.tit.DifferentPoint,
                           Status = timm.tit.Status,
                           TournamentId = timm.tit.TournamentId,
                           Team = team
                       },
                       MatchId = timm.titm.tim.MatchId,
                       Result = timm.titm.tim.Result,
                       NextTeam = timm.titm.tim.NextTeam,
                       TeamName = timm.titm.tim.TeamName,
                       Match = timm.titm.tim.Match,
                   }).Where(m => m.Id == team.Id).FirstOrDefault()!;
                            await _hubContext.Clients.Group(room).SendAsync("TeamInMatch", _mapper.Map<TeamInMatchMT>(listTeam));
                        }
                        return Ok(new
                        {
                            message = "Cập nhật đội bóng trong trận đấu thành công"
                        });
                    }
                    return BadRequest("Cập nhật đội bóng trong trận đấu thất bại");
                }
                return NotFound("Không tìm thấy đội bóng trong trận đấu vơi id là " + teamInMatch.Id);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update auto team in match to tournament</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut("update-team-in-match-to-tournament")]
        [Produces("application/json")]
        public async Task<ActionResult> UpdateTeamInMatch([FromBody] TeamInMatchToTournamentUM model)
        {
            try
            {
                TeamInTournament currentTeamInTournament = await _teamInTournamentService.GetByIdAsync(model.TeamInTournamentId);
                if (currentTeamInTournament == null)
                {
                    return NotFound("Không tìm thấy đội bóng trong trận đấu");
                }
                Team team = await _teamService.GetByIdAsync(currentTeamInTournament.TeamId!.Value);
                int numberTeamInTournament = _teamInTournamentService.CountTeamInATournament(currentTeamInTournament.TournamentId!.Value);

                if (model.TypeUpdate)
                {
                    string groupName = "";
                    if (!String.IsNullOrEmpty(model.TeamIndex.ToString()) && model.TeamIndex > 0)
                    {
                        numberTeamInTournament = model.TeamIndex!.Value;
                    }
                    IQueryable<Match> listMatchInTournament = _matchService.GetList().Where(m => m.TournamentId == currentTeamInTournament.TournamentId)
                    .Join(_teamInMatch.GetList().Where(tim => tim.TeamName == "Đội " + numberTeamInTournament), m => m.Id, tim => tim.MatchId, (m, tim) => new Match
                    {
                        Id = m.Id,
                        MatchDate = m.MatchDate,
                        Status = m.Status,
                        TournamentId = m.TournamentId,
                        Round = m.Round,
                        Fight = m.Fight,
                        GroupFight = m.GroupFight,
                        TeamInMatches = m.TeamInMatches
                    });
                    groupName = listMatchInTournament.ToList()[0].GroupFight!;

                    foreach (Match match in listMatchInTournament.ToList())
                    {
                        foreach (TeamInMatch tim in match.TeamInMatches)
                        {
                            if (tim.TeamName == "Đội " + numberTeamInTournament)
                            {
                                tim.TeamName = team.TeamName;
                                tim.TeamInTournamentId = currentTeamInTournament.Id;
                                await _teamInMatch.UpdateAsync(tim);
                            }
                        }
                    }

                    currentTeamInTournament.GroupName = groupName;
                    await _teamInTournamentService.UpdateAsync(currentTeamInTournament);
                }
                else
                {
                    IQueryable<Match> listMatchInTournament = _matchService.GetList().Where(m => m.TournamentId == currentTeamInTournament.TournamentId)
                    .Join(_teamInMatch.GetList().Where(tim => tim.TeamName == team.TeamName), m => m.Id, tim => tim.MatchId, (m, tim) => new Match
                    {
                        Id = m.Id,
                        MatchDate = m.MatchDate,
                        Status = m.Status,
                        TournamentId = m.TournamentId,
                        Round = m.Round,
                        Fight = m.Fight,
                        GroupFight = m.GroupFight,
                        TeamInMatches = m.TeamInMatches
                    });

                    foreach (Match match in listMatchInTournament.ToList())
                    {
                        foreach (TeamInMatch tim in match.TeamInMatches)
                        {
                            if (tim.TeamName == team.TeamName)
                            {
                                tim.TeamName = "Đội " + numberTeamInTournament;
                                tim.TeamInTournamentId = null;
                                await _teamInMatch.UpdateAsync(tim);
                            }
                        }
                    }
                }

                return Ok("Thành công");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update next team in match to tournament</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut("update-next-team-in-match")]
        [Produces("application/json")]
        public async Task<ActionResult> UpdateNextTeamInMatch([FromBody] TeamInMatchNextUM model)
        {
            try
            {
                Tournament currentTournament = await _tournamentService.GetByIdAsync(model.TournamentId);
                if (currentTournament == null)
                {
                    return NotFound("Không tìm thấy giải đấu");
                }
                if (currentTournament.TournamentTypeId == 2)
                {
                    return BadRequest("Giải vòng tròn không có vòng trong");
                }
                if (currentTournament.TournamentTypeId == 1)
                {
                    TeamInMatch teamInMatchWin = _teamInMatch.GetList().Where(s => s.MatchId == model.MatchId && s.Result > 1).FirstOrDefault()!;
                    if (teamInMatchWin == null)
                    {
                        return BadRequest("Không có đội thắng");
                    }
                    
                    Match matchWin = await _matchService.GetByIdAsync(teamInMatchWin.MatchId!.Value);
                    TeamInMatch teamInMatchNext = _teamInMatch.GetList().Where(tim => tim.Match!.TournamentId == model.TournamentId && tim.NextTeam == "Thắng " + matchWin.Fight).FirstOrDefault()!;
                    if (teamInMatchNext != null)
                    {
                        teamInMatchNext.TeamName = teamInMatchWin.TeamName;
                        teamInMatchNext.TeamInTournamentId = teamInMatchWin.TeamInTournamentId;
                        await _teamInMatch.UpdateAsync(teamInMatchNext);
                    }
                    TeamInTournament teamInTourWin = await _teamInTournamentService.GetByIdAsync(teamInMatchWin.TeamInTournamentId!.Value);
                    teamInTourWin.StatusInTournament = "Trong giải";
                    await _teamInTournamentService.UpdateAsync(teamInTourWin);

                    TeamInMatch teamInMatchLose = _teamInMatch.GetList().Where(s => s.MatchId == model.MatchId && s.Result < 1).FirstOrDefault()!;
                    TeamInTournament teamInTourLose = await _teamInTournamentService.GetByIdAsync(teamInMatchLose.TeamInTournamentId!.Value);
                    teamInTourLose.StatusInTournament = "Bị loại";
                    await _teamInTournamentService.UpdateAsync(teamInTourLose);
                }
                else
                {
                    if(model.MatchId == 0)
                    {
                        IQueryable<TeamInTournament> listTeamInTournament = _teamInTournamentService.GetList().Where(s => s.TournamentId == model.TournamentId);
                        List<TeamInTournament> listTeamInTournamentA = listTeamInTournament.Where(s => s.GroupName == "Bảng A").OrderByDescending(o => o.Point).Take(2).ToList();
                        if (listTeamInTournamentA.Count == 2)
                        {
                            TeamInMatch teamInMatchNext1 = _teamInMatch.GetList().Where(tim => tim.Match!.TournamentId == model.TournamentId && tim.NextTeam == "Nhất bảng A").FirstOrDefault()!;
                            if (teamInMatchNext1 != null)
                            {
                                teamInMatchNext1.TeamName = listTeamInTournamentA[0].Team!.TeamName;
                                teamInMatchNext1.TeamInTournamentId = listTeamInTournamentA[0].Id;
                                await _teamInMatch.UpdateAsync(teamInMatchNext1);
                            }
                            TeamInTournament teamInTourTop1 = await _teamInTournamentService.GetByIdAsync(teamInMatchNext1!.TeamInTournamentId!.Value);
                            teamInTourTop1.StatusInTournament = "Trong giải";
                            await _teamInTournamentService.UpdateAsync(teamInTourTop1);

                            TeamInMatch teamInMatchNext2 = _teamInMatch.GetList().Where(tim => tim.Match!.TournamentId == model.TournamentId && tim.NextTeam == "Nhì bảng A").FirstOrDefault()!;
                            if (teamInMatchNext2 != null)
                            {
                                teamInMatchNext2.TeamName = listTeamInTournamentA[1].Team!.TeamName;
                                teamInMatchNext2.TeamInTournamentId = listTeamInTournamentA[1].Id;
                                await _teamInMatch.UpdateAsync(teamInMatchNext2);
                            }
                            TeamInTournament teamInTourTop2 = await _teamInTournamentService.GetByIdAsync(teamInMatchNext2!.TeamInTournamentId!.Value);
                            teamInTourTop2.StatusInTournament = "Trong giải";
                            await _teamInTournamentService.UpdateAsync(teamInTourTop2);
                        }
                        List<TeamInTournament> listTeamInTournamentAOut = listTeamInTournament.Where(s => s.GroupName == "Bảng A" && s.Id != listTeamInTournamentA[0].Id && s.Id != listTeamInTournamentA[1].Id).ToList();
                        foreach(TeamInTournament tit in listTeamInTournamentAOut)
                        {
                            tit.StatusInTournament = "Bị loại";
                            await _teamInTournamentService.UpdateAsync(tit);
                        }

                        List<TeamInTournament> listTeamInTournamentB = listTeamInTournament.Where(s => s.GroupName == "Bảng B").OrderByDescending(o => o.Point).Take(2).ToList();
                        if (listTeamInTournamentB.Count == 2)
                        {
                            TeamInMatch teamInMatchNext1 = _teamInMatch.GetList().Where(tim => tim.Match!.TournamentId == model.TournamentId && tim.NextTeam == "Nhất bảng B").FirstOrDefault()!;
                            if (teamInMatchNext1 != null)
                            {
                                teamInMatchNext1.TeamName = listTeamInTournamentB[0].Team!.TeamName;
                                teamInMatchNext1.TeamInTournamentId = listTeamInTournamentB[0].Id;
                                await _teamInMatch.UpdateAsync(teamInMatchNext1);
                            }
                            TeamInTournament teamInTourTop1 = await _teamInTournamentService.GetByIdAsync(teamInMatchNext1!.TeamInTournamentId!.Value);
                            teamInTourTop1.StatusInTournament = "Trong giải";
                            await _teamInTournamentService.UpdateAsync(teamInTourTop1);

                            TeamInMatch teamInMatchNext2 = _teamInMatch.GetList().Where(tim => tim.Match!.TournamentId == model.TournamentId && tim.NextTeam == "Nhì bảng B").FirstOrDefault()!;
                            if (teamInMatchNext2 != null)
                            {
                                teamInMatchNext2.TeamName = listTeamInTournamentB[1].Team!.TeamName;
                                teamInMatchNext2.TeamInTournamentId = listTeamInTournamentB[1].Id;
                                await _teamInMatch.UpdateAsync(teamInMatchNext2);
                            }
                            TeamInTournament teamInTourTop2 = await _teamInTournamentService.GetByIdAsync(teamInMatchNext2!.TeamInTournamentId!.Value);
                            teamInTourTop2.StatusInTournament = "Trong giải";
                            await _teamInTournamentService.UpdateAsync(teamInTourTop2);
                        }
                        List<TeamInTournament> listTeamInTournamentBOut = listTeamInTournament.Where(s => s.GroupName == "Bảng B" && s.Id != listTeamInTournamentB[0].Id && s.Id != listTeamInTournamentB[1].Id).ToList();
                        foreach (TeamInTournament tit in listTeamInTournamentBOut)
                        {
                            tit.StatusInTournament = "Bị loại";
                            await _teamInTournamentService.UpdateAsync(tit);
                        }

                        List<TeamInTournament> listTeamInTournamentC = listTeamInTournament.Where(s => s.GroupName == "Bảng C").OrderByDescending(o => o.Point).Take(2).ToList();
                        if (listTeamInTournamentC.Count == 2)
                        {
                            TeamInMatch teamInMatchNext1 = _teamInMatch.GetList().Where(tim => tim.Match!.TournamentId == model.TournamentId && tim.NextTeam == "Nhất bảng C").FirstOrDefault()!;
                            if (teamInMatchNext1 != null)
                            {
                                teamInMatchNext1.TeamName = listTeamInTournamentC[0].Team!.TeamName;
                                teamInMatchNext1.TeamInTournamentId = listTeamInTournamentC[0].Id;
                                await _teamInMatch.UpdateAsync(teamInMatchNext1);
                            }
                            TeamInTournament teamInTourTop1 = await _teamInTournamentService.GetByIdAsync(teamInMatchNext1!.TeamInTournamentId!.Value);
                            teamInTourTop1.StatusInTournament = "Trong giải";
                            await _teamInTournamentService.UpdateAsync(teamInTourTop1);

                            TeamInMatch teamInMatchNext2 = _teamInMatch.GetList().Where(tim => tim.Match!.TournamentId == model.TournamentId && tim.NextTeam == "Nhì bảng C").FirstOrDefault()!;
                            if (teamInMatchNext2 != null)
                            {
                                teamInMatchNext2.TeamName = listTeamInTournamentC[1].Team!.TeamName;
                                teamInMatchNext2.TeamInTournamentId = listTeamInTournamentC[1].Id;
                                await _teamInMatch.UpdateAsync(teamInMatchNext2);
                            }
                            TeamInTournament teamInTourTop2 = await _teamInTournamentService.GetByIdAsync(teamInMatchNext2!.TeamInTournamentId!.Value);
                            teamInTourTop2.StatusInTournament = "Trong giải";
                            await _teamInTournamentService.UpdateAsync(teamInTourTop2);
                        }
                        List<TeamInTournament> listTeamInTournamentCOut = listTeamInTournament.Where(s => s.GroupName == "Bảng C" && s.Id != listTeamInTournamentC[0].Id && s.Id != listTeamInTournamentC[1].Id).ToList();
                        foreach (TeamInTournament tit in listTeamInTournamentCOut)
                        {
                            tit.StatusInTournament = "Bị loại";
                            await _teamInTournamentService.UpdateAsync(tit);
                        }

                        List<TeamInTournament> listTeamInTournamentD = listTeamInTournament.Where(s => s.GroupName == "Bảng D").OrderByDescending(o => o.Point).Take(2).ToList();
                        if (listTeamInTournamentD.Count == 2)
                        {
                            TeamInMatch teamInMatchNext1 = _teamInMatch.GetList().Where(tim => tim.Match!.TournamentId == model.TournamentId && tim.NextTeam == "Nhất bảng D").FirstOrDefault()!;
                            if (teamInMatchNext1 != null)
                            {
                                teamInMatchNext1.TeamName = listTeamInTournamentD[0].Team!.TeamName;
                                teamInMatchNext1.TeamInTournamentId = listTeamInTournamentD[0].Id;
                                await _teamInMatch.UpdateAsync(teamInMatchNext1);
                            }
                            TeamInTournament teamInTourTop1 = await _teamInTournamentService.GetByIdAsync(teamInMatchNext1!.TeamInTournamentId!.Value);
                            teamInTourTop1.StatusInTournament = "Trong giải";
                            await _teamInTournamentService.UpdateAsync(teamInTourTop1);

                            TeamInMatch teamInMatchNext2 = _teamInMatch.GetList().Where(tim => tim.Match!.TournamentId == model.TournamentId && tim.NextTeam == "Nhì bảng D").FirstOrDefault()!;
                            if (teamInMatchNext2 != null)
                            {
                                teamInMatchNext2.TeamName = listTeamInTournamentD[1].Team!.TeamName;
                                teamInMatchNext2.TeamInTournamentId = listTeamInTournamentD[1].Id;
                                await _teamInMatch.UpdateAsync(teamInMatchNext2);
                            }
                            TeamInTournament teamInTourTop2 = await _teamInTournamentService.GetByIdAsync(teamInMatchNext2!.TeamInTournamentId!.Value);
                            teamInTourTop2.StatusInTournament = "Trong giải";
                            await _teamInTournamentService.UpdateAsync(teamInTourTop2);
                        }
                        List<TeamInTournament> listTeamInTournamentDOut = listTeamInTournament.Where(s => s.GroupName == "Bảng D" && s.Id != listTeamInTournamentD[0].Id && s.Id != listTeamInTournamentD[1].Id).ToList();
                        foreach (TeamInTournament tit in listTeamInTournamentDOut)
                        {
                            tit.StatusInTournament = "Bị loại";
                            await _teamInTournamentService.UpdateAsync(tit);
                        }
                    }
                    else
                    {
                        TeamInMatch teamInMatchWin = _teamInMatch.GetList().Where(s => s.MatchId == model.MatchId && s.Result > 1).FirstOrDefault()!;
                        if (teamInMatchWin == null)
                        {
                            return BadRequest("Không có đội thắng");
                        }
                        Match matchWin = await _matchService.GetByIdAsync(teamInMatchWin.MatchId!.Value);
                        TeamInMatch teamInMatchNext = _teamInMatch.GetList().Where(tim => tim.Match!.TournamentId == model.TournamentId && tim.NextTeam == "Thắng " + matchWin.Fight).FirstOrDefault()!;
                        if (teamInMatchNext != null)
                        {
                            teamInMatchNext.TeamName = teamInMatchWin.TeamName;
                            teamInMatchNext.TeamInTournamentId = teamInMatchWin.TeamInTournamentId;
                            await _teamInMatch.UpdateAsync(teamInMatchNext);
                        }
                        TeamInTournament teamInTourWin = await _teamInTournamentService.GetByIdAsync(teamInMatchWin.TeamInTournamentId!.Value);
                        teamInTourWin.StatusInTournament = "Trong giải";
                        await _teamInTournamentService.UpdateAsync(teamInTourWin);

                        TeamInMatch teamInMatchLose = _teamInMatch.GetList().Where(s => s.MatchId == model.MatchId && s.Result < 1).FirstOrDefault()!;
                        TeamInTournament teamInTourLose = await _teamInTournamentService.GetByIdAsync(teamInMatchLose.TeamInTournamentId!.Value);
                        teamInTourLose.StatusInTournament = "Bị loại";
                        await _teamInTournamentService.UpdateAsync(teamInTourLose);
                    }
                }
                return Ok("Thành công");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete team in match By Tournament Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("delete-by-tournament-id")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteTeamByTournamrntId(int tournamentId)
        {
            try
            {
                List<TeamInMatch> listTeamInMatch = _teamInMatch.GetList().Where(t => t.Match!.TournamentId == tournamentId).ToList();
                foreach (TeamInMatch teamInMatch in listTeamInMatch)
                {
                    await _teamInMatch.DeleteAsync(teamInMatch);
                }
                return Ok("Xóa đội bóng trong trận đấu thành công");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
