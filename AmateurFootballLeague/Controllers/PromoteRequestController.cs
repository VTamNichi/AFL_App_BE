using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PromoteRequestController : ControllerBase
    {
        private readonly IPromoteRequestService _promoteRequestService;
        private readonly IMapper _mapper;

        public PromoteRequestController(IPromoteRequestService promoteRequestService, IMapper mapper)
        {
            _promoteRequestService = promoteRequestService;
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
            [FromQuery(Name = "order-by")] PromoteRequestFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<PromoteRequest> promoteRequestList = _promoteRequestService.GetList();
                if (!String.IsNullOrEmpty(content))
                {
                    promoteRequestList = promoteRequestList.Where(s => s.RequestContent.ToUpper().Contains(content.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(identityCard))
                {
                    promoteRequestList = promoteRequestList.Where(s => s.IdentityCard.ToUpper().Contains(identityCard.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(phoneBusiness))
                {
                    promoteRequestList = promoteRequestList.Where(s => s.PhoneBusiness.ToUpper().Contains(phoneBusiness.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(nameBusiness))
                {
                    promoteRequestList = promoteRequestList.Where(s => s.NameBusiness.ToUpper().Contains(nameBusiness.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(tinBusiness))
                {
                    promoteRequestList = promoteRequestList.Where(s => s.Tinbusiness.ToUpper().Contains(tinBusiness.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(status))
                {
                    promoteRequestList = promoteRequestList.Where(s => s.Status == status);
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
    }
}
