using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
//using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamInMatchController : ControllerBase
    {
        private readonly ITeamInMatchService _teamInMatch;
        private readonly IMapper _mapper;
        private readonly IMatchService _matchService;
        private readonly ITeamService _teamService;
        private readonly ITournamentService _tournamentService;

        public TeamInMatchController(ITeamInMatchService teamInMatch,IMapper mapper, IMatchService matchService,ITournamentService tournamentService, ITeamService teamService)
        {
            _teamInMatch = teamInMatch;
            _mapper = mapper;
            _matchService = matchService;
            _teamService = teamService;
            _tournamentService= tournamentService;

        }

        [HttpGet]
        public ActionResult<TeamInMatchMT> GetAllTeamInMatchInTournament(int tournamentId)
        {
            try
            {
                IQueryable<TeamInMatch> listTeam = _teamInMatch.GetList().Join(_matchService.GetList(), tim => tim.Match, m => m, (tim, m) => new { tim, m })
                    .Join(_tournamentService.GetList(), timt => timt.m.Tournament, t => t, (timt, t) => new TeamInMatch
                    {
                        Id = timt.tim.Id,
                        TeamScore = timt.tim.TeamScore,
                        YellowCardNumber = timt.tim.YellowCardNumber,
                        RedCardNumber = timt.tim.RedCardNumber,
                        TeamId = t.Id,
                        MatchId = timt.m.Id,
                        Result = timt.tim.Result,
                        NextTeam = timt.tim.NextTeam,
                        TeamName = timt.tim.TeamName,
                        Match = timt.m

                    }).Where(m => m.Match.TournamentId == tournamentId);
                var temInMatch = new List<TeamInMatch>();
                temInMatch = listTeam.ToList();
                if (temInMatch.Count() > 0)
                {

                    var teamListResponse = new TeamInMatchMTLV
                    {

                        TeamsInMatch = _mapper.Map<List<TeamInMatch>, List<TeamInMatchMT>>(temInMatch)

                    };
                    return Ok(teamListResponse);

                }

                return NotFound("Không tìm thấy đội bóng trong trận đấu");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("matchId")]
        public ActionResult<TeamInMatchMT> GetAllTeamInMatch(int matchId)
        {
            try
            {
                IQueryable<TeamInMatch> listTeam = _teamInMatch.GetList().Join(_matchService.GetList() ,tim => tim.Match, m => m, (tim, m) => new  {tim , m})
                    .Join(_teamService.GetList(), timt => timt.tim.Team, t => t, (timt,t) => new TeamInMatch
                    {
                        Id = timt.tim.Id,
                        TeamScore = timt.tim.TeamScore,
                        YellowCardNumber = timt.tim.YellowCardNumber,
                        RedCardNumber = timt.tim.RedCardNumber,
                        TeamId =t.Id,
                        MatchId = timt.m.Id,
                        Result = timt.tim.Result,
                        NextTeam = timt.tim.NextTeam,
                        TeamName = timt.tim.TeamName,
                        Match = timt.m,
                        Team = t

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
                var checkTeam = _teamInMatch.GetList().Where(t=> t.MatchId == teamInMatch.MatchId && t.TeamId == teamInMatch.TeamId);
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
                team.TeamId = teamInMatch.TeamId;
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
                    team.TeamId = teamInMatch.TeamId;
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

    }
}
