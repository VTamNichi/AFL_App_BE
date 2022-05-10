using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PlayerInTeamController : ControllerBase
    {
        private readonly IPlayerInTeamService _playerInTeam;
        private readonly IFootballPlayerService _footballPlayerService;
        private readonly ITeamService _teamService;
        private readonly IMapper _mapper;
        public PlayerInTeamController(IPlayerInTeamService playerInTeamService , IFootballPlayerService footballPlayerService, ITeamService teamService , IMapper mapper)
        {
            _playerInTeam = playerInTeamService;
            _footballPlayerService = footballPlayerService;
            _teamService = teamService;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<PlayerInTeamVM> GetAllPlayerInTeam(int teamId, SortTypeEnum orderType, int pageIndex= 1, int limit = 5)
        {
            try
            {

            IQueryable<PlayerInTeam> playerList = _playerInTeam.GetList();
                playerList = playerList.Where(p => (int)p.TeamId == teamId);
                var playerListPaging = playerList.Skip((pageIndex - 1) * limit).Take(limit).ToList();
                var playerListOrder = playerListPaging;
                if (orderType == SortTypeEnum.DESC)
                {
                    playerListOrder = playerListPaging.OrderByDescending(p => p.Id).ToList();
                }
                var playerListResponse = new PlayerInTeamLV
                {
                    PlayerInTeams = _mapper.Map<List<PlayerInTeam>, List<PlayerInTeamVM>>(playerListOrder),
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
                        message = "Player Already In Team"
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
                return BadRequest();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
       [Route("api/v1/[controller]/id")]
        public async Task<ActionResult<PlayerInTeamVM>> GetPlayerInTeamById(int id)
        {
            try
            {
                PlayerInTeam player = await _playerInTeam.GetByIdAsync(id);
              if (player != null)
                {
                    return Ok(_mapper.Map<PlayerInTeamVM>(player));
                }
                return NotFound();
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
                            message = success
                        });
                    }
                }
                return BadRequest();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
