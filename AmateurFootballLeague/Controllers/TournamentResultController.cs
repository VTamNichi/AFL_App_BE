using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/tournament-results")]
    [ApiController]
    public class TournamentResultController : ControllerBase
    {
        private readonly ITournamentResultService _tournamentResultService;
        private readonly ITeamInTournamentService _teamInTournamentService;
        private readonly ITournamentService _tournamentService;
        private readonly IMatchService _matchService;
        private readonly ITeamInMatchService _teamInMatchService;
        private readonly IFootballPlayerService _footballPlayerService;
        private readonly ITeamService _teamService;
        private readonly IPlayerInTournamentService _playerInTournamentService;
        private readonly IPlayerInTeamService _playerInTeamService;
        private readonly IMatchDetailService _matchDetailService;
        private readonly IMapper _mapper;
        public TournamentResultController(ITournamentResultService tournamentResultService, ITeamInTournamentService teamInTournamentService, 
            ITournamentService tournamentService,IMatchService matchService, ITeamInMatchService teamInMatchService,IFootballPlayerService footballPlayerService,
            ITeamService teamService, IPlayerInTournamentService playerInTournamentService,IPlayerInTeamService playerInTeamService,IMatchDetailService matchDetailService,IMapper mapper)
        {
            _tournamentResultService = tournamentResultService;
            _teamInTournamentService = teamInTournamentService;
            _tournamentService = tournamentService;
            _matchService = matchService;
            _teamInMatchService = teamInMatchService;   
            _footballPlayerService = footballPlayerService;
            _teamService = teamService;
            _playerInTeamService = playerInTeamService;
            _playerInTournamentService = playerInTournamentService;
            _matchDetailService = matchDetailService;
            _mapper = mapper;
    }

        /// <summary>Get list tournament result</summary>
        /// <returns>List tournamentResult</returns>
        /// <response code="200">Returns list tournamentResult</response>
        /// <response code="404">Not found tournamentResult</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<TournamentResultListVM> GetListTournamentResult(
            [FromQuery(Name = "tournamentId")] int? tourId,
            [FromQuery(Name = "teamId")] int? teamId,
            [FromQuery(Name = "footballPlayerId")] int? footballPlayerId,
            [FromQuery(Name = "prize")] string? prize,
            [FromQuery(Name = "order-by")] TournamentResultFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<TournamentResult> tournamentResultList = _tournamentResultService.GetList();
                if(tourId > 0)
                {
                    List<TournamentResult> result = new List<TournamentResult>();
                    TournamentResult champion = tournamentResultList.Join(_teamService.GetList(), tr => tr.Team , t => t , (tr,t)=> new TournamentResult
                    {
                        Id = tr.Id,
                        Prize = tr.Prize,
                        TeamInTournamentId = tr.TeamInTournamentId,
                        TournamentId = tr.TournamentId,
                        TeamId = tr.TeamId,
                        TotalYellowCard = tr.TotalYellowCard,
                        TotalRedCard = tr.TotalRedCard,
                        TotalWinScrore = tr.TotalWinScrore,
                        TotalWinMatch = tr.TotalWinMatch,
                        TotalLoseMatch = tr.TotalLoseMatch,
                        TotalDrawMatch = tr.TotalDrawMatch,
                        Team = new Team
                        {
                            Id = t.Id,
                            TeamName = t.TeamName,
                            TeamAvatar = t.TeamAvatar
                        },
                        FootballPlayerId = tr.FootballPlayerId
                    }).Where(t => t.TournamentId == tourId && t.Prize == "Champion").FirstOrDefault();
                    result.Add(champion);
                    TournamentResult second = tournamentResultList.Join(_teamService.GetList(), tr => tr.Team, t => t, (tr, t) => new TournamentResult
                    {
                        Id = tr.Id,
                        Prize = tr.Prize,
                        TeamInTournamentId = tr.TeamInTournamentId,
                        TournamentId = tr.TournamentId,
                        TeamId = tr.TeamId,
                        TotalYellowCard = tr.TotalYellowCard,
                        TotalRedCard = tr.TotalRedCard,
                        TotalWinScrore = tr.TotalWinScrore,
                        TotalWinMatch = tr.TotalWinMatch,
                        TotalLoseMatch = tr.TotalLoseMatch,
                        TotalDrawMatch = tr.TotalDrawMatch,
                        Team = new Team
                        {
                            Id = t.Id,
                            TeamName = t.TeamName,
                            TeamAvatar = t.TeamAvatar
                        },
                        FootballPlayerId = tr.FootballPlayerId
                    }).Where(t => t.TournamentId == tourId && t.Prize == "second").FirstOrDefault();
                    result.Add(second);
                    var topScore = tournamentResultList.Join(_footballPlayerService.GetList(), tr => tr.FootballPlayer, t => t, (tr, t) => new TournamentResult
                    {
                        Id = tr.Id,
                        Prize = tr.Prize,
                        TeamInTournamentId = tr.TeamInTournamentId,
                        TournamentId = tr.TournamentId,
                        TeamId = tr.TeamId,
                        FootballPlayerId = tr.FootballPlayerId,
                        FootballPlayer = new FootballPlayer
                        {
                            Id = t.Id,
                            PlayerName = t.PlayerName,
                            PlayerAvatar = t.PlayerAvatar
                        },
                        TotalWinScrore = tr.TotalWinScrore
                        
                    }).Where(t => t.TournamentId == tourId && t.Prize == "Top Goal").ToList();
                    for(int i=0; i<topScore.Count(); i++)
                    {
                        result.Add(topScore[i]);
                    }
                    var listTourResult = new TournamentResultListVM
                    {
                        TournamentResults = _mapper.Map<List<TournamentResult>, List<TournamentResultVM>>(result),
                    };
                    return Ok(listTourResult);
                }
                if (teamId >0)
                {
             
                    tournamentResultList = tournamentResultList.Join(_tournamentService.GetList(), ttr => ttr.Tournament , tou => tou, (ttr,tou)=> new {ttr, tou})
                        .Join(_teamService.GetList(), tr => tr.ttr.Team , t => t, (tr, t)=> new TournamentResult
                    {
                        Id = tr.ttr.Id,
                        Prize = tr.ttr.Prize,
                        TeamInTournamentId = tr.ttr.TeamInTournamentId,
                        TournamentId = tr.ttr.TournamentId,
                        TeamId = tr.ttr.TeamId,
                        TotalYellowCard = tr.ttr.TotalYellowCard,
                        TotalRedCard = tr.ttr.TotalRedCard,
                        TotalWinScrore = tr.ttr.TotalWinScrore,
                        TotalWinMatch = tr.ttr.TotalWinMatch,
                        TotalLoseMatch = tr.ttr.TotalLoseMatch,
                        TotalDrawMatch = tr.ttr.TotalDrawMatch,
                        Team = new Team
                        {
                            Id = t.Id,
                            TeamName = t.TeamName,
                            TeamAvatar = t.TeamAvatar
                        },
                        Tournament = new Tournament
                        {
                            Id= tr.tou.Id,
                            TournamentName = tr.tou.TournamentName,
                            TournamentAvatar = tr.tou.TournamentAvatar
                        }
                        
                    }).Where(s => s.TeamId == teamId && s.TotalWinMatch >=0);
                }

                if (footballPlayerId > 0)
                {
                    tournamentResultList = tournamentResultList.Join(_tournamentService.GetList(), ttr => ttr.Tournament, tou => tou, (ttr, tou) => new { ttr, tou })
                        .Join(_footballPlayerService.GetList(), tr => tr.ttr.FootballPlayer, t => t, (tr, t) => new TournamentResult
                    {
                        Id = tr.ttr.Id,
                        Prize = tr.ttr.Prize,
                        TeamInTournamentId = tr.ttr.TeamInTournamentId,
                        TournamentId = tr.ttr.TournamentId,
                        TeamId = tr.ttr.TeamId,
                        FootballPlayerId = tr.ttr.FootballPlayerId,
                        FootballPlayer = new FootballPlayer
                        {
                            Id = t.Id,
                            PlayerName = t.PlayerName,
                            PlayerAvatar = t.PlayerAvatar
                        },
                        TotalWinScrore = tr.ttr.TotalWinScrore,
                            Tournament = new Tournament
                            {
                                Id = tr.tou.Id,
                                TournamentName = tr.tou.TournamentName,
                                TournamentAvatar = tr.tou.TournamentAvatar
                            }
                        }).Where(t => t.FootballPlayerId == footballPlayerId);
                }

                var tournamentResultListPaging = tournamentResultList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var tournamentResultListOrder = new List<TournamentResult>();
                if (orderBy == TournamentResultFieldEnum.Id)
                {
                    tournamentResultListOrder = tournamentResultListPaging.OrderBy(tnm => tnm.Id).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        tournamentResultListOrder = tournamentResultListPaging.OrderByDescending(tnm => tnm.Id).ToList();
                    }
                }
                if (orderBy == TournamentResultFieldEnum.Prize)
                {
                    tournamentResultListOrder = tournamentResultListPaging.OrderBy(tnm => tnm.Prize).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        tournamentResultListOrder = tournamentResultListPaging.OrderByDescending(tnm => tnm.Prize).ToList();
                    }
                }

                var tournamentResultListResponse = new TournamentResultListVM
                {
                    TournamentResults = _mapper.Map<List<TournamentResult>, List<TournamentResultVM>>(tournamentResultListOrder),
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(tournamentResultListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get tournament result by id</summary>
        /// <returns>Return the tournament result with the corresponding id</returns>
        /// <response code="200">Returns the tournament result with the specified id</response>
        /// <response code="404">No tournament result found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<TournamentResultVM>> GetTournamentResultById(int id)
        {
            try
            {
                TournamentResult currentTournamentResultVM = await _tournamentResultService.GetByIdAsync(id);
                if (currentTournamentResultVM != null)
                {
                    return Ok(_mapper.Map<TournamentResultVM>(currentTournamentResultVM));
                }
                return NotFound("Không tìm thấy kết quả giải đấu với id là " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new tournament result</summary>
        /// <response code="201">Created new tournament result successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task <ActionResult<TournamentResultVM>> CreateTournamentResult(int matchId)
        {
            try
            {
                Match matchInTour = await _matchService.GetByIdAsync(matchId);
                var listResult = _tournamentResultService.GetList().Where(t => t.TournamentId == matchInTour.TournamentId).ToList();    

                if(listResult != null)
                {
                    for(int i=0; i < listResult.Count(); i++)
                    {
                        await _tournamentResultService.DeleteAsync(listResult[i]);
                    }
                }

                IQueryable<Match> match = _matchService.GetList().Join(_teamInMatchService.GetList(), mt => mt.Id, tim => tim.MatchId, (mt, tim) => new { mt, tim }).Where(m => m.mt.Id == matchId)
                    .Join(_teamInTournamentService.GetList(), tit => tit.tim.TeamInTournament, titour => titour, (tit, titour) => new Match
                    {
                        Id = matchId,
                        MatchDate = tit.mt.MatchDate,
                        Status = tit.mt.Status,
                        Round = tit.mt.Round,
                        Fight = tit.mt.Fight,
                        GroupFight = tit.mt.GroupFight,
                        TournamentId = tit.mt.TournamentId,
                        TeamInMatches = new List<TeamInMatch>
                    {
                        new TeamInMatch
                        {
                            Id = tit.tim.Id,
                            TeamScore = tit.tim.TeamScore,
                            TeamInTournament = new TeamInTournament
                            {
                                Id = titour.Id,
                                Point = titour.Point,
                                WinScoreNumber = titour.WinScoreNumber,
                                LoseScoreNumber = titour.LoseScoreNumber,
                                DifferentPoint = titour.DifferentPoint,
                                TotalRedCard = titour.TotalRedCard,
                                TotalYellowCard = titour.TotalYellowCard,
                                TournamentId = titour.TournamentId,
                                TeamId = titour.TeamId
                            }
                        }
                    },
                    });
                  var getChampion = match.ToList();
                var team1 = getChampion[0].TeamInMatches.ToList();
                var team2 = getChampion[1].TeamInMatches.ToList();
                IQueryable<PlayerInTournament> listPlayerTeam1 = _playerInTournamentService.GetList().Join(_playerInTeamService.GetList(),
                    pt => pt.PlayerInTeam, p => p, (pt, p) => new PlayerInTournament
                    {
                        Id = pt.Id,
                        Status = pt.Status,
                        ClothesNumber = pt.ClothesNumber,
                        TeamInTournamentId = pt.TeamInTournamentId,
                        PlayerInTeamId = pt.PlayerInTeamId,
                        PlayerInTeam = new PlayerInTeam
                        {
                            Id = p.Id,
                            Status = p.Status,
                            FootballPlayerId = p.FootballPlayerId,
                            TeamId = p.TeamId
                        }
                    }).Where(p => p.TeamInTournamentId == team1[0].TeamInTournament.Id);
                IQueryable<PlayerInTournament> listPlayerTeam2 = _playerInTournamentService.GetList().Join(_playerInTeamService.GetList(),
                    pt => pt.PlayerInTeam, p => p, (pt, p) => new PlayerInTournament
                    {
                        Id = pt.Id,
                        Status = pt.Status,
                        ClothesNumber = pt.ClothesNumber,
                        TeamInTournamentId = pt.TeamInTournamentId,
                        PlayerInTeamId = pt.PlayerInTeamId,
                        PlayerInTeam = new PlayerInTeam
                        {
                            Id = p.Id,
                            Status = p.Status,
                            FootballPlayerId = p.FootballPlayerId,
                            TeamId = p.TeamId
                        }
                    }).Where(p => p.TeamInTournamentId == team2[0].TeamInTournament.Id);
                var playersTeam1 = listPlayerTeam1.ToList();
                var playersTeam2 = listPlayerTeam2.ToList();

                if (team1[0].TeamScore > team2[0].TeamScore)
                {
                    TournamentResult champTeam = new TournamentResult();
                    champTeam.Prize = "Champion";
                    champTeam.TeamInTournamentId = team1[0].TeamInTournament.Id;
                    champTeam.TournamentId = getChampion[0].TournamentId;
                    champTeam.TeamId = team1[0].TeamInTournament.TeamId;
                    champTeam.TotalYellowCard = team1[0].TeamInTournament.TotalYellowCard;
                    champTeam.TotalRedCard = team1[0].TeamInTournament.TotalRedCard;
                    champTeam.TotalWinScrore = team1[0].TeamInTournament.WinScoreNumber;
                    champTeam.TotalWinMatch = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == team1[0].TeamInTournament.Id && s.Result > 1).Count();
                    champTeam.TotalLoseMatch = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == team1[0].TeamInTournament.Id && s.Result < 1).Count();
                    champTeam.TotalDrawMatch = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == team1[0].TeamInTournament.Id && s.Result == 1).Count();
                    await _tournamentResultService.AddAsync(champTeam);
                    for (int i =0; i<playersTeam1.Count(); i++)
                    {
                        TournamentResult champRs = new TournamentResult();
                        champRs.Prize = "Champion";
                        champRs.TeamInTournamentId = team1[0].TeamInTournament.Id;
                        champRs.TournamentId = getChampion[0].TournamentId;
                        champRs.TeamId = team1[0].TeamInTournament.TeamId;
                        champRs.FootballPlayerId = playersTeam1[i].PlayerInTeam.FootballPlayerId;
                        await _tournamentResultService.AddAsync(champRs);
                    }

                    TournamentResult secondTeam = new TournamentResult();
                    secondTeam.Prize = "second";
                    secondTeam.TeamInTournamentId = team2[0].TeamInTournament.Id;
                    secondTeam.TournamentId = getChampion[0].TournamentId;
                    secondTeam.TeamId = team2[0].TeamInTournament.TeamId;
                    secondTeam.TotalYellowCard = team1[0].TeamInTournament.TotalYellowCard;
                    secondTeam.TotalRedCard = team1[0].TeamInTournament.TotalRedCard;
                    secondTeam.TotalWinScrore = team1[0].TeamInTournament.WinScoreNumber;
                    secondTeam.TotalWinMatch = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == team1[0].TeamInTournament.Id && s.Result > 1).Count();
                    secondTeam.TotalLoseMatch = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == team1[0].TeamInTournament.Id && s.Result < 1).Count();
                    secondTeam.TotalDrawMatch = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == team1[0].TeamInTournament.Id && s.Result == 1).Count();
                    await _tournamentResultService.AddAsync(secondTeam);
                    for (int i = 0; i < playersTeam2.Count(); i++)
                    {
                        TournamentResult second = new TournamentResult();
                        second.Prize = "second";
                        second.TeamInTournamentId = team2[0].TeamInTournament.Id;
                        second.TournamentId = getChampion[0].TournamentId;
                        second.TeamId = team2[0].TeamInTournament.TeamId;
                        second.FootballPlayerId = playersTeam2[i].PlayerInTeam.FootballPlayerId;
                        await _tournamentResultService.AddAsync(second);
                    }
                }

                else
                {
                    TournamentResult champTeam = new TournamentResult();
                    champTeam.Prize = "second";
                    champTeam.TeamInTournamentId = team1[0].TeamInTournament.Id;
                    champTeam.TournamentId = getChampion[0].TournamentId;
                    champTeam.TeamId = team1[0].TeamInTournament.TeamId;
                    champTeam.TotalYellowCard = team1[0].TeamInTournament.TotalYellowCard;
                    champTeam.TotalRedCard = team1[0].TeamInTournament.TotalRedCard;
                    champTeam.TotalWinScrore = team1[0].TeamInTournament.WinScoreNumber;
                    champTeam.TotalWinMatch = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == team1[0].TeamInTournament.Id && s.Result > 1).Count();
                    champTeam.TotalLoseMatch = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == team1[0].TeamInTournament.Id && s.Result < 1).Count();
                    champTeam.TotalDrawMatch = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == team1[0].TeamInTournament.Id && s.Result == 1).Count();
 
                    await _tournamentResultService.AddAsync(champTeam);
                    for (int i = 0; i < playersTeam1.Count(); i++)
                    {
                        TournamentResult champRs = new TournamentResult();
                        champRs.Prize = "second";
                        champRs.TeamInTournamentId = team1[0].TeamInTournament.Id;
                        champRs.TournamentId = getChampion[0].TournamentId;
                        champRs.TeamId = team1[0].TeamInTournament.TeamId;
                        champRs.FootballPlayerId = playersTeam1[i].PlayerInTeam.FootballPlayerId;
                        await _tournamentResultService.AddAsync(champRs);
                    }

                    TournamentResult secondTeam = new TournamentResult();
                    secondTeam.Prize = "Champion";
                    secondTeam.TeamInTournamentId = team2[0].TeamInTournament.Id;
                    secondTeam.TournamentId = getChampion[0].TournamentId;
                    secondTeam.TeamId = team2[0].TeamInTournament.TeamId;
                    secondTeam.TotalYellowCard = team1[0].TeamInTournament.TotalYellowCard;
                    secondTeam.TotalRedCard = team1[0].TeamInTournament.TotalRedCard;
                    secondTeam.TotalWinScrore = team1[0].TeamInTournament.WinScoreNumber;
                    secondTeam.TotalWinMatch = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == team1[0].TeamInTournament.Id && s.Result > 1).Count();
                    secondTeam.TotalLoseMatch = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == team1[0].TeamInTournament.Id && s.Result < 1).Count();
                    secondTeam.TotalDrawMatch = _teamInMatchService.GetList().Where(s => s.TeamInTournamentId == team1[0].TeamInTournament.Id && s.Result == 1).Count();
                    await _tournamentResultService.AddAsync(secondTeam);
                    for (int i = 0; i < playersTeam2.Count(); i++)
                    {
                        TournamentResult second = new TournamentResult();
                        second.Prize = "Champion";
                        second.TeamInTournamentId = team2[0].TeamInTournament.Id;
                        second.TournamentId = getChampion[0].TournamentId;
                        second.TeamId = team2[0].TeamInTournament.TeamId;
                        second.FootballPlayerId = playersTeam2[i].PlayerInTeam.FootballPlayerId;
                        await _tournamentResultService.AddAsync(second);
                    }
                }

                var listScore = _matchDetailService.GetList().Join(_matchService.GetList(), md => md.Match, m => m, (md, m) => new { md, m })
                    .Where(m => m.m.TournamentId == getChampion[0].TournamentId && m.md.ActionMatchId == 1)
                    .GroupBy(m => m.md.FootballPlayerId).Select(m => new { FootballPlayerId = m.Key, Count = m.Count() }).ToList();

                int max = listScore[0].Count;
                for(int i = 0; i< listScore.Count(); i++)
                {
                    if(listScore[i].Count > max)
                    {
                        max = listScore[i].Count;
                    }
                }

                var kingScore = listScore.Where(s => s.Count == max).ToList();

                List<PlayerInTournament> listKingScore = new List<PlayerInTournament>();

                for(int i =0; i< kingScore.Count(); i++)
                {
                    PlayerInTournament player = _playerInTournamentService.GetList().Join(_playerInTeamService.GetList(),
                    pt => pt.PlayerInTeam, p => p, (pt, p) => new PlayerInTournament
                    {
                        Id = pt.Id,
                        Status = pt.Status,
                        ClothesNumber = pt.ClothesNumber,
                        TeamInTournamentId = pt.TeamInTournamentId,
                        PlayerInTeamId = pt.PlayerInTeamId,
                        PlayerInTeam = new PlayerInTeam
                        {
                            Id = p.Id,
                            Status = p.Status,
                            FootballPlayerId = p.FootballPlayerId,
                            TeamId = p.TeamId
                        }
                    }).Where(p => p.PlayerInTeam.FootballPlayerId  == kingScore[i].FootballPlayerId).FirstOrDefault();
                    listKingScore.Add(player);
                }

                for (int i = 0; i < listKingScore.Count(); i++)
                {
                    TournamentResult second = new TournamentResult();
                    second.Prize = "Top Goal";
                    second.TeamInTournamentId = listKingScore[i].TeamInTournamentId;
                    second.TournamentId = getChampion[0].TournamentId;
                    second.TeamId = listKingScore[i].PlayerInTeam.TeamId;
                    second.FootballPlayerId = listKingScore[i].PlayerInTeam.FootballPlayerId;
                    second.TotalWinScrore = max;
                    await _tournamentResultService.AddAsync(second);
                }

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a tournament result</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<TournamentResultVM>> UpdateTournamentResult([FromBody] TournamentResultUM model)
        {
            TournamentResult currentTournamentResult = await _tournamentResultService.GetByIdAsync(model.Id);
            if (currentTournamentResult == null)
            {
                return NotFound("Không tìm thấy kết quả giải đấu với id là " + model.Id);
            }
            try
            {
                if(!String.IsNullOrEmpty(model.TournamentId.ToString()))
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync(model.TournamentId!.Value);
                    if (tournament == null)
                    {
                        return BadRequest("Giải đấu không tồn tại");
                    }
                    currentTournamentResult.TournamentId = model.TournamentId;
                }
                if(!String.IsNullOrEmpty(model.TeamInTournamentId.ToString()))
                {
                    TeamInTournament teamInTournament = await _teamInTournamentService.GetByIdAsync(model.TeamInTournamentId!.Value);
                    if (teamInTournament == null)
                    {
                        return BadRequest("Đội bóng trong giải đấu không tồn tại");
                    }
                    currentTournamentResult.TeamInTournamentId = model.TeamInTournamentId;
                }

                currentTournamentResult.Prize = String.IsNullOrEmpty(model.Prize) ? currentTournamentResult.Prize : model.Prize.Trim();
                currentTournamentResult.Description = String.IsNullOrEmpty(model.Description) ? currentTournamentResult.Description : model.Description.Trim();

                bool isUpdated = await _tournamentResultService.UpdateAsync(currentTournamentResult);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<TournamentResultVM>(currentTournamentResult));
                }
                return BadRequest("Cập nhật kết quả giải đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete tournament result By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            TournamentResult currentTournamentResultVM = await _tournamentResultService.GetByIdAsync(id);
            if (currentTournamentResultVM == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy kết quả giải đấu với id là " + id
                });
            }
            try
            {
                bool isDeleted = await _tournamentResultService.DeleteAsync(currentTournamentResultVM);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Xóa kết quả giải đấu thành công"
                    });
                }
                return BadRequest("Xóa kết quả giải đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
