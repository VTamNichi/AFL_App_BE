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
        public PlayerInTeamController(IPlayerInTeamService playerInTeamService, IMapper mapper , IFootballPlayerService footballPlayer)
        {
            _playerInTeam = playerInTeamService;
            _mapper = mapper;
            _footballPlayerService = footballPlayer;
        }

        [HttpGet]
        public ActionResult<PlayerInTeamLV> GetAllPlayerInTeam(int teamId,string? name, SortTypeEnum orderType, int pageIndex= 1, int limit = 5)
        {
            try
            {

            IQueryable<PlayerInTeam> playerList = _playerInTeam.GetList();
                
                if (!String.IsNullOrEmpty(name))
                {
                    playerList = playerList.Join(_footballPlayerService.GetList(), pit => pit.FootballPlayer, p => p, (pit, p) => new PlayerInTeam
                    {
                        Id = pit.Id,
                        Status = pit.Status,
                        TeamId = pit.TeamId,
                        FootballPlayerId = p.Id,
                        FootballPlayer = p
                    }).Where(p =>p.TeamId == teamId && p.FootballPlayer.PlayerName.ToLower().Contains(name.Trim().ToLower()));
                }
                else
                {
                    playerList = playerList.Where(p => (int)p.TeamId == teamId);
                }
                var playerListPaging = playerList.Skip((pageIndex - 1) * limit).Take(limit).ToList();
                var playerListOrder = playerListPaging;
                if (orderType == SortTypeEnum.DESC)
                {
                    playerListOrder = playerListPaging.OrderByDescending(p => p.Id).ToList();
                }
                int CountList = playerList.Count();
                if (!String.IsNullOrEmpty(name))
                {
                    var playerListFull = new PlayerInTeamLFV
                    {
                        PlayerInTeamsFull = _mapper.Map<List<PlayerInTeam>, List<PlayerInTeamFVM>>(playerListOrder),
                        CountList = CountList,
                        CurrentPage = pageIndex,
                        Size = limit
                    };
                    return Ok(playerListFull);
                    
                }
                var playerListResponse = new PlayerInTeamLV
                {
                    PlayerInTeams = _mapper.Map<List<PlayerInTeam>, List<PlayerInTeamVM>>(playerListOrder),
                    CountList = CountList,
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(playerListResponse);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public async Task<ActionResult<PlayerInTeamVM>> AddPlayerToTeam(PlayerInTeamCM player)
        {
            PlayerInTeam pInTeam = new PlayerInTeam();
            try
            {
                PlayerInTeam checkPlayer = _playerInTeam.GetList().Where(p => (int)p.TeamId == player.TeamId && p.FootballPlayerId == player.FootballPlayerId).FirstOrDefault();
                if (checkPlayer != null)
                {
                    return BadRequest(new
                    {
                        message = "Cầu thủ đã có đội bóng"
                    });
                }
                pInTeam.Status = "";
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
        public async Task<ActionResult> changeStatusPlayerInTeam(int Id, string status)
        {
            try
            {
                PlayerInTeam player = _playerInTeam.GetList().Where(p => p.Id == Id).FirstOrDefault();
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
    }
}
