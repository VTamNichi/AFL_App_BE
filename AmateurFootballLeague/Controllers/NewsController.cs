using AmateurFootballLeague.ExternalService;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
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
        private readonly IUploadFileService _uploadFileService;

        public NewsController(INewsService newsService, ITournamentService tournamentService, IMapper mapper, IUploadFileService uploadFileService)
        {
            _newsService = newsService;
            _tournamentService = tournamentService;
            _mapper = mapper;
            _uploadFileService = uploadFileService;
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
            [FromQuery(Name = "tournament-id")] int? tourId,
            [FromQuery(Name = "status")] bool? status,
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
                if (!String.IsNullOrEmpty(tourId.ToString()))
                {
                    newsList = newsList.Where(s => s.TournamentId == tourId);
                }
                if (!String.IsNullOrEmpty(status.ToString()))
                {
                    newsList = newsList.Where(s => s.Status == status);
                }

                if (orderBy == NewsFieldEnum.Id)
                {
                    newsList = newsList.OrderBy(tnm => tnm.Id);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        newsList = newsList.OrderByDescending(tnm => tnm.Id);
                    }
                }
                if (orderBy == NewsFieldEnum.Content)
                {
                    newsList = newsList.OrderBy(tnm => tnm.Content);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        newsList = newsList.OrderByDescending(tnm => tnm.Content);
                    }
                }

                int countList = newsList.Count();

                var newsListPaging = newsList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var newsListResponse = new NewsListVM
                {
                    News = _mapper.Map<List<News>, List<NewsVM>>(newsListPaging),
                    CountList = countList,
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
                return NotFound("Không thể tìm thấy bản tin với id là " + id);
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
        public async Task<ActionResult<NewsVM>> CreateNews([FromForm] NewsCM model)
        {
            News news = new News();
            try
            {
                Tournament tournament = await _tournamentService.GetByIdAsync(model.TournamentId);
                if (tournament == null)
                {
                    return NotFound("Giải đấu không tồn tại");
                }
                try
                {
                    if (!String.IsNullOrEmpty(model.NewsImage.ToString()))
                    {
                        string fileUrl = await _uploadFileService.UploadFile(model.NewsImage, "images", "image-url");
                        news.NewsImage = fileUrl;
                    }
                }
                catch (Exception)
                {
                    news.NewsImage = "https://t3.ftcdn.net/jpg/03/46/83/96/360_F_346839683_6nAPzbhpSkIpb8pmAwufkC7c5eD7wYws.jpg";
                }
                news.TournamentId = model.TournamentId;
                news.Content = model.Content;
                news.Status = true;
                news.DateCreate = DateTime.Now;

                News newsCreated = await _newsService.AddAsync(news);
                if (newsCreated != null)
                {
                    return CreatedAtAction("GetNewsById", new { id = newsCreated.Id }, _mapper.Map<NewsVM>(newsCreated));
                }
                return BadRequest("Tạo bản tin thất bại");
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
        public async Task<ActionResult<NewsVM>> UpdateNews([FromForm] NewsUM model)
        {
            try
            {
                News oldNews = await _newsService.GetByIdAsync(model.Id);
                if(oldNews == null)
                {
                    return NotFound("Không tìm thấy bản tin");
                }

                if (!String.IsNullOrEmpty(model.TournamentId.ToString()))
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync((int)model.TournamentId);
                    if (tournament == null)
                    {
                        return BadRequest("Giải đấu không tồn tại");
                    }
                    else
                    {
                        oldNews.TournamentId = (int)model.TournamentId;
                    }
                }
                try
                {
                    if (!String.IsNullOrEmpty(model.NewsImage.ToString()))
                    {
                        string fileUrl = await _uploadFileService.UploadFile(model.NewsImage, "images", "image-url");
                        oldNews.NewsImage = fileUrl;
                    }
                }
                catch (Exception) {}

                oldNews.Content = String.IsNullOrEmpty(model.Content) ? oldNews.Content : model.Content;
                oldNews.DateUpdate = DateTime.Now;

                bool isUpdated = await _newsService.UpdateAsync(oldNews);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<NewsVM>(oldNews));
                }
                return BadRequest("Cập nhật bản tin thất bại");
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
                    message = "Không thể tìm thấy bản tin với id là " + id
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
                        message = "Thay đổi trạng thái bản tin thành công"
                    });
                }
                return BadRequest("Thay đổi trạng thái bản tin thất bại");
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
                    message = "Không thể tìm thấy bản tin với id là " + id
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
                        message = "Xóa bản tin thành công"
                    });
                }
                return BadRequest("Xóa bản tin thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
