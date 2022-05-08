using AmateurFootballLeague.ExternalService;
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
    [Route("api/v1/news")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly ITournamentService _tournamentService;
        private readonly IMapper _mapper;

        public NewsController(INewsService newsService, ITournamentService tournamentService, IMapper mapper)
        {
            _newsService = newsService;
            _tournamentService = tournamentService;
            _mapper = mapper;
        }

        /// <summary>Get list news</summary>
        /// <returns>List news</returns>
        /// <response code="200">Returns list news</response>
        /// <response code="404">Not found news</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<NewsListVM> GetListNews(
            [FromQuery(Name = "content")] string? content,
            [FromQuery(Name = "order-by")] NewsFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<News> newsList = _newsService.GetList();
                if (!String.IsNullOrEmpty(content))
                {
                    newsList = newsList.Where(s => s.Content.ToUpper().Contains(content.Trim().ToUpper()));
                }

                var newsListPaging = newsList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var newsListOrder = new List<News>();
                if (orderBy == NewsFieldEnum.Id)
                {
                    newsListOrder = newsListPaging.OrderBy(tnm => tnm.Id).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        newsListOrder = newsListPaging.OrderByDescending(tnm => tnm.Id).ToList();
                    }
                }
                if (orderBy == NewsFieldEnum.Content)
                {
                    newsListOrder = newsListPaging.OrderBy(tnm => tnm.Content).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        newsListOrder = newsListPaging.OrderByDescending(tnm => tnm.Content).ToList();
                    }
                }

                var newsListResponse = new NewsListVM
                {
                    News = _mapper.Map<List<News>, List<NewsVM>>(newsListOrder),
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(newsListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get news by id</summary>
        /// <returns>Return the news with the corresponding id</returns>
        /// <response code="200">Returns the news with the specified id</response>
        /// <response code="404">No news found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<NewsVM>> GetNewsById(int id)
        {
            try
            {
                News currentNews = await _newsService.GetByIdAsync(id);
                if (currentNews != null)
                {
                    return Ok(_mapper.Map<NewsVM>(currentNews));
                }
                return NotFound("Can not found news by id: " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new news</summary>
        /// <response code="201">Created new news successfull</response>
        /// <response code="400">Field is not newsed or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<NewsVM>> CreateNews(
                [FromQuery(Name = "content")] string content,
                [FromQuery(Name = "tournament-id")] int tournamentID
            )
        {
            News news = new News();
            try
            {
                Tournament tournament = await _tournamentService.GetByIdAsync(tournamentID);
                if (tournament == null)
                {
                    return BadRequest("Tournament not exist");
                }
                news.Content = content;
                news.Status = true;
                news.DateCreate = DateTime.Now;

                News newsCreated = await _newsService.AddAsync(news);
                if (newsCreated != null)
                {
                    return CreatedAtAction("GetNewsById", new { id = newsCreated.Id }, _mapper.Map<NewsVM>(newsCreated));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a news</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not newsed</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<NewsVM>> UpdateNews(
                [FromQuery(Name = "content")] string? content,
                [FromQuery(Name = "tournament-id")] int? tournamentID
            )
        {
            News news = new News();
            try
            {
                if (!String.IsNullOrEmpty(tournamentID.ToString()))
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync((int)tournamentID);
                    if (tournament == null)
                    {
                        return BadRequest("Tournament not exist");
                    }
                    else
                    {
                        news.TournamentId = (int)tournamentID;
                    }
                }
                news.Content = String.IsNullOrEmpty(content) ? news.Content : content;
                news.DateUpdate = DateTime.Now;

                bool isUpdated = await _newsService.UpdateAsync(news);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<NewsVM>(news));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Change status news By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("{id}")]
        public async Task<ActionResult> ChangeStatusNews(int id)
        {
            News currentNews = await _newsService.GetByIdAsync(id);
            if (currentNews == null)
            {
                return NotFound(new
                {
                    message = "Can not found news by id: " + id
                });
            }
            try
            {
                currentNews.Status = !currentNews.Status;
                bool isDeleted = await _newsService.UpdateAsync(currentNews);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Success"
                    });
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete news By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            News currentNews = await _newsService.GetByIdAsync(id);
            if (currentNews == null)
            {
                return NotFound(new
                {
                    message = "Can not found news by id: " + id
                });
            }
            try
            {
                currentNews.Status = false;
                currentNews.DateDelete = DateTime.Now;

                bool isUpdated = await _newsService.UpdateAsync(currentNews);
                if (isUpdated)
                {
                    return Ok(new
                    {
                        message = "Success"
                    });
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
