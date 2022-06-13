﻿using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MatchDetailController : ControllerBase
    {
        private readonly IMatchService _match;
        private readonly IMatchDetailService _matchDetail;
        private readonly IMapper _mapper;
        private readonly IPlayerInTournamentService _playerInTournament;
        private readonly ITeamInTournamentService _teamInTournamentService;
        private readonly IPlayerInTeamService _playerInTeamService;
        private IFootballPlayerService _footballPlayerService;

        public MatchDetailController (IMatchService match, IMatchDetailService matchDetail, IMapper mapper, IPlayerInTournamentService playerInTournament, 
            ITeamInTournamentService teamInTournamentService, IPlayerInTeamService playerInTeamService, IFootballPlayerService footballPlayerService)
        {
            _match = match; 
            _matchDetail = matchDetail;
            _mapper = mapper;
            _playerInTournament = playerInTournament;
            _teamInTournamentService = teamInTournamentService;
            _playerInTeamService = playerInTeamService;
            _footballPlayerService = footballPlayerService;
        }

        [HttpGet]
        [Route("MatchId")]
        public ActionResult<MatchDetailFVM> GetMatchDetailByMatch(int matchId)
        {
            try
            {
                if(matchId == 0)
                {
                    IQueryable<MatchDetail> match = _matchDetail.GetList();
                    return Ok (match);
                }
                IQueryable<MatchDetail> listDtMatch = _matchDetail.GetList().Join(_playerInTournament.GetList(), md => md.PlayerInTournament, pit => pit, (md, pit) => new { md, pit })
                    .Join(_teamInTournamentService.GetList(), pitt => pitt.pit.TeamInTournament, tit => tit, (pitt, tit) => new { pitt, tit }).
                    Join(_playerInTeamService.GetList(), pitp => pitp.pitt.pit.PlayerInTeam, piteam => piteam, (pitp, piteam) => new { pitp, piteam })
                    .Join(_footballPlayerService.GetList(), pitf => pitf.piteam.FootballPlayer, f => f, (pitf, f) => new MatchDetail
                    {
                        Id = pitf.pitp.pitt.md.Id,
                        MatchScore = pitf.pitp.pitt.md.MatchScore,
                        YellowCardNumber = pitf.pitp.pitt.md.YellowCardNumber,
                        RedCardNumber = pitf.pitp.pitt.md.RedCardNumber,
                        MatchId = pitf.pitp.pitt.md.MatchId,
                        PlayerInTournament = new PlayerInTournament
                        {
                            Id = pitf.pitp.pitt.pit.Id,
                            Status = pitf.pitp.pitt.pit.Status,
                            ClothesNumber = pitf.pitp.pitt.pit.ClothesNumber,
                            PlayerInTeam = new PlayerInTeam
                            {
                                Id = pitf.piteam.Id,
                                Status = pitf.piteam.Status,
                                TeamId = pitf.pitp.tit.TeamId,
                                FootballPlayer = new FootballPlayer
                                {
                                    Id = f.Id,
                                    PlayerName = f.PlayerName,
                                    PlayerAvatar = f.PlayerAvatar,
                                    Position = f.Position
                                }
                            },
                            

                        }
                    }).Where(m => m.MatchId == matchId);
                var matchDt = new List<MatchDetail>();
                matchDt = listDtMatch.ToList();
                if (matchDt.Count() > 0)
                {

                    var matchDtListResponse = new MatchDetailFLV
                    {

                        MatchDetails = _mapper.Map<List<MatchDetail>, List<MatchDetailFVM>>(matchDt)

                    };
                    return Ok(matchDtListResponse);

                }

                return NotFound("Không tìm thấy chi tiết trận đấu");

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }

        [HttpGet]
        [Route("Id")]
        public async Task<ActionResult<MatchDetailVM>> FindById(int id)
        {
            try {
                MatchDetail match = await _matchDetail.GetByIdAsync(id);
                if(match == null)
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
        public async Task<ActionResult<MatchDetailVM>> CreateMatchDetail(MatchDetailCM match)
        {
            MatchDetail matchDetail = new MatchDetail();
            try { 
               
                    matchDetail.MatchScore = match.MatchScore;
                    matchDetail.YellowCardNumber = match.YellowCardNumber;
                    matchDetail.RedCardNumber = match.RedCardNumber;
                    matchDetail.MatchId = match.MatchId;
                    matchDetail.PlayerInTournamentId = match.PlayerInTournamentId;
                    MatchDetail created =await _matchDetail.AddAsync(matchDetail);
                    if(created != null)
                    {
                        return CreatedAtAction("GetById", new { id = created.Id }, _mapper.Map<MatchDetailVM>(created));
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
                if(matchDetail != null)
                {
                    matchDetail.MatchScore = match.MatchScore;
                    matchDetail.YellowCardNumber = match.YellowCardNumber;
                    matchDetail.RedCardNumber = match.RedCardNumber;
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
                if(matchDetail != null)
                {
                    bool isDeleted = await _matchDetail.DeleteAsync(matchDetail);
                    if (isDeleted)
                    {
                        return Ok( new {
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

    }
}
