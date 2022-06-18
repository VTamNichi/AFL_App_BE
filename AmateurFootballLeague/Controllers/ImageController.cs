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
    [Route("api/v1/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ITournamentService _tournamentService;
        private readonly IUploadFileService _uploadFileService;
        private readonly IMapper _mapper;

        public ImageController(IImageService imageService, ITournamentService tournamentService, IUploadFileService uploadFileService, IMapper mapper)
        {
            _imageService = imageService;
            _tournamentService = tournamentService;
            _uploadFileService = uploadFileService;
            _mapper = mapper;
        }

        /// <summary>Get list image</summary>
        /// <returns>List image</returns>
        /// <response code="200">Returns list image</response>
        /// <response code="404">Not found image</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<ImageListVM> GetListImage(
            [FromQuery(Name = "tournament-id")] int? tourId,
            [FromQuery(Name = "status")] bool? status,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<Image> imageList = _imageService.GetList();
                if (!String.IsNullOrEmpty(tourId.ToString()))
                {
                    imageList = imageList.Where(s => s.TournamentId == tourId);
                }
                if (!String.IsNullOrEmpty(status.ToString()))
                {
                    imageList = imageList.Where(s => s.Status == status);
                }

                imageList = imageList.OrderBy(tnm => tnm.Id);
                if (orderType == SortTypeEnum.DESC)
                {
                    imageList = imageList.OrderByDescending(tnm => tnm.Id);
                }

                int countList = imageList.Count();

                var imageListPaging = imageList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var imageListResponse = new ImageListVM
                {
                    Images = _mapper.Map<List<Image>, List<ImageVM>>(imageListPaging),
                    CountList = countList,
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(imageListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get image by id</summary>
        /// <returns>Return the image with the corresponding id</returns>
        /// <response code="200">Returns the image with the specified id</response>
        /// <response code="404">No image found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<ImageVM>> GetImageById(int id)
        {
            try
            {
                Image currentImage = await _imageService.GetByIdAsync(id);
                if (currentImage != null)
                {
                    return Ok(_mapper.Map<ImageVM>(currentImage));
                }
                return NotFound("Không thể tìm thấy hình ảnh với id là " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new image</summary>
        /// <response code="201">Created new image successfull</response>
        /// <response code="400">Field is not imageed or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<ImageVM>> CreateImage([FromForm] ImageCM model)
        {
            Image image = new();
            try
            {
                Tournament tournament = await _tournamentService.GetByIdAsync(model.TournamentId);
                if (tournament == null)
                {
                    return NotFound("Giải đấu không tồn tại");
                }

                string fileUrl = await _uploadFileService.UploadFile(model.File!, "images", "image-url");
                image.ImageUrl = fileUrl;
                image.Status = true;
                image.DateCreate = DateTime.Now.AddHours(7);
                image.TournamentId = model.TournamentId;

                Image imageCreated = await _imageService.AddAsync(image);
                if (imageCreated != null)
                {
                    return CreatedAtAction("GetImageById", new { id = imageCreated.Id }, _mapper.Map<ImageVM>(imageCreated));
                }
                return BadRequest("Tạo hình ảnh thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a image</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not imageed</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<ImageVM>> UpdateImage([FromForm] ImageUM model)
        {
            try
            {
                Image image = await _imageService.GetByIdAsync(model.Id);
                if(image == null)
                {
                    return NotFound("Hình ảnh không tồn tại");
                }
                try
                {
                    if (!String.IsNullOrEmpty(model.File!.ToString()))
                    {
                        string fileUrl = await _uploadFileService.UploadFile(model.File, "images", "image-url");
                        image.ImageUrl = fileUrl;
                    }
                }
                catch { }
                if (!String.IsNullOrEmpty(model.TournamentId.ToString()))
                {
                    image.TournamentId = model.TournamentId;
                }
                image.DateUpdate = DateTime.Now.AddHours(7);

                bool isUpdated = await _imageService.UpdateAsync(image);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<ImageVM>(image));
                }
                return BadRequest("Cập nhật bản tin thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Change status image By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("{id}")]
        public async Task<ActionResult> ChangeStatusImage(int id)
        {
            Image currentImage = await _imageService.GetByIdAsync(id);
            if (currentImage == null)
            {
                return NotFound(new
                {
                    message = "Không thể tìm thấy hình ảnh với id là " + id
                });
            }
            try
            {
                currentImage.Status = !currentImage.Status;
                bool isDeleted = await _imageService.UpdateAsync(currentImage);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Thay đổi trạng thái hình ảnh thành công"
                    });
                }
                return BadRequest("Thay đổi trạng thái hình ảnh thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete image By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            Image currentImage = await _imageService.GetByIdAsync(id);
            if (currentImage == null)
            {
                return NotFound(new
                {
                    message = "Không thể tìm thấy hình ảnh với id là " + id
                });
            }
            try
            {
                currentImage.Status = false;
                currentImage.DateDelete = DateTime.Now.AddHours(7);

                bool isUpdated = await _imageService.UpdateAsync(currentImage);
                if (isUpdated)
                {
                    return Ok(new
                    {
                        message = "Xóa trạng thái hình ảnh thành công"
                    });
                }
                return BadRequest("Xóa trạng thái hình ảnh thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
