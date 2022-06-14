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
        public ActionResult<PlayerInTournamentLV> GetAllPlayerInTournament(
                [FromQuery(Name = "team-in-tournament-id")] int? teamInTourId,
                [FromQuery(Name = "player-in-team-id")] int? playerInTeamId,
                [FromQuery(Name = "status")] string? status,
                [FromQuery(Name = "clothes-number-min")] int? clothesNumberMin,
                [FromQuery(Name = "clothes-number-max")] int? clothesNumberMax,
                [FromQuery(Name = "order-type")] SortTypeEnum orderType,
                [FromQuery(Name = "page-offset")] int pageIndex = 1,
                int limit = 5
            )
        {
            try
            {
                IQueryable<PlayerInTournament> playerList = _playerInTournament.GetList();
                if (!String.IsNullOrEmpty(teamInTourId.ToString()))
                {
                    playerList = playerList.Where(s => s.TeamInTournamentId == teamInTourId);
                }
                if (!String.IsNullOrEmpty(playerInTeamId.ToString()))
                {
                    playerList = playerList.Where(s => s.PlayerInTeamId == playerInTeamId);
                }
                if (!String.IsNullOrEmpty(status))
                {
                    playerList = playerList.Where(s => s.Status == status);
                }
                if (!String.IsNullOrEmpty(clothesNumberMin.ToString()))
                {
                    playerList = playerList.Where(s => s.ClothesNumber >= clothesNumberMin);
                }
                if (!String.IsNullOrEmpty(clothesNumberMax.ToString()))
                {
                    playerList = playerList.Where(s => s.ClothesNumber <= clothesNumberMax);
                }

                playerList = playerList.OrderBy(p => p.Id);
                if (orderType == SortTypeEnum.DESC)
                {
                    playerList = playerList.OrderByDescending(p => p.Id);
                }

                int countList = playerList.Count();

                var playerListPaging = playerList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var playerListResponse = new PlayerInTournamentLV
                {
                    PlayerInTournaments = _mapper.Map<List<PlayerInTournament>, List<PlayerInTournamentVM>>(playerListPaging),
                    CountList = countList,
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
                        message = "Cầu thủ đã tồn tại trong giải đấu"
                    });

                }
                //PlayerInTournament checkPlayerFromAnotherTeam = _playerInTournament.GetList().Where(p => (int)p.TeamInTournamentId != player.TeamInTournamentId && p.PlayerInTeamId == player.PlayerInTeamId).FirstOrDefault();
                //if(checkPlayerFromAnotherTeam != null)
                //{
                //    return BadRequest(new
                //    {
                //        message = "Cầu thủ đang trong một đội bóng khác"
                //    });
                //}
                playerCreate.TeamInTournamentId = player.TeamInTournamentId;
                playerCreate.PlayerInTeamId = player.PlayerInTeamId;
                playerCreate.ClothesNumber = String.IsNullOrEmpty(player.ClothesNumber.ToString()) ? 0 : player.ClothesNumber; 
                PlayerInTournament playerCreatedSuccess = await _playerInTournament.AddAsync(playerCreate);
                if(playerCreatedSuccess != null)
                {
                    return CreatedAtAction("GetById", new { id = playerCreatedSuccess.Id }, _mapper.Map<PlayerInTournamentVM>(playerCreatedSuccess));
                }
                return BadRequest("Tạo cầu thủ trong giải đấu thất bại");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet] 
        [Route("id")]
        public async Task<ActionResult<PlayerInTournamentVM>> GetById(int id)
        {
            try
            {
                PlayerInTournament player =await _playerInTournament.GetByIdAsync(id);
                if(player != null)
                {
                    return Ok(_mapper.Map<PlayerInTournamentVM>(player));
                }
                return NotFound("Không tìm thấy cầu thủ trong giải đấu với id là " + id);
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
                            message = "Xóa cầu thủ trong giải đấu thành công"
                        });
                    }
                    else
                    {
                        return BadRequest("Xóa cầu thủ trong giải đấu thất bại");
                    }
                }
                return NotFound("Không tìm thấy cầu thủ trong giải đấu với id là " + id);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
