using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

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

        public TeamInMatchController(ITeamInMatchService teamInMatch,IMapper mapper, IMatchService matchService,ITournamentService tournamentService, ITeamService teamService, ITeamInTournamentService teamInTournamentService)
        {
            _teamInMatch = teamInMatch;
            _mapper = mapper;
            _matchService = matchService;
            _teamService = teamService;
            _tournamentService= tournamentService;
            _teamInTournamentService = teamInTournamentService;
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
    .GroupJoin(_teamService.GetList(), timm => timm.tim.TeamInTournament.Team, team => team, (timm, team) => new {timm,team})
    .SelectMany(t => t.team.DefaultIfEmpty(),(t, temp) => new TeamInMatch
    {
        Id = t.timm.tim.Id,
        TeamScore = t.timm.tim.TeamScore,
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
    }).Where(t => t.Match.TournamentId == tournamentId);
                return Ok(listTeam);
                if (fullInfo == true)
                {
                    listTeam = _teamInMatch.GetList().Join(_matchService.GetList(), tim => tim.Match, m => m, (tim, m) => new { tim, m })
                   .Join(_teamInTournamentService.GetList(), ttim => ttim.tim.TeamInTournament, te => te, (ttim, te) => new { ttim, te })
                   .Join(_tournamentService.GetList(), timt => timt.ttim.m.Tournament, t => t, (timt, t) => new TeamInMatch
                   {
                       Id = timt.ttim.tim.Id,
                       TeamScore = timt.ttim.tim.TeamScore,
                       YellowCardNumber = timt.ttim.tim.YellowCardNumber,
                       RedCardNumber = timt.ttim.tim.RedCardNumber,
                       TeamInTournamentId = timt.ttim.tim.TeamInTournamentId,
                       MatchId = timt.ttim.m.Id,
                       Result = timt.ttim.tim.Result,
                       NextTeam = timt.ttim.tim.NextTeam,
                       TeamName = timt.ttim.tim.TeamName,
                       Match = timt.ttim.m,
                       TeamInTournament = timt.te
                   }).Where(m => m.Match.TournamentId == tournamentId);
                }
                if(orderType == SortTypeEnum.DESC)
                {
                    listTeam = listTeam.OrderByDescending(t => t.Id);
                }
                var temInMatch = new List<TeamInMatch>();
                temInMatch = listTeam.ToList();
                if (temInMatch.Count() > 0)
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
                IQueryable<TeamInMatch> listTeam = _teamInMatch.GetList().Join(_matchService.GetList() ,tim => tim.Match, m => m, (tim, m) => new  {tim , m})
                    .Join(_teamInTournamentService.GetList(), timt => timt.tim.TeamInTournament, t => t, (timt,t) => new TeamInMatch
                    {
                        Id = timt.tim.Id,
                        TeamScore = timt.tim.TeamScore,
                        YellowCardNumber = timt.tim.YellowCardNumber,
                        RedCardNumber = timt.tim.RedCardNumber,
                        TeamInTournamentId =t.Id,
                        MatchId = timt.m.Id,
                        Result = timt.tim.Result,
                        NextTeam = timt.tim.NextTeam,
                        TeamName = timt.tim.TeamName,
                        Match = timt.m,
                        TeamInTournament = t

                    } ).Where(m => m.MatchId == matchId);
                var temInMatch = new List<TeamInMatch>();
                temInMatch = listTeam.ToList();
                if (temInMatch.Count()>0)
                {

                    var teamListResponse = new TeamInMatchMTLV
                    {

                        TeamsInMatch = _mapper.Map<List<TeamInMatch>, List<TeamInMatchMT>>(temInMatch)

                    };
                    return Ok(teamListResponse);

                }

                return NotFound("Không tìm thấy đội bóng trong trận đấu");

            }
            catch(Exception ex)
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
            TeamInMatch team = new TeamInMatch();
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
                team.YellowCardNumber = teamInMatch.YellowCardNumber;
                team.RedCardNumber = teamInMatch.RedCardNumber;
                team.TeamInTournamentId = teamInMatch.TeamInTournamentId;
                team.MatchId = teamInMatch.MatchId;
                team.Result = "";
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
        public async Task<ActionResult> Update(TeamInMatchUM teamInMatch)
        {
            try
            {
                TeamInMatch team = await _teamInMatch.GetByIdAsync(teamInMatch.Id);
                if(team != null)
                {
                    team.TeamScore = teamInMatch.TeamScore;    
                    team.YellowCardNumber = teamInMatch.YellowCardNumber;
                    team.RedCardNumber = teamInMatch.RedCardNumber;
                    team.TeamInTournamentId = teamInMatch.TeamInTournamentId;
                    team.Result = teamInMatch.Result;
                    team.NextTeam = teamInMatch.NextTeam;
                    team.TeamName = teamInMatch.TeamName;

                    bool isUpdated =await _teamInMatch.UpdateAsync(team);
                    if (isUpdated)
                    {
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

        /// <summary>Update a team in match to tournament</summary>
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
                Team team = await _teamService.GetByIdAsync(currentTeamInTournament.TeamId.Value);
                int numberTeamInTournament = _teamInTournamentService.CountTeamInATournament(currentTeamInTournament.TournamentId.Value);

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

                foreach (Match match in listMatchInTournament.ToList())
                {
                    foreach(TeamInMatch tim in match.TeamInMatches)
                    {
                        if(tim.TeamName == "Đội " + numberTeamInTournament)
                        {
                            tim.TeamName = team.TeamName;
                            tim.TeamInTournamentId = currentTeamInTournament.Id;
                            await _teamInMatch.UpdateAsync(tim);
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
    }
}
