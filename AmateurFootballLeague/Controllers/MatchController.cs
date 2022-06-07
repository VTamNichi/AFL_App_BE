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

        public MatchController(IMatchService matchService, ITeamInMatchService teamInMatch, ITournamentService tournamentService, IAgoraProvider agoraProvider, IMapper mapper, ITeamService teamService)
        {
            _matchService = matchService;
            _teamInMatch = teamInMatch;
            _tournamentService = tournamentService;
            _agoraProvider = agoraProvider;
            _mapper = mapper;
            _teamService = teamService;
        }

        /// <summary>Get list match</summary>
        /// <returns>List match</returns>
        /// <response code="200">Returns list match</response>
        /// <response code="404">Not found match</response>
        /// <response code="500">Internal server error</response>
        /// 

        [HttpGet]
        [Route("TournamentID")]
        public ActionResult<MatchListFVM> GetAllMatchByTournamentID (int tournamentId, bool? fullInfo, SortTypeEnum orderType)
        {
            try
            {
                IQueryable<Match> listMatch = _matchService.GetList().Join(_teamInMatch.GetList(), m => m.Id, tim => tim.MatchId, (m, tim) => new Match
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

                if(fullInfo == true)
                {
                    listMatch = _matchService.GetList().Join(_teamInMatch.GetList(), m => m.Id, tim => tim.MatchId, (m, tim) => new { m, tim })
                       .Join(_teamService.GetList(), timt => timt.tim.Team, t => t, (timt, t) => new Match
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
                                   TeamId = t.Id,
                                   MatchId = timt.tim.MatchId,
                                   NextTeam = timt.tim.NextTeam,
                                   Team = t
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
                                if (checkTeamInMatchFull.Id != checkTeamInMatch.Id)
                                {
                                    matchFull[j].TeamInMatches.Add(matchCheckTeam[i].TeamInMatches.FirstOrDefault());
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
                if (match.Count() > 0)
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


        [HttpGet]
        [Produces("application/json")]
        public ActionResult<MatchListVM> GetListMatch(
            [FromQuery(Name = "match-date")] string? matchDate,
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

                var matchListPaging = matchList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var matchListOrder = new List<Match>();
                if (orderBy == MatchFieldEnum.Id)
                {
                    matchListOrder = matchListPaging.OrderBy(tnm => tnm.Id).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        matchListOrder = matchListPaging.OrderByDescending(tnm => tnm.Id).ToList();
                    }
                }
                if (orderBy == MatchFieldEnum.MatchDate)
                {
                    matchListOrder = matchListPaging.OrderBy(tnm => tnm.MatchDate).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        matchListOrder = matchListPaging.OrderByDescending(tnm => tnm.MatchDate).ToList();
                    }
                }
                if (orderBy == MatchFieldEnum.Status)
                {
                    matchListOrder = matchListPaging.OrderBy(tnm => tnm.Status).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        matchListOrder = matchListPaging.OrderByDescending(tnm => tnm.Status).ToList();
                    }
                }

                var matchListResponse = new MatchListVM
                {
                    Matchs = _mapper.Map<List<Match>, List<MatchVM>>(matchListOrder),
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
        public async Task<ActionResult<MatchVM>> CreateMatch(
                [FromQuery(Name = "match-date")] DateTime matchDate,
                [FromQuery(Name = "status")] MatchStatusEnum status,
                [FromQuery(Name = "tournament-id")] int tournamentID,
                [FromQuery(Name = "round")] string? round,
                [FromQuery(Name = "fight")] string? fight,
                [FromQuery(Name = "group-fight")] string? groupFight
            )
        {
            Match match = new Match();
            try
            {
                Tournament tournament = await _tournamentService.GetByIdAsync(tournamentID);
                if(tournament == null)
                {
                    return BadRequest("Giải đấu không tồn tại");
                }
                match.TournamentId = tournamentID;
                match.MatchDate = matchDate;
                match.Status = status == MatchStatusEnum.NotStart ? "Not start" : status == MatchStatusEnum.Processing ? "Process" : "Finished";
                match.Round = String.IsNullOrEmpty(round) ? "" : round;
                match.Fight = String.IsNullOrEmpty(fight) ? "" : fight;
                match.GroupFight = String.IsNullOrEmpty(groupFight) ? "" : groupFight;
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
        public async Task<ActionResult<MatchVM>> UpdateMatch(
                [FromQuery(Name = "match-id")] int matchId,
                [FromQuery(Name = "match-date")] DateTime? matchDate,
                [FromQuery(Name = "status")] MatchStatusEnum? status,
                [FromQuery(Name = "tournament-id")] int? tournamentID,
                [FromQuery(Name = "round")] string? round,
                [FromQuery(Name = "fight")] string? fight,
                [FromQuery(Name = "group-fight")] string? groupFight,
                [FromQuery(Name = "create-token")] string? createToken
            )
        {
            try
            {
                Match match = await _matchService.GetByIdAsync(matchId);
                if(match == null)
                {
                    return NotFound("Không tìm thấy trận đấu");
                }
                if (!String.IsNullOrEmpty(tournamentID.ToString()))
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync((int)tournamentID);
                    if (tournament == null)
                    {
                        return BadRequest("Giải đấu không tồn tại");
                    } else
                    {
                        match.TournamentId = (int)tournamentID;
                    }
                }
                match.MatchDate = String.IsNullOrEmpty(matchDate.ToString()) ? match.MatchDate : matchDate;
                match.Status = status == MatchStatusEnum.NotStart ? "Not start" : status == MatchStatusEnum.Processing ? "Processing" : status == MatchStatusEnum.Finished ? "Finished" : match.Status;
                match.Round = String.IsNullOrEmpty(round) ? match.Round : round;
                match.Fight = String.IsNullOrEmpty(fight) ? match.Fight : fight;
                match.GroupFight = String.IsNullOrEmpty(groupFight) ? match.GroupFight : groupFight;
                match.TokenLivestream = String.IsNullOrEmpty(createToken) ? match.TokenLivestream : _agoraProvider.GenerateToken("MATCH_" + matchId, 0.ToString(), 0);

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
        public async Task<ActionResult> ScheduleMatch(
                [FromQuery(Name = "tournament-id")] int tournamentID
            )
        {
            try
            {
                Tournament tournament = await _tournamentService.GetByIdAsync((int)tournamentID);
                if (tournament == null)
                {
                    return BadRequest("Giải đấu không tồn tại");
                }
                int totalMatch = 0;
                if (tournament.TournamentTypeId == 1)
                {
                    int n = 0;
                    for (int i = 0; i <= tournament.FootballTeamNumber; i++)
                    {
                        n = (int)Math.Pow(2, i);
                        if (n > tournament.FootballTeamNumber)
                        {
                            i -= 1;
                            n = i;
                            break;
                        }
                    }

                    int numberMatchR1 = (int)(tournament.FootballTeamNumber - Math.Pow(2, n));
                    if(numberMatchR1 == 0)
                    {
                        int round = 1;
                        int fight = 1;
                        totalMatch = (int) tournament.FootballTeamNumber - 1;
                        tournament.FootballTeamNumber = tournament.FootballTeamNumber / 2;
                        int winNumber = 1;
                        while (tournament.FootballTeamNumber >= 1)
                        {
                            for (int i = 1; i <= tournament.FootballTeamNumber; i++)
                            {
                                Match match = new Match();
                                match.TournamentId = tournamentID;
                                match.Status = "Not start";
                                match.TokenLivestream = "";
                                if (tournament.FootballTeamNumber == 2)
                                {
                                    match.Round = "Bán kết";
                                } else if (tournament.FootballTeamNumber == 1)
                                {
                                    match.Round = "Chung kết";
                                } else
                                {
                                    match.Round = "Vòng " + round;
                                }
                                match.Fight = "Trận " + fight;
                                match.GroupFight = "";

                                Match matchCreated = await _matchService.AddAsync(match);

                                TeamInMatch tim1 = new TeamInMatch();
                                tim1.MatchId = matchCreated.Id;
                                tim1.TeamName = "Chưa có đội";
                                if(round > 1)
                                {
                                    tim1.NextTeam = "Thắng Trận " + winNumber;
                                    winNumber++;
                                }
                                await _teamInMatch.AddAsync(tim1);

                                TeamInMatch tim2 = new TeamInMatch();
                                tim2.MatchId = matchCreated.Id;
                                tim2.TeamName = "Chưa có đội";
                                if (round > 1)
                                {
                                    tim2.NextTeam = "Thắng Trận " + winNumber;
                                    winNumber++;
                                }
                                await _teamInMatch.AddAsync(tim2);

                                fight++;
                            }
                            round++;
                            tournament.FootballTeamNumber = tournament.FootballTeamNumber / 2;
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
                            Match match = new Match();
                            match.TournamentId = tournamentID;
                            match.Status = "Not start";
                            match.Round = "Vòng " + round;
                            match.TokenLivestream = "";
                            match.Fight = "Trận " + fight;
                            match.GroupFight = "";

                            Match matchCreated = await _matchService.AddAsync(match);

                            TeamInMatch tim1 = new TeamInMatch();
                            tim1.MatchId = matchCreated.Id;
                            tim1.TeamName = "Chưa có đội";
                            tim1.NextTeam = "";
                            await _teamInMatch.AddAsync(tim1);

                            TeamInMatch tim2 = new TeamInMatch();
                            tim2.MatchId = matchCreated.Id;
                            tim2.TeamName = "Chưa có đội";
                            tim2.NextTeam = "";
                            await _teamInMatch.AddAsync(tim2);
                            fight++;
                        }
                        round++;

                        int numberMatchR2 = ((int)tournament.FootballTeamNumber - totalMatch) / 2;
                        int tmpNumberMatchR2 = numberMatchR2;
                        int numberOfNext = numberMatchR1;
                        totalMatch += numberMatchR2;
                        while (numberMatchR2 >= 1)
                        {
                            for (int i = 1; i <= numberMatchR2; i++)
                            {
                                Match match = new Match();
                                match.TournamentId = tournamentID;
                                match.Status = "Not start";
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

                                TeamInMatch tim1 = new TeamInMatch();
                                tim1.MatchId = matchCreated.Id;
                                tim1.TeamName = "Chưa có đội";
                                if (round == 2 && numberOfNext > 0)
                                {
                                    tim1.NextTeam = "Thắng Trận " + winNumber;
                                    winNumber++;
                                    numberOfNext--;
                                } else
                                {
                                    tim1.NextTeam = "";
                                }
                                if (round > 2)
                                {
                                    tim1.NextTeam = "Thắng Trận " + winNumber;
                                    winNumber++;
                                }                               
                                await _teamInMatch.AddAsync(tim1);

                                TeamInMatch tim2 = new TeamInMatch();
                                tim2.MatchId = matchCreated.Id;
                                tim2.TeamName = "Chưa có đội";
                                tim2.NextTeam = "";
                                if (round > 2)
                                {
                                    tim2.NextTeam = "Thắng Trận " + winNumber;
                                    winNumber++;
                                }
                                if (round == 2 && numberOfNext > 0 && numberMatchR1 > tmpNumberMatchR2)
                                {
                                    tim2.NextTeam = "Thắng Trận " + winNumber;
                                    winNumber++;
                                    numberOfNext--;
                                }
                                await _teamInMatch.AddAsync(tim2);

                                fight++;
                            }
                            round++;
                            numberMatchR2 = numberMatchR2 / 2;
                            if(numberMatchR2 >= 1)
                            {
                                totalMatch += numberMatchR2;
                            }
                        }

                    }

                }
                else if (tournament.TournamentTypeId == 2)
                {
                    for (int i = 1; i < tournament.FootballTeamNumber; i++)
                    {
                        totalMatch += i;
                    }
                    for (int i = 1; i <= totalMatch; i++)
                    {
                        Match match = new Match();
                        match.TournamentId = tournamentID;
                        match.Status = "Not start";
                        match.Round = "";
                        match.TokenLivestream = "";
                        match.GroupFight = "";
                        match.Fight = "Trận " + i;

                        Match matchCreated = await _matchService.AddAsync(match);
                        //Random rd = new Random();

                        TeamInMatch tim1 = new TeamInMatch();
                        //int tn1 = rd.Next(1, tournament.FootballTeamNumber.Value);
                        tim1.MatchId = matchCreated.Id;
                        tim1.TeamName = "Chưa có đội";
//                        tim1.TeamName = tn1;
                        await _teamInMatch.AddAsync(tim1);

                        TeamInMatch tim2 = new TeamInMatch();
                        tim2.TeamName = "Chưa có đội";
                        tim2.MatchId = matchCreated.Id;
  //                      tim2.TeamName = tn2;
                        await _teamInMatch.AddAsync(tim2);
                    }

                }
                else
                {
                    int numberTeamOfGroup = (int) (tournament.FootballTeamNumber / tournament.GroupNumber);
                    int numberTeamRemain = (int) tournament.FootballTeamNumber % numberTeamOfGroup;
                    int table = 65;
                    for (int i = 1; i <= tournament.GroupNumber; i++)
                    {
                        int totalMatchInGroup = 0;
                        if(numberTeamRemain > 0)
                        {
                            for (int j = 1; j < numberTeamOfGroup + 1; j++)
                            {
                                totalMatchInGroup += j;
                                totalMatch += j;
                            }
                            numberTeamRemain--;
                        } else
                        {
                            for (int j = 1; j < numberTeamOfGroup; j++)
                            {
                                totalMatchInGroup += j;
                                totalMatch += j;
                            }
                        }

                        for (int j = 1; j <= totalMatchInGroup; j++)
                        {
                            Match match = new Match();
                            match.TournamentId = tournamentID;
                            match.Status = "Not start";
                            match.Round = "Vòng bảng";
                            match.TokenLivestream = "";
                            match.GroupFight = "Bảng " + char.ConvertFromUtf32(table);
                            match.Fight = "";

                            Match matchCreated = await _matchService.AddAsync(match);

                            TeamInMatch tim1 = new TeamInMatch();
                            tim1.MatchId = matchCreated.Id;
                            tim1.TeamName = "Chưa có đội";
                            await _teamInMatch.AddAsync(tim1);

                            TeamInMatch tim2 = new TeamInMatch();
                            tim2.MatchId = matchCreated.Id;
                            tim2.TeamName = "Chưa có đội";
                            await _teamInMatch.AddAsync(tim2);
                        }
                        table++;
                    }

                    int round = 1;
                    int fight = 1;
                    totalMatch += (int)tournament.GroupNumber * 2 - 1;
                    int winNumber = 1;
                    while (tournament.GroupNumber >= 1)
                    {
                        for (int i = 1; i <= tournament.GroupNumber; i++)
                        {
                            Match match = new Match();
                            match.TournamentId = tournamentID;
                            match.Status = "Not start";
                            match.TokenLivestream = "";
                            match.GroupFight = "";
                            if (tournament.GroupNumber == 4)
                            {
                                match.Round = "Tứ kết";
                                match.GroupFight = "Tứ kết";
                            }
                            if (tournament.GroupNumber == 2)
                            {
                                match.Round = "Bán kết";
                                match.GroupFight = "Bán kết";
                            }
                            else if (tournament.GroupNumber == 1)
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

                            TeamInMatch tim1 = new TeamInMatch();
                            tim1.MatchId = matchCreated.Id;
                            tim1.TeamName = "Chưa có đội";
                            if (round > 1)
                            {
                                tim1.NextTeam = "Thắng Trận " + winNumber;
                                winNumber++;
                            }
                            await _teamInMatch.AddAsync(tim1);

                            TeamInMatch tim2 = new TeamInMatch();
                            tim2.MatchId = matchCreated.Id;
                            tim2.TeamName = "Chưa có đội";
                            if (round > 1)
                            {
                                tim2.NextTeam = "Thắng Trận " + winNumber;
                                winNumber++;
                            }
                            await _teamInMatch.AddAsync(tim2);

                            fight++;
                        }
                        round++;
                        tournament.GroupNumber = tournament.GroupNumber / 2;
                    }
                }

                return Ok(totalMatch);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
