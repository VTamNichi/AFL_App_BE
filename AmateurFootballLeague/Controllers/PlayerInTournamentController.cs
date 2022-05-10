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
    public class PlayerInTournamentController : ControllerBase
    {
        private readonly IPlayerInTournamentService _playerInTournament;
        private readonly IMapper _mapper;
        public PlayerInTournamentController(IPlayerInTournamentService playerInTournament, IMapper mapper)
        {
            _playerInTournament = playerInTournament;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<PlayerInTeamVM> GetAllPlayerInTournament(SortTypeEnum orderType, int pageIndex = 1, int limit = 5)
        {
            try
            {
                IQueryable<PlayerInTournament> playerList = _playerInTournament.GetList();
                playerList = playerList.Select(p => p);
                var playerListPaging = playerList.Skip((pageIndex - 1) * limit).Take(limit).ToList();
                var playerListOrder = playerListPaging;
                if (orderType == SortTypeEnum.DESC)
                {
                    playerListOrder = playerListPaging.OrderByDescending(p => p.Id).ToList();
                }
                var playerListResponse = new PlayerInTournamentLV
                {
                    PlayerInTournaments = _mapper.Map<List<PlayerInTournament>, List<PlayerInTournamentVM>>(playerListOrder),
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
        public async Task<ActionResult<PlayerInTournamentVM>> CreatePlayerInTournament(PlayerInTournamentCM player)
        {
            PlayerInTournament playerCreate = new PlayerInTournament();
            try
            {
                PlayerInTournament checkPlayer = _playerInTournament.GetList().Where(p => (int)p.TeamInTournamentId == player.TeamInTournamentId && p.PlayerInTeamId == player.PlayerInTeamId).FirstOrDefault();
                if(checkPlayer != null)
                {
                    return BadRequest(new
                    {
                        message = "Player exist in this team"
                    });

                }
                PlayerInTournament checkPlayerFromAnotherTeam = _playerInTournament.GetList().Where(p => (int)p.TeamInTournamentId != player.TeamInTournamentId && p.PlayerInTeamId == player.PlayerInTeamId).FirstOrDefault();
                if(checkPlayerFromAnotherTeam != null)
                {
                    return BadRequest(new
                    {
                        message = "Player in another team"
                    });
                }
                playerCreate.TeamInTournamentId = player.TeamInTournamentId;
                playerCreate.PlayerInTeamId = player.PlayerInTeamId;
                PlayerInTournament playerCreatedSuccess = await _playerInTournament.AddAsync(playerCreate);
                if(playerCreatedSuccess != null)
                {
                    return CreatedAtAction("GetById", new { id = playerCreatedSuccess.Id }, _mapper.Map<PlayerInTournamentVM>(playerCreatedSuccess));
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
        public async Task<ActionResult<PlayerInTournamentVM>> GetById(int id)
        {
            try
            {
                PlayerInTournament player =await _playerInTournament.GetByIdAsync(id);
                if(player != null)
                {
                    return Ok(_mapper.Map<PlayerInTournamentVM>(player));
                }
                return NotFound();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteById(int id)
        {
            try
            {
                PlayerInTournament player = await _playerInTournament.GetByIdAsync(id);
                if(player != null)
                {
                    bool success = await _playerInTournament.DeleteAsync(player);
                    if (success)
                    {
                        return Ok(new
                        {
                            message = "Delete Success"
                        });
                    }
                    else
                    {
                        return BadRequest();
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
