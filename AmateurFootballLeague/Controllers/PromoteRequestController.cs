using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PromoteRequestController : ControllerBase
    {
        private readonly IPromoteRequestService _promoteRequestService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public PromoteRequestController(IPromoteRequestService promoteRequestService, IUserService userService, IMapper mapper)
        {
            _promoteRequestService = promoteRequestService;
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>Get list promote requests</summary>
        /// <returns>List promoteRequests</returns>
        /// <response code="200">Returns list promoteRequests</response>
        /// <response code="404">Not found promoteRequests</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<PromoteRequestListVM> GetListPromoteRequest(
            [FromQuery(Name = "content")] string? content,
            [FromQuery(Name = "identity-card")] string? identityCard,
            [FromQuery(Name = "phone-business")] string? phoneBusiness,
            [FromQuery(Name = "name-business")] string? nameBusiness,
            [FromQuery(Name = "tin-business")] string? tinBusiness,
            [FromQuery(Name = "status")] string? status,
            [FromQuery(Name = "user-id")] int? userId,
            [FromQuery(Name = "order-by")] PromoteRequestFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<PromoteRequest> promoteRequestList = _promoteRequestService.GetList().
                    Join(_userService.GetList(), pr => pr.User, u => u, (pr, u) => new PromoteRequest
                    {
                        Id = pr.Id,
                        RequestContent = pr.RequestContent,
                        IdentityCard = pr.IdentityCard,
                        DateIssuance = pr.DateIssuance,
                        PhoneBusiness = pr.PhoneBusiness,
                        NameBusiness = pr.NameBusiness,
                        Tinbusiness = pr.Tinbusiness,
                        Status = pr.Status,
                        Reason = pr.Reason,
                        DateCreate = pr.DateCreate,
                        UserId = pr.UserId,
                        User = u
                    });
                if (!String.IsNullOrEmpty(content))
                {
                    promoteRequestList = promoteRequestList.Where(s => s.RequestContent!.ToUpper().Contains(content.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(identityCard))
                {
                    promoteRequestList = promoteRequestList.Where(s => s.IdentityCard!.ToUpper().Contains(identityCard.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(phoneBusiness))
                {
                    promoteRequestList = promoteRequestList.Where(s => s.PhoneBusiness!.ToUpper().Contains(phoneBusiness.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(nameBusiness))
                {
                    promoteRequestList = promoteRequestList.Where(s => s.NameBusiness!.ToUpper().Contains(nameBusiness.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(tinBusiness))
                {
                    promoteRequestList = promoteRequestList.Where(s => s.Tinbusiness!.ToUpper().Contains(tinBusiness.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(status))
                {
                    promoteRequestList = promoteRequestList.Where(s => s.Status == status);
                }
                if (!String.IsNullOrEmpty(userId.ToString()))
                {
                    promoteRequestList = promoteRequestList.Where(s => s.UserId == userId);
                }

                int countList = promoteRequestList.Count();

                if(orderType == SortTypeEnum.ASC)
                {
                    if(orderBy == PromoteRequestFieldEnum.Id)
                    {
                        promoteRequestList = promoteRequestList.OrderBy(pr => pr.Id);
                    }
                    else if(orderBy == PromoteRequestFieldEnum.IdentityCard)
                    {
                        promoteRequestList = promoteRequestList.OrderBy(pr => pr.IdentityCard);
                    }
                    else if (orderBy == PromoteRequestFieldEnum.PhoneBusiness)
                    {
                        promoteRequestList = promoteRequestList.OrderBy(pr => pr.PhoneBusiness);
                    }
                    else if (orderBy == PromoteRequestFieldEnum.NameBusiness)
                    {
                        promoteRequestList = promoteRequestList.OrderBy(pr => pr.NameBusiness);
                    }
                    else if (orderBy == PromoteRequestFieldEnum.Tinbusiness)
                    {
                        promoteRequestList = promoteRequestList.OrderBy(pr => pr.Tinbusiness);
                    }
                    else if (orderBy == PromoteRequestFieldEnum.Status)
                    {
                        promoteRequestList = promoteRequestList.OrderBy(pr => pr.Status);
                    }
                }
                else
                {
                    if (orderBy == PromoteRequestFieldEnum.Id)
                    {
                        promoteRequestList = promoteRequestList.OrderByDescending(pr => pr.Id);
                    }
                    else if (orderBy == PromoteRequestFieldEnum.IdentityCard)
                    {
                        promoteRequestList = promoteRequestList.OrderByDescending(pr => pr.IdentityCard);
                    }
                    else if (orderBy == PromoteRequestFieldEnum.PhoneBusiness)
                    {
                        promoteRequestList = promoteRequestList.OrderByDescending(pr => pr.PhoneBusiness);
                    }
                    else if (orderBy == PromoteRequestFieldEnum.NameBusiness)
                    {
                        promoteRequestList = promoteRequestList.OrderByDescending(pr => pr.NameBusiness);
                    }
                    else if (orderBy == PromoteRequestFieldEnum.Tinbusiness)
                    {
                        promoteRequestList = promoteRequestList.OrderByDescending(pr => pr.Tinbusiness);
                    }
                    else if (orderBy == PromoteRequestFieldEnum.Status)
                    {
                        promoteRequestList = promoteRequestList.OrderByDescending(pr => pr.Status);
                    }
                }
                
                var promoteRequestListPaging = promoteRequestList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var promoteRequestListResponse = new PromoteRequestListVM
                {
                    PromoteRequests = _mapper.Map<List<PromoteRequestVM>>(promoteRequestListPaging),
                    CountList = countList,
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(promoteRequestListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get promote request by id</summary>
        /// <returns>Return the promote request with the corresponding id</returns>
        /// <response code="200">Returns the promote request type with the specified id</response>
        /// <response code="404">No promote request found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<PromoteRequestVM>> GetPromoteRequestById(int id)
        {
            try
            {
                PromoteRequest currentPR = await _promoteRequestService.GetByIdAsync(id);
                if (currentPR != null)
                {
                    User user = await _userService.GetByIdAsync(currentPR.UserId!.Value);
                    currentPR.User = user;
                    return Ok(_mapper.Map<PromoteRequestVM>(currentPR));
                }
                return NotFound("Không thể tìm thấy yêu cầu nâng cấp với id là " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new promote request</summary>
        /// <response code="201">Created new promote request successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<PromoteRequestVM>> CreatePromoteRequest([FromBody] PromoteRequestCM model)
        {
            try
            {
                User user = await _userService.GetByIdAsync(model.UserId);
                if(user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }
                PromoteRequest promoteRequest = _mapper.Map<PromoteRequest>(model);
                promoteRequest.DateCreate = DateTime.Now.AddHours(7);
                promoteRequest.Status = "Chưa duyệt";
                promoteRequest.Reason = "";

                PromoteRequest promoteRequestCreated = await _promoteRequestService.AddAsync(promoteRequest);
                if (promoteRequestCreated != null)
                {
                    return CreatedAtAction("GetPromoteRequestById", new { id = promoteRequestCreated.Id }, _mapper.Map<PromoteRequestVM>(promoteRequestCreated));
                }
                return BadRequest("Tạo vai trò mới thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a promoteRequest</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<PromoteRequestVM>> UpdatePromoteRequest([FromBody] PromoteRequestUM model)
        {
            try
            {
                PromoteRequest currentPromoteRequest = await _promoteRequestService.GetByIdAsync(model.Id);
                if (currentPromoteRequest == null)
                {
                    return NotFound("Không tìm thấy yêu cầu nâng cấp tài khoản");
                }
                currentPromoteRequest.RequestContent = String.IsNullOrEmpty(model.RequestContent) ? currentPromoteRequest.RequestContent : model.RequestContent;
                currentPromoteRequest.IdentityCard = String.IsNullOrEmpty(model.IdentityCard) ? currentPromoteRequest.IdentityCard : model.IdentityCard;
                currentPromoteRequest.DateIssuance = String.IsNullOrEmpty(model.DateIssuance.ToString()) ? currentPromoteRequest.DateIssuance : model.DateIssuance;
                currentPromoteRequest.PhoneBusiness = String.IsNullOrEmpty(model.PhoneBusiness) ? currentPromoteRequest.PhoneBusiness : model.PhoneBusiness;
                currentPromoteRequest.NameBusiness = String.IsNullOrEmpty(model.NameBusiness) ? currentPromoteRequest.NameBusiness : model.NameBusiness;
                currentPromoteRequest.Tinbusiness = String.IsNullOrEmpty(model.Tinbusiness) ? currentPromoteRequest.Tinbusiness : model.Tinbusiness;
                currentPromoteRequest.Status = String.IsNullOrEmpty(model.Status) ? currentPromoteRequest.Status : model.Status;
                currentPromoteRequest.Reason = String.IsNullOrEmpty(model.Reason) ? currentPromoteRequest.Reason : model.Reason;

                bool isUpdated = await _promoteRequestService.UpdateAsync(currentPromoteRequest);
                if (isUpdated)
                {
                    User user = await _userService.GetByIdAsync(currentPromoteRequest.UserId!.Value);
                    currentPromoteRequest.User = user;
                    return Ok(_mapper.Map<PromoteRequestVM>(currentPromoteRequest));
                }
                return BadRequest("Cập nhật vai trò thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}