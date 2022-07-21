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
        private readonly ITournamentService _tournamentService;
        private readonly ITeamService _teamService1;
        private readonly ITeamInMatchService _teamInMatch;
        private readonly ITeamInTournamentService _teamInTournamentService;
        private readonly IPlayerInTeamService _playerInTeamService;
        private readonly IPlayerInTournamentService _playerInTournamentService;
        public ScorePredictionController (IScorePredictionService scorePrediction , IMapper mapper, IMatchService matchService,
            ITournamentService tournamentService, IUserService userService, ITeamService teamService1, ITeamInTournamentService teamInTournamentService,
            IPlayerInTournamentService playerInTournamentService, IPlayerInTeamService playerInTeamService , ITeamInMatchService teamInMatch)
        {
            _scorePrediction = scorePrediction; 
            _mapper = mapper;
            _matchService = matchService;
            _tournamentService = tournamentService;
            _teamService1 = teamService1;
            _teamInMatch = teamInMatch;
            _teamInTournamentService= teamInTournamentService;
            _playerInTeamService = playerInTeamService;
            _playerInTournamentService = playerInTournamentService;
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
                Tournament checkTour = _tournamentService.GetList().Join(_matchService.GetList(), t => t.Id, m => m.TournamentId, (t, m) => new { t, m }).
                    Where(t => t.m.Id == model.MatchId).Select(t => new Tournament
                    {
                        Id = t.t.Id,
                        Status = t.t.Status,
                        UserId = t.t.UserId

                    }).Where(t=> t.UserId == model.UserId).FirstOrDefault()!;
                Tournament findTour = _tournamentService.GetList().Join(_matchService.GetList(), t => t.Id, m => m.TournamentId, (t, m) => new { t, m }).
                    Where(t => t.m.Id == model.MatchId).Select(t => new Tournament
                    {
                        Id = t.t.Id,
                        Status = t.t.Status,
                        UserId = t.t.UserId

                    }).FirstOrDefault()!;
                if(checkTour != null)
                {
                    return BadRequest(new
                    {
                        messgae = "Bạn không thể dự đoán giải đấu của mình"
                    });
                }

                Team chekcTeam = _teamService1.GetList().Join(_teamInTournamentService.GetList(), t => t.Id, tit => tit.TeamId, (t, tit) => new { t, tit })
                    .Where(t => t.t.Id == model.UserId && t.tit.TournamentId == findTour.Id ).Select(t=> new Team
                    {
                        Id =t.t.Id,
                        TeamName = t.t.TeamName
                    }).FirstOrDefault()!;

                if(chekcTeam != null)
                {
                    return BadRequest(new
                    {
                        messgae = "Bạn không thể dự đoán giải đang tham dự"
                    });
                }

                PlayerInTeam checkPlayer = _playerInTeamService.GetList().Join(_playerInTournamentService.GetList(), pit => pit.Id, pitour => pitour.PlayerInTeamId, (pit, pitour) => new { pit, pitour }).
                Where(p => p.pit.FootballPlayerId == model.UserId)
                    .Join(_teamInTournamentService.GetList(), pt => pt.pitour.TeamInTournament, tit => tit, (pt, tit) => new { pt, tit }).Where(t => t.tit.TournamentId == findTour.Id).
                    Select(t => new PlayerInTeam
                    {
                        Id = t.pt.pit.Id,
                        TeamId = t.pt.pit.TeamId,
                        FootballPlayerId = t.pt.pit.FootballPlayerId
                    }).FirstOrDefault()!;

                if(checkPlayer != null)
                {
                    return BadRequest(new
                    {
                        messgae = "Bạn không thể dự đoán giải đấu đang tham dự"
                    });
                }
                Match checkMatch =await _matchService.GetByIdAsync(model.MatchId);
                if(checkMatch == null)
                {
                    return BadRequest(new
                    {
                        messgae = "Trận đấu không tồn tại"
                    });
                }
                DateTime date = DateTime.Now.AddHours(7);
                if (checkMatch.MatchDate < date)
                {
                    return BadRequest(new
                    {
                        messgae = "Trận đấu đã bắt đầu"
                    });
                }
                if(!check.Any())
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

        [HttpGet("truePredict")]
        public ActionResult<ScorePredictionVM> GetTrueScorePrediction(int matchId)
        {
            try
            {
                ScorePrediction scorePredict = _scorePrediction.GetList().Where(s => s.MatchId == matchId && s.Status == "true").FirstOrDefault();
                if (scorePredict == null)
                {
                    return NotFound("Không có dự đoán trong trận này ");
                }
                return Ok(_mapper.Map<ScorePredictionVM>(scorePredict));
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        public async Task<ActionResult> UpdateScorePrediction(ScorePredictionUM model)
        {
            try
            {
                ScorePrediction scorePrediction = await _scorePrediction.GetByIdAsync(model.Id);
                var check = _scorePrediction.GetList().Where(s => s.UserId == model.UserId && s.MatchId == model.MatchId);
                Match checkMatch = await _matchService.GetByIdAsync(model.MatchId);
                if (checkMatch == null)
                {
                    return BadRequest(new
                    {
                        messgae = "Trận đấu không tồn tại"
                    });
                }
                DateTime date = DateTime.Now.AddHours(7);
                if (checkMatch.MatchDate < date)
                {
                    return BadRequest(new
                    {
                        messgae = "Trận đấu đã bắt đầu"
                    });
                }
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
        public async Task<ActionResult> ChangeStatusScorePrediction(int matchId)
        {
            try
            {
                IQueryable<TeamInMatch> result =  _teamInMatch.GetList().Where(s => s.MatchId == matchId);
                var listResult = result.ToList();
                IQueryable<ScorePrediction> scorePrediction = _scorePrediction.GetList().Where(s => s.MatchId == matchId);
                if(scorePrediction == null)
                {
                    return NotFound("Không có dự đoán cho trận đấu này");
                }
                ScorePrediction closetScore = new ScorePrediction();
                if (scorePrediction != null)
                {
                    closetScore = scorePrediction.Where(s => s.TeamInMatchAid == listResult[0].Id && s.TeamInMatchBid == listResult[1].Id && s.TeamAscore == 2 && s.TeamBscore == 2).FirstOrDefault();
                    if(closetScore == null)
                    {
                        closetScore =  scorePrediction.Where(s=> s.TeamInMatchAid == listResult[1].Id && s.TeamInMatchBid == listResult[0].Id && s.TeamAscore == 2 && s.TeamBscore == 2).FirstOrDefault();
                    }

                    if(closetScore == null)
                    {
    
                        List<ScorePrediction> listScoreOneSide = new List<ScorePrediction>();
                        if(listResult[0].TeamScore > listResult[1].TeamScore)
                        {
                            int scoreB = (int)listResult[1].TeamScore;
                            listScoreOneSide = scorePrediction.Where(s => s.TeamInMatchAid == listResult[0].Id && s.TeamInMatchBid == listResult[1].Id && s.TeamAscore == listResult[0].TeamScore && s.TeamBscore < listResult[0].TeamScore).ToList();
                            if (listScoreOneSide == null || listScoreOneSide.Count()==0)
                            {
                                 scoreB = (int)listResult[0].TeamScore;
                                listScoreOneSide = scorePrediction.Where(s => s.TeamInMatchAid == listResult[1].Id && s.TeamInMatchBid == listResult[0].Id && s.TeamAscore == listResult[1].TeamScore && s.TeamBscore < listResult[1].TeamScore).ToList();
                            }

                            if (listScoreOneSide.Count() > 0)
                            {
                                int minScore = (int)listScoreOneSide[0].TeamBscore;
                                int min = Math.Abs(scoreB -minScore);
                                foreach (ScorePrediction s in listScoreOneSide)
                                {
                                    var checkMin = Math.Abs(scoreB - (int)s.TeamBscore);
                                    if(checkMin < min)
                                    {
                                        min = checkMin;
                                        minScore = (int)s.TeamBscore;
                                    }
                                }

                                closetScore = scorePrediction.Where(s => s.TeamInMatchAid == listResult[0].Id && s.TeamInMatchBid == listResult[1].Id && s.TeamAscore == listResult[0].TeamScore && s.TeamBscore == minScore).FirstOrDefault();
                                if (closetScore == null)
                                {
                                    closetScore = scorePrediction.Where(s => s.TeamInMatchAid == listResult[1].Id && s.TeamInMatchBid == listResult[0].Id && s.TeamAscore == listResult[1].TeamScore && s.TeamBscore == minScore).FirstOrDefault();
                                }
                            }
                            if(closetScore == null)
                            {
                                listScoreOneSide = scorePrediction.Where(s => s.TeamInMatchAid == listResult[0].Id && s.TeamInMatchBid == listResult[1].Id &&  s.TeamBscore < s.TeamAscore).ToList();
                                int scoreA = (int)listResult[0].TeamScore;
                                 scoreB = (int)listResult[1].TeamScore;
                                if (listScoreOneSide == null || listScoreOneSide.Count() == 0)
                                {
                                     scoreA = (int)listResult[1].TeamScore;
                                     scoreB = (int)listResult[0].TeamScore;
                                    listScoreOneSide = scorePrediction.Where(s => s.TeamInMatchAid == listResult[1].Id && s.TeamInMatchBid == listResult[0].Id && s.TeamBscore < s.TeamAscore).ToList();
                                }
                                if (listScoreOneSide.Count() > 0)
                                {
                                    int minScoreA = (int)listScoreOneSide[0].TeamAscore;
                                    int minScoreB = (int)listScoreOneSide[0].TeamBscore;
                                    int min = Math.Abs(scoreA - minScoreA) + Math.Abs(scoreB - minScoreB);
                                    int dif = Math.Abs(minScoreA - minScoreB);
                                    foreach (ScorePrediction s in listScoreOneSide)
                                    {
                                        var checkMin = Math.Abs(scoreA - (int)s.TeamAscore) + Math.Abs(scoreB - (int)s.TeamBscore);
                                        var checkDif = s.TeamAscore - s.TeamBscore;
                                        if (checkMin < min)
                                        {
                                           min = checkMin;
                                            minScoreA = (int)s.TeamAscore;
                                            minScoreB = (int)s.TeamBscore;
                                        }
                                    }

                                    closetScore = scorePrediction.Where(s => s.TeamInMatchAid == listResult[0].Id && s.TeamInMatchBid == listResult[1].Id && s.TeamAscore == minScoreA && s.TeamBscore == minScoreB).FirstOrDefault();
                                    if (closetScore == null)
                                    {
                                        closetScore = scorePrediction.Where(s => s.TeamInMatchAid == listResult[1].Id && s.TeamInMatchBid == listResult[0].Id && s.TeamAscore == minScoreA && s.TeamBscore == minScoreB).FirstOrDefault();
                                    }
                                }
                            }

                        }
                        if (listResult[0].TeamScore < listResult[1].TeamScore)
                        {
                            int scoreA = (int)listResult[0].TeamScore;
                            listScoreOneSide = scorePrediction.Where(s => s.TeamInMatchAid == listResult[0].Id && s.TeamInMatchBid == listResult[1].Id && s.TeamAscore < listResult[1].TeamScore && s.TeamBscore == listResult[1].TeamScore).ToList();
                            if (listScoreOneSide == null || listScoreOneSide.Count() == 0)
                            {
                                 scoreA = (int)listResult[1].TeamScore;
                                listScoreOneSide = scorePrediction.Where(s => s.TeamInMatchAid == listResult[1].Id && s.TeamInMatchBid == listResult[0].Id && s.TeamAscore < listResult[0].TeamScore && s.TeamBscore == listResult[0].TeamScore).ToList();
                            }
                            if (listScoreOneSide.Count() > 0)
                            {
                                int minScore = (int)listScoreOneSide[0].TeamAscore;
                                 scoreA = (int)listResult[0].TeamScore;
                                int min = Math.Abs(scoreA - minScore);
                                foreach (ScorePrediction s in listScoreOneSide)
                                {
                                    var checkMin = Math.Abs(scoreA - (int)s.TeamAscore);
                                    if (checkMin < min)
                                    {
                                        min = checkMin;
                                        minScore = (int)s.TeamAscore;
                                    }
                                }

                                closetScore = scorePrediction.Where(s => s.TeamInMatchAid == listResult[0].Id && s.TeamInMatchBid == listResult[1].Id && s.TeamAscore == minScore && s.TeamBscore == listResult[1].TeamScore).FirstOrDefault();
                                if (closetScore == null)
                                {
                                    closetScore = scorePrediction.Where(s => s.TeamInMatchAid == listResult[1].Id && s.TeamInMatchBid == listResult[0].Id && s.TeamAscore == minScore && s.TeamBscore == listResult[0].TeamScore).FirstOrDefault();
                                }
                            }

                            if (closetScore == null)
                            {
                                listScoreOneSide = scorePrediction.Where(s => s.TeamInMatchAid == listResult[0].Id && s.TeamInMatchBid == listResult[1].Id && s.TeamBscore > s.TeamAscore).ToList();
                                 scoreA = (int)listResult[0].TeamScore;
                                int scoreB = (int)listResult[1].TeamScore;
                                if (listScoreOneSide == null || listScoreOneSide.Count() == 0)
                                {
                                    scoreA = (int)listResult[1].TeamScore;
                                    scoreB = (int)listResult[0].TeamScore;
                                    listScoreOneSide = scorePrediction.Where(s => s.TeamInMatchAid == listResult[1].Id && s.TeamInMatchBid == listResult[0].Id && s.TeamBscore > s.TeamAscore).ToList();
                                }
                                if (listScoreOneSide.Count() > 0)
                                {
                                    int minScoreA = (int)listScoreOneSide[0].TeamAscore;
                                    int minScoreB = (int)listScoreOneSide[0].TeamBscore;
                                    int min = Math.Abs(scoreA - minScoreA) + Math.Abs(scoreB - minScoreB);
                                    int dif = Math.Abs(minScoreA - minScoreB);
                                    foreach (ScorePrediction s in listScoreOneSide)
                                    {
                                        var checkMin = Math.Abs(scoreA - (int)s.TeamAscore) + Math.Abs(scoreB - (int)s.TeamBscore);
                                        var checkDif = s.TeamAscore - s.TeamBscore;
                                        if (checkMin < min)
                                        {
                                            min = checkMin;
                                            minScoreA = (int)s.TeamAscore;
                                            minScoreB = (int)s.TeamBscore;
                                        }
                                    }

                                    closetScore = scorePrediction.Where(s => s.TeamInMatchAid == listResult[0].Id && s.TeamInMatchBid == listResult[1].Id && s.TeamAscore == minScoreA && s.TeamBscore == minScoreB).FirstOrDefault();
                                    if (closetScore == null)
                                    {
                                        closetScore = scorePrediction.Where(s => s.TeamInMatchAid == listResult[1].Id && s.TeamInMatchBid == listResult[0].Id && s.TeamAscore == minScoreA && s.TeamBscore == minScoreB).FirstOrDefault();
                                    }
                                }
                            }

                        }
                        if (listResult[0].TeamScore == listResult[1].TeamScore)
                        {
                            ScorePrediction drawUp = scorePrediction.Where(s => s.TeamInMatchAid == listResult[0].Id && s.TeamInMatchBid == listResult[1].Id && s.TeamAscore == listResult[0].TeamScore+1 && s.TeamBscore == listResult[1].TeamScore + 1).FirstOrDefault();
                            if (drawUp == null)
                            {
                                drawUp = scorePrediction.Where(s => s.TeamInMatchAid == listResult[1].Id && s.TeamInMatchBid == listResult[0].Id && s.TeamAscore == listResult[1].TeamScore + 1 && s.TeamBscore == listResult[0].TeamScore + 1).FirstOrDefault();
                            }
                            ScorePrediction drawDown = scorePrediction.Where(s => s.TeamInMatchAid == listResult[0].Id && s.TeamInMatchBid == listResult[1].Id && s.TeamAscore == listResult[0].TeamScore - 1 && s.TeamBscore == listResult[1].TeamScore - 1).FirstOrDefault();
                            if (drawDown == null)
                            {
                                drawDown = scorePrediction.Where(s => s.TeamInMatchAid == listResult[1].Id && s.TeamInMatchBid == listResult[0].Id && s.TeamAscore == listResult[1].TeamScore - 1 && s.TeamBscore == listResult[0].TeamScore - 1).FirstOrDefault();
                            }
                            if(drawUp != null && drawDown != null)
                            {
                                if (drawUp.Id < drawDown.Id)
                                {
                                    closetScore = drawUp;
                                }
                                else
                                {
                                    closetScore = drawDown;
                                }
                            }

                            if (drawUp != null && drawDown == null)
                            {
                                    closetScore = drawUp;             
                            }

                            if (drawUp == null && drawDown != null)
                            {
                                closetScore = drawDown;
                            }

                            if (closetScore == null)
                            {
                                listScoreOneSide = scorePrediction.Where(s => s.TeamInMatchAid == listResult[0].Id && s.TeamInMatchBid == listResult[1].Id && s.TeamAscore == s.TeamBscore).ToList();
                                if (listScoreOneSide == null || listScoreOneSide.Count() == 0)
                                {
                                    listScoreOneSide = scorePrediction.Where(s => s.TeamInMatchAid == listResult[1].Id && s.TeamInMatchBid == listResult[0].Id && s.TeamAscore ==s.TeamBscore).ToList();
                                }
                                int scoreB = (int)listResult[1].TeamScore;

                                if (listScoreOneSide.Count() > 0)
                                {
                                    int minScore = (int)listScoreOneSide[0].TeamBscore;
                                    int min = Math.Abs(scoreB - minScore);
                                    foreach (ScorePrediction s in listScoreOneSide)
                                    {
                                        var checkMin = Math.Abs(scoreB - (int)s.TeamBscore);
                                        if (checkMin < min)
                                        {
                                            min = checkMin;
                                            minScore = (int)s.TeamBscore;
                                        }
                                    }

                                    closetScore = scorePrediction.Where(s => s.TeamInMatchAid == listResult[0].Id && s.TeamInMatchBid == listResult[1].Id && s.TeamAscore == minScore && s.TeamBscore == minScore).FirstOrDefault();
                                    if (closetScore == null)
                                    {
                                        closetScore = scorePrediction.Where(s => s.TeamInMatchAid == listResult[1].Id && s.TeamInMatchBid == listResult[0].Id && s.TeamAscore == minScore && s.TeamBscore == minScore).FirstOrDefault();
                                    }
                                }
                            }
                        }
                    }
                    closetScore.Status = "true";
                    bool isUpdated = await _scorePrediction.UpdateAsync(closetScore);
                    if (isUpdated)
                    {
                        return Ok(new
                        {
                            message = "Thay đổi trạng thái dự đoán tỷ số thành công"
                        });
                    }
                    return BadRequest("Thay đổi trạng thái dự đoán tỷ số thất bại");
                }
                return NoContent();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
