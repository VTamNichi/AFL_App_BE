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
        public readonly IPlayerInTeamService _playerInTeam;
        public readonly IFootballPlayerService _footballPlayerService;
        private readonly ITournamentService _tournamentService;
        private readonly IMapper _mapper;
        public PlayerInTournamentController(IPlayerInTournamentService playerInTournament,IPlayerInTeamService playerInTeam, 
            IFootballPlayerService footballPlayerService,ITournamentService tournamentService, IMapper mapper)
        {
            _playerInTournament = playerInTournament;
            _playerInTeam = playerInTeam;
            _footballPlayerService = footballPlayerService;
            _tournamentService = tournamentService;
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
                    playerList = playerList.Join(_playerInTeam.GetList(), pit => pit.PlayerInTeam, t => t, (pit, t) => new { pit, t }).
                        Join(_footballPlayerService.GetList(), tf => tf.t.FootballPlayer, f => f, (tf, f) => new { tf, f }).
                        Select(p => new PlayerInTournament
                        {
                            Id = p.tf.pit.Id,
                            Status = p.tf.pit.Status,
                            ClothesNumber = p.tf.pit.ClothesNumber,
                            TeamInTournamentId = p.tf.pit.TeamInTournamentId,
                            PlayerInTeamId = p.tf.pit.PlayerInTeamId,
                            PlayerInTeam = new PlayerInTeam
                            {
                                Id = p.tf.t.Id,
                                Status = p.tf.t.Status,
                                TeamId = p.tf.t.TeamId,
                                FootballPlayerId = p.tf.t.FootballPlayerId,
                                FootballPlayer = new FootballPlayer
                                {
                                    Id = p.f.Id,
                                    PlayerName= p.f.PlayerName, 
                                    PlayerAvatar = p.f.PlayerAvatar,
                                    Position = p.f.Position
                                }
                            }
                        }).
                        Where(s => s.TeamInTournamentId == teamInTourId);
                    int count = playerList.Count();
                    var listFullPlayer = playerList.ToList();
                    var playerListFullResponse = new PlayerInTournamentLFV
                    {
                        PlayerInTournaments = _mapper.Map<List<PlayerInTournament>, List<PlayerInTournamentFVM>>(listFullPlayer),
                        CountList = count
                    };
                    return Ok(playerListFullResponse);
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
        public async Task<ActionResult<PlayerInTournamentVM>> CreatePlayerInTournament(PlayerInTournamentCM player, int footballPlayerId)
        {
            PlayerInTournament playerCreate = new();
            try
            {

                DateTime date = DateTime.Now.AddHours(7);
                PlayerInTournament checkPlayer = _playerInTournament.GetList().Where(p => p.TeamInTournamentId! == player.TeamInTournamentId && p.PlayerInTeamId == player.PlayerInTeamId).FirstOrDefault()!;

                if(checkPlayer != null)
                {
                    return BadRequest(new
                    {
                        message = "Cầu thủ đã tồn tại trong giải đấu"
                    });

                }
                //                 IQueryable<PlayerInTournament> checkPlayerFromAnotherTeam = _playerInTournament.GetList().Join(_playerInTeamService.GetList(), ptour => ptour.PlayerInTeam, pteam => pteam,
                //                     (ptour, pteam) => new { ptour, pteam }).Where(pit => pit.pteam.FootballPlayerId == footballPlayerId).Join(_footballPlayerService.GetList(), ptf => ptf.pteam.FootballPlayer, f => f, (ptf, f) => new { ptf, f })
                //                     .Join(_teamInTournamentService.GetList(), ptt => ptt.ptf.ptour.TeamInTournament, titour => titour, (ptt, titour) => new { ptt, titour }).Where(t => t.titour.StatusInTournament!= "Bị loại").
                //                     .Join(_tournamentService.GetList(), titt => titt.titour.Tournament, t => t, (titt, t) => new { titt, t }).Where(t => t.t.TournamentEndDate > date && t.t.Status == true)
                //                     .Select(p => new PlayerInTournament
                //                     {
                //                         Id = p.titt.ptt.ptf.ptour.Id,
                //                         Status = p.titt.ptt.ptf.ptour.Status,
                //                         ClothesNumber = p.titt.ptt.ptf.ptour.ClothesNumber                     
                //                     });
                //                 FootballPlayer footballPlayer =await _footballPlayerService.GetByIdAsync(footballPlayerId);
                //                 if (checkPlayerFromAnotherTeam != null)
                //                 {
                //                     return BadRequest(new
                //                     {
                //                         message = "Cầu thủ" +footballPlayer.PlayerName +"đang thi đấu trong một giải đấu khác"
                //                     });
                //                 }
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
        public async Task<ActionResult> DeleteById(int id, int? tourId, int? teamIntourId)
        {
            try
            {
                if(tourId >0 && teamIntourId >0)
                {
                    Tournament tour = _tournamentService.GetList().Where(t => t.Id == tourId).FirstOrDefault();
                    if (tour == null)
                    {
                        return NotFound(new { message = "Giải đấu không tồn tại " });
                    }

                    DateTime date = DateTime.Now.AddHours(7);

                    if (tour.RegisterEndDate < date)
                    {
                        return BadRequest(new
                        {
                            message = "Không thể xóa cầu thủ khi giải đấu đang diễn ra"
                        });
                    }

                    IQueryable<PlayerInTournament> listPlayer = _playerInTournament.GetList().Where(p => p.TeamInTournamentId == teamIntourId);
                    var playerIntour = listPlayer.ToList();
                    int numplayer = 0;
                    if (tour.FootballFieldTypeId == 1)
                    {
                        numplayer = 5;
                    }

                    if (tour.FootballFieldTypeId == 2)
                    {
                        numplayer = 7;
                    }
                    if (tour.FootballFieldTypeId == 3)
                    {
                        numplayer = 11;
                    }
                    if (playerIntour.Count() <= numplayer)
                    {
                        return BadRequest(new
                        {
                            message = "Số lượng cầu thủ tham gia giải không đủ"
                        });
                    }

                }
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
                        return BadRequest(new {
                            message = "Xóa cầu thủ trong giải đấu thất bại"});
                    }
                }
                return NotFound(new { message = "Không tìm thấy cầu thủ trong giải đấu với id là " + id});
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
