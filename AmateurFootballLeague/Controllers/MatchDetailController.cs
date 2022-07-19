using AmateurFootballLeague.Hubs;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MatchDetailController : ControllerBase
    {
        private readonly IMatchDetailService _matchDetail;
        private readonly IMapper _mapper;
        private readonly IPlayerInTournamentService _playerInTournament;
        private readonly ITeamInTournamentService _teamInTournamentService;
        private readonly IPlayerInTeamService _playerInTeamService;
        private readonly IFootballPlayerService _footballPlayerService;
        private readonly IMatchService _matchService;
        private readonly IActionMatchService _actionMatchService;
        private readonly IHubContext<CommentHub> _hubContext;

        public MatchDetailController(IMatchDetailService matchDetail, IMapper mapper, IPlayerInTournamentService playerInTournament,
            ITeamInTournamentService teamInTournamentService, IPlayerInTeamService playerInTeamService, IFootballPlayerService footballPlayerService, IMatchService matchService, IActionMatchService actionMatchService,
            IHubContext<CommentHub> hubContext)
        {
            _matchDetail = matchDetail;
            _mapper = mapper;
            _playerInTournament = playerInTournament;
            _teamInTournamentService = teamInTournamentService;
            _playerInTeamService = playerInTeamService;
            _footballPlayerService = footballPlayerService;
            _matchService = matchService;
            _actionMatchService = actionMatchService;
            _hubContext = hubContext;
        }

        [HttpGet]
        [Route("MatchId")]
        public async Task<ActionResult<MatchDetailFVM>> GetMatchDetailByMatch(int matchId)
        {
            try
            {
                if (matchId == 0)
                {
                    IQueryable<MatchDetail> match = _matchDetail.GetList();
                    return Ok(match);
                }
                IQueryable<MatchDetail> listDtMatch = _matchDetail.GetList()
                    .Join(_footballPlayerService.GetList(), md => md.FootballPlayer, f => f, (md, f) => new MatchDetail
                    {
                        Id = md.Id,
                        ActionMatchId = md.ActionMatchId,
                        ActionMinute =md.ActionMinute,
                        MatchId = md.MatchId,
                        PlayerInTournamentId = md.PlayerInTournamentId,
                        FootballPlayerId = f.Id,
                        FootballPlayer = new FootballPlayer
                                {
                                    Id = f.Id,
                                    PlayerName = f.PlayerName,
                                    PlayerAvatar = f.PlayerAvatar,
                                    Position = f.Position
                                }                            
                    }).Where(m => m.MatchId == matchId);
                var matchDt = new List<MatchDetail>();
                matchDt = listDtMatch.ToList();
                if (matchDt.Count > 0)
                {

                    var matchDtListResponse = new MatchDetailFLV
                    {

                        MatchDetails = _mapper.Map<List<MatchDetail>, List<MatchDetailFVM>>(matchDt)

                    };
                    return Ok(matchDtListResponse);

                }

                return NotFound("Không tìm thấy chi tiết trận đấu");

            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }

        [HttpGet]
        [Route("Id")]
        public async Task<ActionResult<MatchDetailVM>> FindById(int id)
        {
            try
            {
                MatchDetail match = await _matchDetail.GetByIdAsync(id);
                if (match == null)
                {
                    return NotFound("Không tìm thấy chi tiết trận đấu");
                }
                return Ok(_mapper.Map<MatchDetailVM>(match));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<ActionResult<MatchDetailVM>> CreateMatchDetail(MatchDetailCM match , string? room)
        {
            MatchDetail matchDetail = new();
            try
            {
                Match crrMatch = await _matchService.GetByIdAsync(match.MatchId);
                if (crrMatch == null)
                {
                    return NotFound("Không tìm thấy trận đấu");
                }
                PlayerInTournament pitour = await _playerInTournament.GetByIdAsync(match.PlayerInTournamentId);
                if (pitour == null)
                {
                    return NotFound("Không tìm thấy cầu thủ trong giải đấu");
                }
                ActionMatch actionM = await _actionMatchService.GetByIdAsync(match.ActionMatchId);
                if (actionM == null)
                {
                    return NotFound("Không tìm thấy loại hành động");
                }
                matchDetail.ActionMatchId = match.ActionMatchId;
                matchDetail.ActionMinute = String.IsNullOrEmpty(match.ActionMinute) ? "" : match.ActionMinute;
                matchDetail.MatchId = match.MatchId;
                matchDetail.PlayerInTournamentId = match.PlayerInTournamentId;
                matchDetail.FootballPlayerId = match.FootballPlayerId;
                MatchDetail created = await _matchDetail.AddAsync(matchDetail);
                if (created != null)
                {
                   if (!String.IsNullOrEmpty(room))
                   {
                       MatchDetail dtMatch = _matchDetail.GetList()
                   .Join(_footballPlayerService.GetList(), md => md.FootballPlayer, f => f, (md, f) => new MatchDetail
                   {
                       Id = md.Id,
                       ActionMatchId = md.ActionMatchId,
                       ActionMinute = md.ActionMinute,
                       MatchId = md.MatchId,
                       PlayerInTournamentId = md.PlayerInTournamentId,
                       FootballPlayerId = f.Id,
                       FootballPlayer = new FootballPlayer
                               {
                                   Id = f.Id,
                                   PlayerName = f.PlayerName,
                                   PlayerAvatar = f.PlayerAvatar,
                                   Position = f.Position
                               }
                   }).Where(m => m.Id == created.Id).FirstOrDefault();

                       await _hubContext.Clients.Group(room).SendAsync("MatchDetail", _mapper.Map<MatchDetailFVM>(dtMatch));
                   }
                   return CreatedAtAction("FindById", new { id = created.Id }, _mapper.Map<MatchDetailVM>(created));
                }

                return BadRequest("Tạo chi tiết trận đấu thất bại");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<ActionResult<MatchDetailVM>> UpdateMatchDetail(MatchDetailUM match)
        {
            try
            {
                MatchDetail matchDetail = await _matchDetail.GetByIdAsync(match.Id);
                if (matchDetail != null)
                {
                    ActionMatch actionM = await _actionMatchService.GetByIdAsync(match.ActionMatchId);
                    if (actionM == null)
                    {
                        return NotFound("Không tìm thấy loại hành động");
                    }
                    matchDetail.ActionMatchId = match.ActionMatchId;
                    matchDetail.ActionMinute = String.IsNullOrEmpty(matchDetail.ActionMinute) ? "" : match.ActionMinute;
                    bool isUpdated = await _matchDetail.UpdateAsync(matchDetail);
                    if (isUpdated)
                    {
                        return Ok(new
                        {
                            message = "Cập nhập chi tiết trận đấu thành công"
                        });
                    }
                    return BadRequest("Cập nhật chi tiết trận đấu thất bại");
                }
                return NotFound("Không tìm thấy chi tiết trận đấu");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteMatchDetail(int id)
        {
            try
            {
                MatchDetail matchDetail = await _matchDetail.GetByIdAsync(id);
                if (matchDetail != null)
                {
                    bool isDeleted = await _matchDetail.DeleteAsync(matchDetail);
                    if (isDeleted)
                    {
                        return Ok(new
                        {
                            message = "Xóa chi tiết trận đấu thành công"
                        });
                    }
                    return BadRequest("Xóa chi tiết trận đấu thất bại");
                }
                return NotFound("Không tìm thấy chi tiết trận đấu");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpDelete("delete-type")]
        public async Task<ActionResult> DeleteMatchDetailByType(int matchId, DeleteType type)
        {
            try
            {
                IQueryable<MatchDetail> listMatchDetail = _matchDetail.GetList().Where(md => md.MatchId == matchId);

                if (type == DeleteType.score)
                {
                    listMatchDetail = listMatchDetail.Where(md => md.ActionMatchId == 1);
                }
                else if (type == DeleteType.yellow)
                {
                    listMatchDetail = listMatchDetail.Where(md => md.ActionMatchId == 2);
                }
                else
                {
                    listMatchDetail = listMatchDetail.Where(md => md.ActionMatchId == 3);
                }
                if (listMatchDetail.Any())
                {
                    foreach (MatchDetail matchDetail in listMatchDetail.ToList())
                    {
                        await _matchDetail.DeleteAsync(matchDetail);
                    }
                    return Ok(new
                    {
                        message = "Xóa chi tiết trận đấu thành công"
                    });
                }
                return NotFound("Không tìm thấy chi tiết trận đấu");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
