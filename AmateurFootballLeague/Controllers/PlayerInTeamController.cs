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
    public class PlayerInTeamController : ControllerBase
    {
        private readonly IPlayerInTeamService _playerInTeam;
        private readonly IMapper _mapper;
        private readonly IFootballPlayerService _footballPlayerService;
        private readonly ITeamService _teamService;
        private readonly IUserService _userService;
        private readonly IPlayerInTournamentService _playerInTournament;
        private readonly ITournamentService _tournamentService;
        private readonly ITeamInTournamentService _teamInTournamentService;

        public PlayerInTeamController(IPlayerInTeamService playerInTeamService, IMapper mapper , IFootballPlayerService footballPlayer, ITeamService teamService, IUserService userService
            ,IPlayerInTournamentService playerInTournamentService, ITournamentService tournamentService, ITeamInTournamentService teamInTournamentService)
        {
            _playerInTeam = playerInTeamService;
            _mapper = mapper;
            _footballPlayerService = footballPlayer;
            _teamService = teamService;
            _userService = userService;
            _playerInTournament = playerInTournamentService;
            _tournamentService = tournamentService;
            _teamInTournamentService = teamInTournamentService;
        }

        [HttpGet]
        public ActionResult<PlayerInTeamLFV> GetAllPlayerInTeam(int? teamId,int? footballPlayerId, string? name,string? status,string? busy, SortTypeEnum orderType, int pageIndex= 1, int limit = 5)
        {
            try
            {

                IQueryable<PlayerInTeam> playerList = _playerInTeam.GetList();

                if (!String.IsNullOrEmpty(teamId.ToString()))
                {
                    playerList = playerList.Join(_footballPlayerService.GetList(), pit => pit.FootballPlayer, p => p, (pit, p) => new PlayerInTeam
                    {
                        Id = pit.Id,
                        Status = pit.Status,
                        TeamId = pit.TeamId,
                        FootballPlayerId = p.Id,
                        FootballPlayer = p
                    }).Where(p => p.TeamId == teamId).Join(_userService.GetList(), fp => fp.FootballPlayer!.IdNavigation, u => u, (fp, u) => new PlayerInTeam
                    {
                        Id = fp.Id,
                        Status = fp.Status,
                        TeamId = fp.TeamId,
                        FootballPlayerId = fp.FootballPlayerId,
                        FootballPlayer = new FootballPlayer
                        {
                            Id = fp.FootballPlayer!.Id,
                            PlayerName = fp.FootballPlayer.PlayerName,
                            PlayerAvatar = fp.FootballPlayer.PlayerAvatar,
                            Position = fp.FootballPlayer.Position,
                            Description = fp.FootballPlayer.Description,
                            Status = fp.FootballPlayer.Status,
                            DateCreate = fp.FootballPlayer.DateCreate,
                            DateUpdate = fp.FootballPlayer.DateUpdate,
                            DateDelete = fp.FootballPlayer.DateDelete,
                            IdNavigation = u
                        }
                    });
                }
                DateTime date = DateTime.Now.AddHours(7);
                
                if (!String.IsNullOrEmpty(footballPlayerId.ToString()) && String.IsNullOrEmpty(teamId.ToString()))
                {
                    playerList = playerList.Join(_teamService.GetList(), pit => pit.Team, p => p, (pit, p) => new PlayerInTeam
                    {
                        Id = pit.Id,
                        Status = pit.Status,
                        TeamId = pit.TeamId,
                        FootballPlayerId = pit.FootballPlayerId,
                        Team = p
                    }).Where(p => p.FootballPlayerId == footballPlayerId);
                }
                if (!String.IsNullOrEmpty(footballPlayerId.ToString()) && !String.IsNullOrEmpty(teamId.ToString()))
                {
                    playerList = playerList.Where(p => p.FootballPlayerId == footballPlayerId);
                }

                if (!String.IsNullOrEmpty(name))
                {
                    playerList = playerList.Where(p => p.FootballPlayer!.PlayerName!.ToLower().Contains(name.Trim().ToLower()));
                }

                if (!String.IsNullOrEmpty(status))
                {
                    playerList = playerList.Where(p => p.Status!.ToLower() == status.ToLower());
                }
                var playerBusy = new List<PlayerInTeam>();
                var playerListFree = new List<PlayerInTeam>();
                var checkList = playerList.ToList();
                if (!String.IsNullOrEmpty(teamId.ToString()) && busy == "busy" && !String.IsNullOrEmpty(status))
                {
                    
                   
                    for (int i = 0; i < checkList.Count(); i++)
                    {
                        IQueryable<PlayerInTeam> busyList = _playerInTeam.GetList().Join(_footballPlayerService.GetList(), pit => pit.FootballPlayer, p => p, (pit, p) => new { pit, p }).Where(p => p.p.Id == checkList[i].FootballPlayerId).
                            Join(_playerInTournament.GetList(), pitt => pitt.pit.Id, pitour => pitour.PlayerInTeamId, (pitt, pitour) => new { pitt, pitour })
                            .Join(_teamInTournamentService.GetList(), pitt => pitt.pitour.TeamInTournament, tit => tit, (pitt, tit) => new { pitt, tit }).
                            Join(_tournamentService.GetList(), tit => tit.tit.Tournament, t => t, (tit, t) => new { tit, t }).Where(p => p.t.TournamentEndDate > date && p.t.Status == true).
                            Join(_userService.GetList(), p => p.tit.pitt.pitt.p.IdNavigation, u => u, (p, u) => new PlayerInTeam
                            {
                                Id = p.tit.pitt.pitt.pit.Id,
                                Status = p.tit.pitt.pitt.pit.Status,
                                TeamId = p.tit.pitt.pitt.pit.TeamId,
                                FootballPlayerId = p.tit.pitt.pitt.pit.FootballPlayerId,
                                FootballPlayer = new FootballPlayer
                                {
                                    Id = p.tit.pitt.pitt.p.Id,
                                    PlayerName = p.tit.pitt.pitt.p.PlayerName,
                                    PlayerAvatar = p.tit.pitt.pitt.p.PlayerAvatar,
                                    Position = p.tit.pitt.pitt.p.Position,
                                    Description = p.tit.pitt.pitt.p.Description,
                                    Status = p.tit.pitt.pitt.p.Status,
                                    DateCreate = p.tit.pitt.pitt.p.DateCreate,
                                    DateUpdate = p.tit.pitt.pitt.p.DateUpdate,
                                    DateDelete = p.tit.pitt.pitt.p.DateDelete,
                                    IdNavigation = u
                                },

                            });
                        if (busyList.Count() > 0)
                        {
                            playerBusy.Add(checkList[i]);
                        }
                    }
                        

                    var playerListBusy = new PlayerInTeamLFV
                    {
                        PlayerInTeamsFull = _mapper.Map<List<PlayerInTeam>, List<PlayerInTeamFVM>>(playerBusy),
                    };
                    return Ok(playerListBusy);
                }

                    //else
                    //{
                    //    playerList = playerList.Where(p => (int)p.TeamId == teamId);
                    //}

                    if (orderType == SortTypeEnum.DESC)
                {
                    playerList = playerList.OrderByDescending(p => p.Id);
                }
                int CountList = playerList.Count();

                var playerListPaging = playerList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var playerListFull = new PlayerInTeamLFV
                    {
                        PlayerInTeamsFull = _mapper.Map<List<PlayerInTeam>, List<PlayerInTeamFVM>>(playerListPaging),
                        CountList = CountList,
                        CurrentPage = pageIndex,
                        Size = limit
                    };
                    return Ok(playerListFull);


                //var playerListResponse = new PlayerInTeamLV
                //{
                //    PlayerInTeams = _mapper.Map<List<PlayerInTeam>, List<PlayerInTeamVM>>(playerListOrder),
                //    CountList = CountList,
                //    CurrentPage = pageIndex,
                //    Size = limit
                //};

                //return Ok(playerListResponse);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<ActionResult<PlayerInTeamVM>> AddPlayerToTeam(PlayerInTeamCM player)
        {
            PlayerInTeam pInTeam = new();
            try
            {
                PlayerInTeam checkPlayer = _playerInTeam.GetList().Where(p => p.TeamId!.Value == player.TeamId && p.FootballPlayerId == player.FootballPlayerId).FirstOrDefault()!;
                if (checkPlayer != null)
                {
                    return BadRequest(new
                    {
                        message = "Cầu thủ đã có đội bóng"
                    });
                }
                pInTeam.Status = player.Status;
                pInTeam.TeamId = player.TeamId;
                pInTeam.FootballPlayerId = player.FootballPlayerId;
                PlayerInTeam playerInTeamCreated = await _playerInTeam.AddAsync(pInTeam);
                if(playerInTeamCreated!= null)
                {
                    return CreatedAtAction("GetPlayerInTeamById", new { id = playerInTeamCreated.Id }, _mapper.Map<PlayerInTeamVM>(playerInTeamCreated));
                }
                return BadRequest("Thêm cầu thủ trông đội bóng thất bại");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<PlayerInTeamVM>> GetPlayerInTeamById(int id)
        {
            try
            {
                PlayerInTeam player = await _playerInTeam.GetByIdAsync(id);
              if (player != null)
                {
                    return Ok(_mapper.Map<PlayerInTeamVM>(player));
                }
                return NotFound("Không tìm thấy cầu thủ trong đội bóng với id là " + id);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<ActionResult> ChangeStatusPlayerInTeam(int Id, string status)
        {
            try
            {
                PlayerInTeam player = _playerInTeam.GetList().Where(p => p.Id == Id).FirstOrDefault()!;
                if(player != null)
                {
                    player.Status = status;
                    bool success  = await _playerInTeam.UpdateAsync(player);
                    if (success)
                    {
                        return Ok(new
                        {
                            message = "Thay đổi trạng thái cầu thủ trong đội bóng thành công"
                        });
                    }
                }
                return NotFound("Không tìm thấy cầu thủ trong đội bóng với id là " + Id);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            PlayerInTeam player = await _playerInTeam.GetByIdAsync(id);
            if (player == null)
            {
                return NotFound(new
                {
                    message = "Không thể tìm thấy cầu thủ trong đội bóng"
                });
            }
            try
            {
                bool isDeleted = await _playerInTeam.DeleteAsync(player);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Xóa cầu thủ trong đội bóng thành công"
                    });
                }
                return BadRequest("Xóa cầu thủ trong đội bóng thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
