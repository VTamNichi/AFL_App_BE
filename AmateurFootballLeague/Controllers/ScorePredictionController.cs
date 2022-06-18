using AmateurFootballLeague.IRepositories;
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
    public class ScorePredictionController : ControllerBase
    {
        private readonly IScorePredictionService _scorePrediction;
        private readonly IMapper _mapper;
        private readonly IMatchService _matchService;
        public ScorePredictionController (IScorePredictionService scorePrediction , IMapper mapper, IMatchService matchService)
        {
            _scorePrediction = scorePrediction; 
            _mapper = mapper;
            _matchService = matchService;
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<ActionResult<ScorePredictionVM>> GetById(int id)
        {
            try
            {
                ScorePrediction sp = await _scorePrediction.GetByIdAsync (id);
                if(sp == null)
                {
                    return NotFound("Không tìm thấy dự đoán tỷ số với id là " + id);
                }
                return Ok(_mapper.Map<ScorePredictionVM>(sp));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public ActionResult<ScorePredictionFVM> GetAllScorePrediction(int tournamentId, int userId,
            SortTypeEnum orderType)
        {
            try
            {
                IQueryable<ScorePrediction> listPredict = _scorePrediction.GetList().Join(_matchService.GetList(), s => s.Match, m => m,
                    (s, m) => new ScorePrediction
                    {
                        Id = s.Id,
                        TeamAscore = s.TeamAscore,
                        TeamBscore = s.TeamBscore,
                        Status = s.Status,
                        TeamInMatchAid = s.TeamInMatchAid,
                        TeamInMatchBid = s.TeamInMatchBid,
                        UserId = s.UserId,
                        MatchId = m.Id,
                        Match = m
                    }).Where(s => s.UserId == userId && s.Match!.TournamentId == tournamentId);
                if (orderType == SortTypeEnum.DESC)
                {
                    listPredict = listPredict.OrderByDescending(s => s.Id);
                }
                var predictListResponse = new ScorePredictionLVF
                {

                    Scores = _mapper.Map<List<ScorePrediction>, List<ScorePredictionFVM>>(listPredict.ToList())
                };
            
                return Ok(predictListResponse);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [HttpPost]
        public async Task<ActionResult<ScorePredictionVM>> CreateScorePrediction(ScorePredictionCM model)
        {
            ScorePrediction scorePrediction = new();
            try
            {
                var check =  _scorePrediction.GetList().Where(s => s.UserId == model.UserId && s.MatchId == model.MatchId);
                if(check == null  || check.Count() == 0)
                {
                    scorePrediction.TeamAscore = model.TeamAscore;

                    scorePrediction.TeamBscore  = model.TeamBscore;
                    scorePrediction.Status = model.Status;

                    scorePrediction.TeamInMatchAid = model.TeamInMatchAid;

                    scorePrediction.TeamInMatchBid = model.TeamInMatchBid;

                    scorePrediction.UserId =model.UserId;

                    scorePrediction.MatchId = model.MatchId;
                    ScorePrediction created = await _scorePrediction.AddAsync(scorePrediction);
                    if (created != null)
                    {
                        return CreatedAtAction("GetById", new { id = created.Id }, _mapper.Map<ScorePrediction>(created));
                    }
                    return BadRequest("Tạo dự đoán tỷ số thất bại");
                }
                return BadRequest(new
                {
                    message = "Người dùng đã dự đoán trận đấu này"
                });
            }
            catch
            {
                return StatusCode (StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateScorePrediction(ScorePredictionUM model)
        {
            try
            {
                ScorePrediction scorePrediction = await _scorePrediction.GetByIdAsync(model.Id);
                if (scorePrediction != null)
                {
                    scorePrediction.TeamAscore = model.TeamAscore;

                    scorePrediction.TeamBscore = model.TeamBscore;
                    bool isUpdated = await _scorePrediction.UpdateAsync(scorePrediction);
                    if (isUpdated)
                    {
                        return Ok(new
                        {
                            message = "Cập nhật dự đoán tỷ số thành công"
                        });
                    }
                    return BadRequest("Cập nhật dự đoán tỷ số thất bại");
                }
                return NotFound("Không tìm thấy dự đoán tỷ số với id là " + model.Id);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        [Route("Status")]
        public async Task<ActionResult> ChangeStatusScorePrediction(int id , string status)
        {
            try
            {
                ScorePrediction scorePrediction = await _scorePrediction.GetByIdAsync(id);
                if (scorePrediction != null)
                {
                    scorePrediction.Status = status;
                    bool isUpdated = await _scorePrediction.UpdateAsync(scorePrediction);
                    if (isUpdated)
                    {
                        return Ok(new
                        {
                            message = "Thay đổi trạng thái dự đoán tỷ số thành công"
                        });
                    }
                    return BadRequest("Thay đổi trạng thái dự đoán tỷ số thất bại");
                }
                return NotFound("Không tìm thấy dự đoán tỷ số với id là " + id);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
