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
        public PlayerInTeamController(IPlayerInTeamService playerInTeamService, IMapper mapper , IFootballPlayerService footballPlayer, ITeamService teamService)
        {
            _playerInTeam = playerInTeamService;
            _mapper = mapper;
            _footballPlayerService = footballPlayer;
            _teamService = teamService;
        }

        [HttpGet]
        public ActionResult<PlayerInTeamLFV> GetAllPlayerInTeam(int? teamId,int? footballPlayerId, string? name,string? status, SortTypeEnum orderType, int pageIndex= 1, int limit = 5)
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
                    }).Where(p => p.TeamId == teamId);
                }
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
                    playerList = playerList.Where(p => p.FootballPlayer.PlayerName.ToLower().Contains(name.Trim().ToLower()));
                }

                if (!String.IsNullOrEmpty(status))
                {
                    playerList = playerList.Where(p => p.Status.ToLower() == status.ToLower());
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
