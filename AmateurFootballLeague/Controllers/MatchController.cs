using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/matchs")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly ITeamInMatchService _teamInMatch;
        private readonly ITournamentService _tournamentService;
        private readonly IAgoraProvider _agoraProvider;
        private readonly IMapper _mapper;
        private readonly ITeamService _teamService;
        private readonly IPlayerInTeamService _playerInTeamService;
        private readonly IPlayerInTournamentService _playerInTournament;
        private readonly ITeamInTournamentService _teamInTournamentService;

        public MatchController(IMatchService matchService, ITeamInMatchService teamInMatch, ITournamentService tournamentService,
            IAgoraProvider agoraProvider, IMapper mapper, ITeamService teamService, IPlayerInTeamService playerInTeamService,
            IPlayerInTournamentService playerInTournament, ITeamInTournamentService teamInTournamentService)
        {
            _matchService = matchService;
            _teamInMatch = teamInMatch;
            _tournamentService = tournamentService;
            _agoraProvider = agoraProvider;
            _mapper = mapper;
            _teamService = teamService;
            _playerInTeamService = playerInTeamService;
            _playerInTournament = playerInTournament;
            _teamInTournamentService = teamInTournamentService;
        }

        [HttpGet]
        [Route("TournamentID")]
        public ActionResult<MatchListFVM> GetAllMatchByTournamentID (int? tournamentId,int? footballPlayerID, bool? fullInfo)
        {
            try
            {
                IQueryable<Match> listMatch = _matchService.GetList();
                if (tournamentId > 0)
                {
                        listMatch= listMatch.Join(_teamInMatch.GetList(), m => m.Id, tim => tim.MatchId, (m, tim) => new Match
                     {
                         Id = m.Id,
                         MatchDate = m.MatchDate,
                         Status = m.Status,
                         TournamentId = m.TournamentId,
                         Round = m.Round,
                         Fight = m.Fight,
                         GroupFight = m.GroupFight,
                         TeamInMatches = m.TeamInMatches
                     }).Where(m => m.TournamentId == tournamentId);
                }

                if(footballPlayerID>0)
                {
                    DateTime fromDate = DateTime.Now.Date;
                    var date = DateTime.Now.AddDays(+1).ToShortDateString().Split("/");
                    string nextDate = date[0] + "-"+date[1] + "-"+date[2];
                    DateTime fromDate2 = Convert.ToDateTime(nextDate);
                    Console.WriteLine($"Date Value: {fromDate2}");
                    Console.WriteLine($"Date Value: {nextDate}");
                    Console.WriteLine($"To date Date Value: {fromDate}");
                    listMatch = listMatch.Join(_teamInMatch.GetList(), m => m.Id, tim => tim.MatchId, (m, tim)=>new { m, tim }).Where(m => m.m.MatchDate >= fromDate && m.m.MatchDate < fromDate2).
                        Join(_teamInTournamentService.GetList(), timt => timt.tim.TeamInTournament, t => t, (timt, t) => new {timt,t}).Join(_playerInTournament.GetList(),
                        tpit=> tpit.t.Id, pit=>pit.TeamInTournamentId, (tpit, pit) => new
                        {
                            tpit,pit
                        }).Join(_playerInTeamService.GetList(), pitt=> pitt.pit.PlayerInTeam , piteam => piteam, (pitt, piteam) => new
                        {
                            pitt,piteam
                        }).Where(pit => pit.piteam.FootballPlayerId == footballPlayerID).Select(m=> new Match
                        {
                            Id = m.pitt.tpit.timt.m.Id,
                            MatchDate = m.pitt.tpit.timt.m.MatchDate,
                            Status = m.pitt.tpit.timt.m.Status,
                            TournamentId = m.pitt.tpit.timt.m.TournamentId,
                            Round = m.pitt.tpit.timt.m.Round,
                            Fight = m.pitt.tpit.timt.m.Fight,
                            GroupFight = m.pitt.tpit.timt.m.GroupFight,
                            TeamInMatches = new List<TeamInMatch>
                           {
                               new TeamInMatch
                               {
                                   Id = m.pitt.tpit.timt.tim.Id,
                                   TeamScore = m.pitt.tpit.timt.tim.TeamScore,
                                   YellowCardNumber = m.pitt.tpit.timt.tim.YellowCardNumber,
                                   RedCardNumber = m.pitt.tpit.timt.tim.RedCardNumber,
                                   Result = m.pitt.tpit.timt.tim.Result,
                                   TeamName = m.pitt.tpit.timt.tim.TeamName,
                                   TeamInTournamentId = m.pitt.tpit.t.Id,
                                   MatchId = m.pitt.tpit.timt.tim.MatchId,
                                   NextTeam = m.pitt.tpit.timt.tim.NextTeam,
                                   TeamInTournament = new TeamInTournament
                                   {
                                       Id = m.pitt.tpit.t.Id,
                                        Point = m.pitt.tpit.t.Point,
                    WinScoreNumber = m.pitt.tpit.t.WinScoreNumber,
                    LoseScoreNumber = m.pitt.tpit.t.LoseScoreNumber,
                    DifferentPoint = m.pitt.tpit.t.DifferentPoint,
                    TotalYellowCard = m.pitt.tpit.t.TotalYellowCard,
                    TotalRedCard = m.pitt.tpit.t.TotalRedCard,
                    GroupName = m.pitt.tpit.t.GroupName,
                    Status = m.pitt.tpit.t.Status,
                    StatusInTournament = m.pitt.tpit.t.StatusInTournament,
                    TournamentId = m.pitt.tpit.t.TournamentId,
                    TeamId = m.pitt.tpit.t.TeamId,
                                   }
                               }
                           }
                        });
                    var matchCheckTeam = new List<Match>();
                    var findMatch = listMatch.ToList();
                    for (int i = 0; i < findMatch.Count; i++)
                    {
                        IQueryable<Match> aMatch = _matchService.GetList().Join(_teamInMatch.GetList(), m => m.Id, tim => tim.MatchId, (m, tim) => new { m, tim })
                       .Join(_teamService.GetList(), timt => timt.tim.TeamInTournament!.Team, t => t, (timt, t) => new Match
                       {
                           Id = timt.m.Id,
                           MatchDate = timt.m.MatchDate,
                           Status = timt.m.Status,
                           TournamentId = timt.m.TournamentId,
                           Round = timt.m.Round,
                           Fight = timt.m.Fight,
                           GroupFight = timt.m.GroupFight,
                           TeamInMatches = new List<TeamInMatch>
                           {
                               new TeamInMatch
                               {
                                   Id = timt.tim.Id,
                                   TeamScore = timt.tim.TeamScore,
                                   YellowCardNumber = timt.tim.YellowCardNumber,
                                   RedCardNumber = timt.tim.RedCardNumber,
                                   Result = timt.tim.Result,
                                   TeamName = timt.tim.TeamName,
                                   TeamInTournamentId = t.Id,
                                   MatchId = timt.tim.MatchId,
                                   NextTeam = timt.tim.NextTeam,
                                   TeamInTournament = new TeamInTournament
                                   {
                                       Team =t
                                   }
                               }
                           }
                       }).Where(m => m.Id == findMatch[i].Id);
                        var matchQuery = aMatch.ToList();
                        for(int j = 0; j < matchQuery.Count; j++)
                        {
                            matchCheckTeam.Add(matchQuery[j]);
                        }
                    }
                    var matchFull = new List<Match>
                    {
                        matchCheckTeam[0]
                    };
                    var checkTeam = false;
                    for (int i = 0; i < matchCheckTeam.Count; i++)
                    {
                        checkTeam = false;
                        for (int j = 0; j < matchFull.Count; j++)
                        {
                            if (matchFull[j].Id == matchCheckTeam[i].Id)
                            {
                                checkTeam = true;
                                var checkTeamInMatchFull = matchFull[j].TeamInMatches.FirstOrDefault();
                                var checkTeamInMatch = matchCheckTeam[i].TeamInMatches.FirstOrDefault();
                                if (checkTeamInMatchFull!.Id != checkTeamInMatch!.Id)
                                {
                                    matchFull[j].TeamInMatches.Add(matchCheckTeam[i].TeamInMatches.FirstOrDefault()!);
                                }
                                break;
                            }
                            checkTeam = false;
                        }
                        if (checkTeam == false)
                        {
                            matchFull.Add(matchCheckTeam[i]);
                        }
                    }
                    var matchListResponse = new MatchListFVM
                    {

                        Matchs = _mapper.Map<List<Match>, List<MatchFVM>>(matchFull)

                    };
                    return Ok(matchListResponse);
                }

                if(fullInfo == true)
                {
                    listMatch = _matchService.GetList().Join(_teamInMatch.GetList(), m => m.Id, tim => tim.MatchId, (m, tim) => new { m, tim })
                       .Join(_teamService.GetList(), timt => timt.tim.TeamInTournament!.Team, t => t, (timt, t) => new Match
                       {
                           Id = timt.m.Id,
                           MatchDate = timt.m.MatchDate,
                           Status = timt.m.Status,
                           TournamentId = timt.m.TournamentId,
                           Round = timt.m.Round,
                           Fight = timt.m.Fight,
                           GroupFight = timt.m.GroupFight,
                           TeamInMatches = new List<TeamInMatch>
                           {
                               new TeamInMatch
                               {
                                   Id = timt.tim.Id,
                                   TeamScore = timt.tim.TeamScore,
                                   YellowCardNumber = timt.tim.YellowCardNumber,
                                   RedCardNumber = timt.tim.RedCardNumber,
                                   Result = timt.tim.Result,
                                   TeamName = timt.tim.TeamName,
                                   TeamInTournamentId = t.Id,
                                   MatchId = timt.tim.MatchId,
                                   NextTeam = timt.tim.NextTeam,
                                   TeamInTournament = new TeamInTournament
                                   {
                                       Team = t
                                   }
                               }
                           }    
                       }).Where(m => m.TournamentId == tournamentId);
                    var matchFull = new List<Match>();
                    var matchCheckTeam = listMatch.ToList();
                    matchFull.Add(matchCheckTeam[0]);
                    var checkTeam = false;
                    for (int i = 0; i < matchCheckTeam.Count; i++)
                    {
                        checkTeam = false;
                        for (int j = 0; j < matchFull.Count; j++)
                        {
                            if (matchFull[j].Id == matchCheckTeam[i].Id)
                            {
                                checkTeam = true;
                                var checkTeamInMatchFull = matchFull[j].TeamInMatches.FirstOrDefault();
                                var checkTeamInMatch = matchCheckTeam[i].TeamInMatches.FirstOrDefault();
                                if (checkTeamInMatchFull!.Id != checkTeamInMatch!.Id)
                                {
                                    matchFull[j].TeamInMatches.Add(matchCheckTeam[i].TeamInMatches.FirstOrDefault()!);
                                }
                                break;
                            }
                            checkTeam = false;
                        }
                        if (checkTeam == false)
                        {
                            matchFull.Add(matchCheckTeam[i]);
                        }
                    }

                    var matchListResponse = new MatchListFVM
                    {

                        Matchs = _mapper.Map<List<Match>, List<MatchFVM>>(matchFull)

                    };
                    return Ok(matchListResponse);
                }
                var match = new List<Match>();
                var matchCheck = listMatch.ToList();
                match.Add(matchCheck[0]);
                var check = false;
                for(int i=0; i<matchCheck.Count; i++)
                {
                    check = false;
                    for(int j=0; j<match.Count; j++)
                    {
                        if(match[j].Id == matchCheck[i].Id)
                        {
                            check=true;
                            break;
                        }
                        check=false;
                    }
                    if(check == false)
                    {
                        match.Add(matchCheck[i]);   
                    }
                }

               /* match = listMatch.ToList()*/;
                if (match.Count > 0)
                {

                    var matchListResponse = new MatchListFVM
                    {

                        Matchs = _mapper.Map<List<Match>, List<MatchFVM>>(match)

                    };
                    return Ok(matchListResponse);

                }


                return NotFound();
            }
            catch (Exception ex)
            {
                //return StatusCode(StatusCodes.Status500InternalServerError);
                return BadRequest(ex);
            }
        }

        /// <summary>Get list match</summary>
        /// <returns>List match</returns>
        /// <response code="200">Returns list match</response>
        /// <response code="404">Not found match</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<MatchListVM> GetListMatch(
            [FromQuery(Name = "match-date")] string? matchDate,
            [FromQuery(Name = "status")] string? status,
            [FromQuery(Name = "tournament-id")] int? tournamentId,
            [FromQuery(Name = "order-by")] MatchFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<Match> matchList = _matchService.GetList();
                
                if (!String.IsNullOrEmpty(matchDate))
                {
                    matchList = matchList.Where(s => s.MatchDate.Equals(matchDate));
                }
                if (!String.IsNullOrEmpty(status))
                {
                    matchList = matchList.Where(s => s.Status!.ToLower().Equals(status.ToLower()));
                }
                if (!String.IsNullOrEmpty(tournamentId.ToString()))
                {
                    matchList = matchList.Where(s => s.TournamentId == tournamentId);
                }
                
                if (orderBy == MatchFieldEnum.Id)
                {
                    matchList = matchList.OrderBy(tnm => tnm.Id);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        matchList = matchList.OrderByDescending(tnm => tnm.Id);
                    }
                }
                if (orderBy == MatchFieldEnum.MatchDate)
                {
                    matchList = matchList.OrderBy(tnm => tnm.MatchDate);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        matchList = matchList.OrderByDescending(tnm => tnm.MatchDate);
                    }
                }
                if (orderBy == MatchFieldEnum.Status)
                {
                    matchList = matchList.OrderBy(tnm => tnm.Status);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        matchList = matchList.OrderByDescending(tnm => tnm.Status);
                    }
                }
                int countList = matchList.Count();

                var matchListPaging = matchList.Skip((pageIndex - 1) * limit).Take(limit).ToList();
                var matchListResponse = new MatchListVM
                {
                    Matchs = _mapper.Map<List<Match>, List<MatchVM>>(matchListPaging),
                    CountList = countList,
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(matchListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get match by id</summary>
        /// <returns>Return the match with the corresponding id</returns>
        /// <response code="200">Returns the match with the specified id</response>
        /// <response code="404">No match found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<MatchVM>> GetMatchById(int id)
        {
            try
            {
                Match currentMatch = await _matchService.GetByIdAsync(id);
                if (currentMatch != null)
                {
                    return Ok(_mapper.Map<MatchVM>(currentMatch));
                }
                return NotFound("Không thể tìm thấy trận đấu với id là " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new match</summary>
        /// <response code="201">Created new match successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<MatchVM>> CreateMatch([FromBody] MatchCM model)
        {
            Match match = new();
            try
            {
                Tournament tournament = await _tournamentService.GetByIdAsync(model.TournamentId);
                if(tournament == null)
                {
                    return BadRequest("Giải đấu không tồn tại");
                }
                match.TournamentId = model.TournamentId;
                match.MatchDate = model.MatchDate;
                match.Status = String.IsNullOrEmpty(model.Status) ? "Chưa bắt đầu" : model.Status;
                match.Round = String.IsNullOrEmpty(model.Round) ? "" : model.Round;
                match.Fight = String.IsNullOrEmpty(model.Fight) ? "" : model.Fight;
                match.GroupFight = String.IsNullOrEmpty(model.GroupFight) ? "" : model.GroupFight;
                match.TokenLivestream = "";

                Match matchCreated = await _matchService.AddAsync(match);
                if (matchCreated != null)
                {
                    return CreatedAtAction("GetMatchById", new { id = matchCreated.Id }, _mapper.Map<MatchVM>(matchCreated));
                }
                return BadRequest("Tạo trận đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a match</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<MatchVM>> UpdateMatch([FromBody] MatchUM model)
        {
            try
            {
                Match match = await _matchService.GetByIdAsync(model.Id);
                if(match == null)
                {
                    return NotFound("Không tìm thấy trận đấu");
                }
                if (!String.IsNullOrEmpty(model.TournamentId.ToString()))
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync(model.TournamentId!.Value);
                    if (tournament == null)
                    {
                        return BadRequest("Giải đấu không tồn tại");
                    } else
                    {
                        match.TournamentId = (int)model.TournamentId;
                    }
                }
                match.MatchDate = String.IsNullOrEmpty(model.MatchDate.ToString()) ? match.MatchDate : model.MatchDate;
                match.Status = String.IsNullOrEmpty(model.Status) ? match.Status : model.Status;
                match.Round = String.IsNullOrEmpty(model.Round) ? match.Round : model.Round;
                match.Fight = String.IsNullOrEmpty(model.Fight) ? match.Fight : model.Fight;
                match.GroupFight = String.IsNullOrEmpty(model.GroupFight) ? match.GroupFight : model.GroupFight;
                match.TokenLivestream = String.IsNullOrEmpty(model.CreateToken) ? match.TokenLivestream : _agoraProvider.GenerateToken("MATCH_" + model.Id, 0.ToString(), 0);

                bool isUpdated = await _matchService.UpdateAsync(match);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<MatchVM>(match));
                }
                return BadRequest("Cập nhật trận đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete match By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            Match currentMatch = await _matchService.GetByIdAsync(id);
            if (currentMatch == null)
            {
                return NotFound(new
                {
                    message = "Không thể tìm thấy trận đấu với id là " + id
                });
            }
            try
            {
                bool isDeleted = await _matchService.DeleteAsync(currentMatch);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Xóa trận đấu thành công"
                    });
                }
                return BadRequest("Xóa trận đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Schedule the match</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost("schedule")]
        [Produces("application/json")]
        public async Task<ActionResult> ScheduleMatch([FromQuery(Name = "tournament-id")] int tournamentID)
        {
            try
            {
                Tournament tournament = await _tournamentService.GetByIdAsync((int)tournamentID);
                if (tournament == null)
                {
                    return NotFound("Giải đấu không tồn tại");
                }
                int groupNum = tournament.GroupNumber!.Value;
                int numTeam = tournament.FootballTeamNumber!.Value;

                Random rd = new();
                List<int> listTeamNameAllGroup = new();
                List<int> listTeamName = new();
                int totalMatch = 0;
                if (tournament.TournamentTypeId == 1)
                {
                    int n = 0;
                    for (int i = 0; i <= numTeam; i++)
                    {
                        n = (int)Math.Pow(2, i);
                        if (n > numTeam)
                        {
                            i -= 1;
                            n = i;
                            break;
                        }
                    }

                    int numberMatchR1 = (int)(numTeam - Math.Pow(2, n));
                    if(numberMatchR1 == 0)
                    {
                        int round = 1;
                        int fight = 1;
                        totalMatch = (int) numTeam - 1;
                        numTeam /=  2;
                        int winNumber = 1;
                        while (numTeam >= 1)
                        {
                            for (int i = 1; i <= numTeam; i++)
                            {
                                Match match = new();
                                match.TournamentId = tournamentID;
                                match.Status = "Chưa bắt đầu";
                                match.TokenLivestream = "";
                                if (numTeam == 4)
                                {
                                    match.Round = "Tứ kết";
                                }
                                else if (numTeam == 2)
                                {
                                    match.Round = "Bán kết";
                                }
                                else if (numTeam == 1)
                                {
                                    match.Round = "Chung kết";
                                } else
                                {
                                    match.Round = "Vòng " + round;
                                }
                                match.Fight = "Trận " + fight;
                                match.GroupFight = "";
                                Match matchCreated = await _matchService.AddAsync(match);

                                TeamInMatch tim1 = new();
                                tim1.MatchId = matchCreated.Id;
                                int tn1 = rd.Next(1, tournament.FootballTeamNumber.Value + 1);
                                if (round > 1)
                                {
                                    tim1.NextTeam = "Thắng Trận " + winNumber;
                                    tim1.TeamName = "Thắng Trận " + winNumber;
                                    winNumber++;
                                }
                                else
                                {
                                    while (listTeamName.Where(x => x == tn1).Count() == 1)
                                    {
                                        tn1 = rd.Next(1, tournament.FootballTeamNumber.Value + 1);
                                    }
                                    listTeamName.Add(tn1);
                                    tim1.TeamName = "Đội " + tn1.ToString();
                                }
                                await _teamInMatch.AddAsync(tim1);

                                TeamInMatch tim2 = new();
                                tim2.MatchId = matchCreated.Id;
                                if (round > 1)
                                {
                                    tim2.NextTeam = "Thắng Trận " + winNumber;
                                    tim2.TeamName = "Thắng Trận " + winNumber;
                                    winNumber++;
                                }
                                else
                                {
                                    int tn2 = rd.Next(1, tournament.FootballTeamNumber.Value + 1);
                                    while (listTeamName.Where(x => x == tn2).Count() == 1 || tn1 == tn2)
                                    {
                                        tn2 = rd.Next(1, tournament.FootballTeamNumber.Value + 1);
                                    }
                                    listTeamName.Add(tn2);
                                    tim2.TeamName = "Đội " + tn2.ToString();
                                }
                                await _teamInMatch.AddAsync(tim2);

                                fight++;
                            }
                            round++;
                            numTeam /= 2;
                        }
                    }
                    else
                    {
                        totalMatch += numberMatchR1;
                        
                        int round = 1;
                        int fight = 1;
                        int winNumber = 1;
                        for (int i = 1; i <= numberMatchR1; i++)
                        {
                            Match match = new();
                            match.TournamentId = tournamentID;
                            match.Status = "Chưa bắt đầu";
                            match.Round = "Vòng " + round;
                            match.TokenLivestream = "";
                            match.Fight = "Trận " + fight;
                            match.GroupFight = "";
                            Match matchCreated = await _matchService.AddAsync(match);

                            TeamInMatch tim1 = new();
                            tim1.MatchId = matchCreated.Id;
                            int tn1 = rd.Next(1, tournament.FootballTeamNumber.Value + 1);
                            while (listTeamName.Where(x => x == tn1).Count() == 1)
                            {
                                tn1 = rd.Next(1, tournament.FootballTeamNumber.Value + 1);
                            }
                            listTeamName.Add(tn1);
                            tim1.TeamName = "Đội " + tn1.ToString();
                            tim1.NextTeam = "";
                            await _teamInMatch.AddAsync(tim1);

                            TeamInMatch tim2 = new();
                            tim2.MatchId = matchCreated.Id;
                            int tn2 = rd.Next(1, tournament.FootballTeamNumber.Value + 1);
                            while (listTeamName.Where(x => x == tn2).Count() == 1 || tn1 == tn2)
                            {
                                tn2 = rd.Next(1, tournament.FootballTeamNumber.Value + 1);
                            }
                            listTeamName.Add(tn2);
                            tim2.TeamName = "Đội " + tn2.ToString();
                            tim2.NextTeam = "";
                            await _teamInMatch.AddAsync(tim2);
                            fight++;
                        }
                        round++;

                        int numberMatchR2 = ((int)numTeam - totalMatch) / 2;
                        int tmpNumberMatchR2 = numberMatchR2;
                        int numberOfNext = numberMatchR1;
                        totalMatch += numberMatchR2;
                        while (numberMatchR2 >= 1)
                        {
                            for (int i = 1; i <= numberMatchR2; i++)
                            {
                                tmpNumberMatchR2--;
                                Match match = new();
                                match.TournamentId = tournamentID;
                                match.Status = "Chưa bắt đầu";
                                match.TokenLivestream = "";
                                if (numberMatchR2 == 4)
                                {
                                    match.Round = "Tứ kết";
                                }
                                else if (numberMatchR2 == 2)
                                {
                                    match.Round = "Bán kết";
                                }
                                else if (numberMatchR2 == 1)
                                {
                                    match.Round = "Chung kết";
                                }
                                else
                                {
                                    match.Round = "Vòng " + round;
                                }
                                match.Fight = "Trận " + fight;
                                match.GroupFight = "";
                                Match matchCreated = await _matchService.AddAsync(match);

                                TeamInMatch tim1 = new();
                                tim1.MatchId = matchCreated.Id;
                                int tn1 = rd.Next(1, tournament.FootballTeamNumber.Value + 1);
                                tim1.TeamName = "Chưa có đội";
                                if (round > 2)
                                {
                                    tim1.NextTeam = "Thắng Trận " + winNumber;
                                    tim1.TeamName = "Thắng Trận " + winNumber;
                                    winNumber++;
                                }
                                else
                                {
                                    if (round == 2 && numberOfNext > 0)
                                    {
                                        tim1.NextTeam = "Thắng Trận " + winNumber;
                                        tim1.TeamName = "Thắng Trận " + winNumber;
                                        winNumber++;
                                        numberOfNext--;
                                        tn1 = 0;
                                    }
                                    else
                                    {
                                        while (listTeamName.Where(x => x == tn1).Count() == 1)
                                        {
                                            tn1 = rd.Next(1, tournament.FootballTeamNumber.Value + 1);
                                        }
                                        listTeamName.Add(tn1);
                                        tim1.TeamName = "Đội " + tn1.ToString();
                                        tim1.NextTeam = "";
                                    }
                                }
                                await _teamInMatch.AddAsync(tim1);

                                TeamInMatch tim2 = new();
                                tim2.MatchId = matchCreated.Id;
                                int tn2 = rd.Next(1, tournament.FootballTeamNumber.Value + 1);

                                tim2.TeamName = "Chưa có đội";
                                tim2.NextTeam = "";
                                if (round > 2)
                                {
                                    tim2.NextTeam = "Thắng Trận " + winNumber;
                                    tim2.TeamName = "Thắng Trận " + winNumber;
                                    winNumber++;
                                }
                                else
                                {
                                    if (round == 2 && numberOfNext > 0 && numberOfNext > tmpNumberMatchR2 * 2)
                                    {
                                        tim2.NextTeam = "Thắng Trận " + winNumber;
                                        tim2.TeamName = "Thắng Trận " + winNumber;
                                        winNumber++;
                                        numberOfNext--;
                                    }
                                    else
                                    {
                                        while (listTeamName.Where(x => x == tn2).Count() == 1 || tn1 == tn2)
                                        {
                                            tn2 = rd.Next(1, tournament.FootballTeamNumber.Value + 1);
                                        }
                                        listTeamName.Add(tn2);
                                        tim2.TeamName = "Đội " + tn2.ToString();
                                    }
                                }
                                
                                await _teamInMatch.AddAsync(tim2);

                                fight++;
                            }
                            round++;
                            numberMatchR2 /= 2;
                            if(numberMatchR2 >= 1)
                            {
                                totalMatch += numberMatchR2;
                            }
                        }
                    }
                }
                else if (tournament.TournamentTypeId == 2)
                {
                    for (int i = 1; i < numTeam; i++)
                    {
                        totalMatch += i;
                    }
                    List<string> listTest = new();
                    
                    for (int i = 1; i <= totalMatch; i++)
                    {
                        int tn1 = rd.Next(1, numTeam + 1);
                        if (listTeamName.Count == 0)
                        {
                            listTeamName.Add(tn1);
                        }
                        else
                        {
                            while (listTeamName.Where(x => x == tn1).Count() == numTeam - 1)
                            {
                                tn1 = rd.Next(1, numTeam + 1);
                            }
                        }

                        int tn2 = rd.Next(1, numTeam + 1);
                        while (listTeamName.Where(x => x == tn2).Count() == numTeam - 1 || tn2 == tn1)
                        {
                            tn2 = rd.Next(1, numTeam + 1);
                        }

                        if (listTest.Count == 0)
                        {
                            listTest.Add(tn1.ToString() + tn2.ToString());
                            listTest.Add(tn2.ToString() + tn1.ToString());

                            Match match = new();
                            match.TournamentId = tournamentID;
                            match.Status = "Chưa bắt đầu";
                            match.Round = "";
                            match.TokenLivestream = "";
                            match.GroupFight = "";
                            match.Fight = "Trận " + i;
                            Match matchCreated = await _matchService.AddAsync(match);

                            TeamInMatch tim1 = new();
                            tim1.MatchId = matchCreated.Id;
                            tim1.TeamName = "Chưa có đội";
                            tim1.TeamName = "Đội " + tn1.ToString();
                            await _teamInMatch.AddAsync(tim1);

                            TeamInMatch tim2 = new();
                            tim2.TeamName = "Chưa có đội";
                            tim2.MatchId = matchCreated.Id;
                            tim2.TeamName = "Đội " + tn2.ToString();
                            await _teamInMatch.AddAsync(tim2);
                        }
                        else
                        {
                            bool flag = false;
                            foreach (string test in listTest)
                            {
                                if (test.Equals(tn1.ToString() + tn2.ToString()))
                                {
                                    flag = true;
                                }
                            }
                            if (flag)
                            {
                                i--;
                            }
                            else
                            {
                                listTest.Add(tn1.ToString() + tn2.ToString());
                                listTest.Add(tn2.ToString() + tn1.ToString());

                                Match match = new();
                                match.TournamentId = tournamentID;
                                match.Status = "Chưa bắt đầu";
                                match.Round = "";
                                match.TokenLivestream = "";
                                match.GroupFight = "";
                                match.Fight = "Trận " + i;
                                Match matchCreated = await _matchService.AddAsync(match);

                                TeamInMatch tim1 = new();
                                tim1.MatchId = matchCreated.Id;
                                tim1.TeamName = "Chưa có đội";
                                tim1.TeamName = "Đội " + tn1.ToString();
                                await _teamInMatch.AddAsync(tim1);

                                TeamInMatch tim2 = new();
                                tim2.TeamName = "Chưa có đội";
                                tim2.MatchId = matchCreated.Id;
                                tim2.TeamName = "Đội " + tn2.ToString();
                                await _teamInMatch.AddAsync(tim2);
                            }
                        }
                    }
                }
                else
                {
                    int numberTeamOfGroup = (int) (numTeam / groupNum);
                    int numberTeamRemain = (int) numTeam -  numberTeamOfGroup * groupNum;
                    int table = 65;
                    for (int i = 1; i <= numTeam; i++)
                    {
                        listTeamNameAllGroup.Add(i);
                    }
                for (int i = 1; i <= groupNum; i++)
                    {
                        int totalMatchInGroup = 0;
                        int realNumTeamInGroup = numberTeamOfGroup;
                        if (numberTeamRemain > 0)
                        {
                            for (int j = 1; j < numberTeamOfGroup + 1; j++)
                            {
                                totalMatchInGroup += j;
                                totalMatch += j;
                            }
                            realNumTeamInGroup++;
                            numberTeamRemain--;
                        }
                        else
                        {
                            for (int j = 1; j < numberTeamOfGroup; j++)
                            {
                                totalMatchInGroup += j;
                                totalMatch += j;
                            }
                        }
                        List<int> listTeamNameGroup = new();
                        for (int j = 1; j <= realNumTeamInGroup; j++)
                        {
                            int pos = rd.Next(0, numTeam);
                            listTeamNameGroup.Add(listTeamNameAllGroup[pos]);
                            listTeamNameAllGroup.RemoveAt(pos);
                            numTeam--;
                        }
                        List<string> listTest = new();
                        for (int j = 1; j <= totalMatchInGroup; j++)
                        {
                            int pos1 = rd.Next(0, realNumTeamInGroup);
                            int tn1 = listTeamNameGroup[pos1];
                            if (listTeamName.Count == 0)
                            {
                                listTeamName.Add(tn1);
                            }
                            else
                            {
                                while (listTeamName.Where(x => x == tn1).Count() == realNumTeamInGroup - 1)
                                {
                                    pos1 = rd.Next(0, realNumTeamInGroup);
                                    tn1 = listTeamNameGroup[pos1];
                                }
                            }

                            int pos2 = rd.Next(0, realNumTeamInGroup);
                            int tn2 = listTeamNameGroup[pos2];
                            while (listTeamName.Where(x => x == tn2).Count() == realNumTeamInGroup - 1 || tn2 == tn1)
                            {
                                pos2 = rd.Next(0, realNumTeamInGroup);
                                tn2 = listTeamNameGroup[pos2];
                            }

                            if (listTest.Count == 0)
                            {
                                listTest.Add(tn1.ToString() + tn2.ToString());
                                listTest.Add(tn2.ToString() + tn1.ToString());

                                Match match = new();
                                match.TournamentId = tournamentID;
                                match.Status = "Chưa bắt đầu";
                                match.Round = "Vòng bảng";
                                match.TokenLivestream = "";
                                match.GroupFight = "Bảng " + char.ConvertFromUtf32(table);
                                match.Fight = "";
                                Match matchCreated = await _matchService.AddAsync(match);

                                TeamInMatch tim1 = new();
                                tim1.MatchId = matchCreated.Id;
                                tim1.TeamName = "Đội " + tn1.ToString();
                                await _teamInMatch.AddAsync(tim1);

                                TeamInMatch tim2 = new();
                                tim2.MatchId = matchCreated.Id;
                                tim2.TeamName = "Đội " + tn2.ToString();
                                await _teamInMatch.AddAsync(tim2);
                            }
                            else
                            {
                                bool flag = false;
                                foreach (string test in listTest)
                                {
                                    if (test.Equals(tn1.ToString() + tn2.ToString()))
                                    {
                                        flag = true;
                                    }
                                }
                                if (flag)
                                {
                                    j--;
                                }
                                else
                                {
                                    listTest.Add(tn1.ToString() + tn2.ToString());
                                    listTest.Add(tn2.ToString() + tn1.ToString());

                                    Match match = new();
                                    match.TournamentId = tournamentID;
                                    match.Status = "Chưa bắt đầu";
                                    match.Round = "Vòng bảng";
                                    match.TokenLivestream = "";
                                    match.GroupFight = "Bảng " + char.ConvertFromUtf32(table);
                                    match.Fight = "";
                                    Match matchCreated = await _matchService.AddAsync(match);

                                    TeamInMatch tim1 = new();
                                    tim1.MatchId = matchCreated.Id;
                                    tim1.TeamName = "Đội " + tn1.ToString();
                                    await _teamInMatch.AddAsync(tim1);

                                    TeamInMatch tim2 = new();
                                    tim2.MatchId = matchCreated.Id;
                                    tim2.TeamName = "Đội " + tn2.ToString();
                                    await _teamInMatch.AddAsync(tim2);
                                }
                            }
                        }
                        table++;
                    }

                    int round = 1;
                    int fight = 1;
                    totalMatch += (int)groupNum * 2 - 1;
                    int winNumber = 1;
                    while (groupNum >= 1)
                    {
                        for (int i = 1; i <= groupNum; i++)
                        {
                            Match match = new();
                            match.TournamentId = tournamentID;
                            match.Status = "Chưa bắt đầu";
                            match.TokenLivestream = "";
                            match.GroupFight = "";
                            if (groupNum == 4)
                            {
                                match.Round = "Tứ kết";
                                match.GroupFight = "Tứ kết";
                            }
                            if (groupNum == 2)
                            {
                                match.Round = "Bán kết";
                                match.GroupFight = "Bán kết";
                            }
                            else if (groupNum == 1)
                            {
                                match.Round = "Chung kết";
                                match.GroupFight = "Chung kết";
                            }
                            else
                            {
                                match.Round = "Vòng " + round;
                            }
                            match.Fight = "Trận " + fight;
                            
                            Match matchCreated = await _matchService.AddAsync(match);

                            TeamInMatch tim1 = new();
                            tim1.MatchId = matchCreated.Id;
                            if (round > 1)
                            {
                                tim1.NextTeam = "Thắng Trận " + winNumber;
                                tim1.TeamName = "Thắng Trận " + winNumber;
                                winNumber++;
                            }
                            else
                            {
                                if(groupNum == 2)
                                {
                                    if(i == 1)
                                    {
                                        tim1.TeamName = "Nhất bảng A";
                                    }
                                    else
                                    {
                                        tim1.TeamName = "Nhì bảng A";
                                    }
                                }
                                else
                                {
                                    if (i == 1)
                                    {
                                        tim1.TeamName = "Nhất bảng A";
                                    }
                                    else if (i == 2)
                                    {
                                        tim1.TeamName = "Nhì bảng A";
                                    }
                                    else if (i == 3)
                                    {
                                        tim1.TeamName = "Nhất bảng C";
                                    }
                                    else
                                    {
                                        tim1.TeamName = "Nhì bảng C";
                                    }
                                }
                            }
                            await _teamInMatch.AddAsync(tim1);

                            TeamInMatch tim2 = new();
                            tim2.MatchId = matchCreated.Id;
                            if (round > 1)
                            {
                                tim2.NextTeam = "Thắng Trận " + winNumber;
                                tim2.TeamName = "Thắng Trận " + winNumber;
                                winNumber++;
                            }
                            else
                            {
                                if (groupNum == 2)
                                {
                                    if (i == 1)
                                    {
                                        tim2.TeamName = "Nhì bảng B";
                                    }
                                    else
                                    {
                                        tim2.TeamName = "Nhất bảng B";
                                    }
                                }
                                else
                                {
                                    if (i == 1)
                                    {
                                        tim2.TeamName = "Nhì bảng B";
                                    }
                                    else if (i == 2)
                                    {
                                        tim2.TeamName = "Nhất bảng B";
                                    }
                                    else if (i == 3)
                                    {
                                        tim2.TeamName = "Nhì bảng D";
                                    }
                                    else
                                    {
                                        tim2.TeamName = "Nhất bảng D";
                                    }
                                }
                            }
                            await _teamInMatch.AddAsync(tim2);

                            fight++;
                        }
                        round++;
                        groupNum /= 2;
                    }
                }

                return Ok(totalMatch);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete match By Tournament Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("delete-match-by-tournament-id")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteMatchByTournamrntId(int tournamentId)
        {
            try
            {
                List<Match> listMatch = _matchService.GetList().Where(t => t.TournamentId == tournamentId).ToList();
                foreach (Match match in listMatch)
                {
                    await _matchService.DeleteAsync(match);
                }
                return Ok("Xóa trận đấu trong giải đấu thành công");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
