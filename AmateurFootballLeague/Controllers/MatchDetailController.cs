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

        public MatchDetailController (IMatchService match, IMatchDetailService matchDetail, IMapper mapper, IPlayerInTournamentService playerInTournament)
        {
            _match = match; 
            _matchDetail = matchDetail;
            _mapper = mapper;
            _playerInTournament = playerInTournament;
        }

        [HttpGet]
        [Route("MatchId")]
        public ActionResult<MatchDetailFVM> GetMatchDetailByMatch(int matchId)
        {
            try
            {
                IQueryable<MatchDetail> listDtMatch = _matchDetail.GetList().Join(_match.GetList(), md => md.Match, m => m, (md, m) => new { md, m })
                    .Join(_playerInTournament.GetList(), mdp => mdp.md.PlayerInTournament, p => p, (mdp, p) => new MatchDetail
                    {
                        Id = mdp.md.Id,
                        MatchScore = mdp.md.MatchScore,
                        YellowCardNumber = mdp.md.YellowCardNumber,
                        RedCardNumber = mdp.md.RedCardNumber,
                        MatchId = mdp.m.Id,
                        PlayerInTournamentId = p.Id,
                        Match = mdp.m,
                        PlayerInTournament = p

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
