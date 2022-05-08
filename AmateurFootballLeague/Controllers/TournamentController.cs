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
    [Route("api/v1/tournaments")]
    [ApiController]
    //[Authorize(Roles = "ADMIN")]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly IUserService _userService;
        private readonly IUploadFileService _uploadFileService;
        private readonly IMapper _mapper;

        public TournamentController(ITournamentService tournamentService, IUserService userService, IUploadFileService uploadFileService, IMapper mapper)
        {
            _tournamentService = tournamentService;
            _userService = userService;
            _uploadFileService = uploadFileService;
            _mapper = mapper;
        }

        /// <summary>Get list tournament</summary>
        /// <returns>List tournament</returns>
        /// <response code="200">Returns list tournament</response>
        /// <response code="404">Not found tournament</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<TournamentTypeListVM> GetListTournamentType(
            [FromQuery(Name = "tournament-name")] string? name,
            [FromQuery(Name = "order-by")] TournamentFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<Tournament> tournamentList = _tournamentService.GetList();
                if (!String.IsNullOrEmpty(name))
                {
                    tournamentList = tournamentList.Where(s => s.TournamentName.ToUpper().Contains(name.Trim().ToUpper()));
                }
                var tournamentListPaging = tournamentList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var tournamentListOrder = new List<Tournament>();
                if (orderBy == TournamentFieldEnum.Id)
                {
                    tournamentListOrder = tournamentListPaging.OrderBy(tnm => tnm.Id).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        tournamentListOrder = tournamentListPaging.OrderByDescending(tnm => tnm.Id).ToList();
                    }
                }
                if (orderBy == TournamentFieldEnum.TournamentName)
                {
                    tournamentListOrder = tournamentListPaging.OrderBy(tnm => tnm.TournamentName).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        tournamentListOrder = tournamentListPaging.OrderByDescending(tnm => tnm.TournamentName).ToList();
                    }
                }
                if (orderBy == TournamentFieldEnum.Mode)
                {
                    tournamentListOrder = tournamentListPaging.OrderBy(tnm => tnm.Mode).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        tournamentListOrder = tournamentListPaging.OrderByDescending(tnm => tnm.Mode).ToList();
                    }
                }

                var tournamentListResponse = new TournamentListVM
                {
                    Tournaments = _mapper.Map<List<Tournament>, List<TournamentVM>>(tournamentListOrder),
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(tournamentListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get tournament by id</summary>
        /// <returns>Return the tournament with the corresponding id</returns>
        /// <response code="200">Returns the tournament with the specified id</response>
        /// <response code="404">No tournament found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<TournamentVM>> GetTournamentById(int id)
        {
            try
            {
                Tournament currentTournament = await _tournamentService.GetByIdAsync(id);
                if (currentTournament != null)
                {
                    return Ok(_mapper.Map<TournamentVM>(currentTournament));
                }
                return NotFound("Can not found tournament by id: " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new tournament</summary>
        /// <response code="201">Created new tournament successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<TournamentVM>> CreateTournament([FromForm] TournamentCM model)
        {
            Tournament tournament = new Tournament();
            try
            {

                User user = await _userService.GetByIdAsync(model.UserId);
                if(user.RoleId != 2)
                {
                    return BadRequest(new
                    {
                        message = "User is not host role"
                    });
                }

                bool isDuplicated = _tournamentService.GetList().Where(s => s.TournamentName.Trim().ToUpper().Equals(model.TournamentName.Trim().ToUpper())).FirstOrDefault() != null;
                if (isDuplicated)
                {
                    return BadRequest(new
                    {
                        message = "Tournament Name is duplicated"
                    });
                }
                tournament.TournamentName = model.TournamentName;
                tournament.Mode = "PUBLIC";
                if (model.Mode == TournamentModeEnum.PRIVATE)
                {
                    tournament.Mode = "PRIVATE";
                }
                if (!String.IsNullOrEmpty(model.TournamentAvatar.ToString()))
                {
                    string fileUrl = await _uploadFileService.UploadFile(model.TournamentAvatar, "service", "service-detail");
                    tournament.TournamentAvatar = fileUrl;
                }
                else
                {
                    tournament.TournamentAvatar = "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAkGBxETERMQEhIRExMVFhUYFhUYFRUPFRUYFRUWFhcXFRcYHSggGBolGxUVITEiJSkrLi4uFx8zODMtNygtLisBCgoKBQUFDgUFDisZExkrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrKysrK//AABEIAOMA3gMBIgACEQEDEQH/xAAbAAEAAgMBAQAAAAAAAAAAAAAABQYCBAcDAf/EAEUQAAIBAQQEDAMFBQYHAAAAAAABAgMEBQYREiExURMiMkFhcYGRobHB0QdSciNCYoLhM0OSorIUJHPC8PE0NVNUY9Li/8QAFAEBAAAAAAAAAAAAAAAAAAAAAP/EABQRAQAAAAAAAAAAAAAAAAAAAAD/2gAMAwEAAhEDEQA/AO0gAAAAAAAAGtabfSp8upCPXJIdZBAWnGFkjsnKb/DF+byIu0Y9j9yi39UtHyTAuYOdV8cWl8mNKHY5ebNZ37eFTkyqflhl5Idpx8cktrRzL+yXlU5rU+tyivF5GUcLW+W2nL81SP8A7AdLT3H05k8NXhHZTn+Wa9JHzg7yp/8AdLtnJeqA6cDmUcSW6nypS/PBeqNyhjquuVTpy74PzA6CCoWfHdN8ulOPU1P2JWy4qsc/3ui90k4eOwCaB5ULRCazhOMl0NPyPUAAAAAAABAAAAPkpJLNtJb3qI7ENunRs86sEnKOWWeta2l6nPoztttk0nOpltS4sI57+ZAXm34pstLVwmnLdDjeOwrttx3N6qVKMVvk9J9y1Hvd2AXqdeql+GCz/mfsWWwYaslLXGlFv5pcd+OwCgO2XhaXlF1pLdFOEfDJd5t2XBFrnrm4Q+qWk/A6VGKWpJJdxhVtEI8qUY9bSAqFl+H9NftK05dEYqC8cyVs2ELHH93p/VJs2q9/UI/ecupZmlVxQvu032vLwQEtQuuhDkUaUeqEU+/I2lFLYkVOriSs9ihHszNWpfVof7xrqSQF3GZQZ26q9tSb/MzydaT2yl3sDoeYOeKvNbJS72e0LwrLZUn/ABMC9ygntSfWszStFy2afKoUn06CT70VmnfloX38+tJm3SxNVXKjB98QNi04LsctkZQ+mT9cyItXw+X7uv2Tj6p+hN0cTwfKhJdTUvY36F80JffS+ri+YHPrRhK20tcFpZc8JZPu1M86d+2+zvKbqauapFvxevxOpQmnrTTXQ8z5VpRkspRjJbmk14gUew47WytSy/FB5+D9yxWC/bNW5FWOfyviy7ntPG34QslTWocG98Hor+HYVi8cC14calONRbuRL2YF/By+yX1bLNPg5SlqaTpz15d+tdh06DzSb3IdIIBAAABD4uWdjrdS/qiQfwyeu0Lop/5yexX/AMHX+n1RX/hny6/0w85AXe02qFNZzko+fYiGtWJorVTi30vUu418XLj03+F+ZAASFpvqvP7+it0eL+poSk3rbb69Z8AAAAAAAAAAAAAAAAAGdKtKLzjJx6nkWfD1otFTXN501zta2+hkPc11utLN6oLa9/Qi5U6ailGKyS2IdIGFaqoxcpbEeNgtDnFye95dQHNMXPO8Kn1U1/JA6acxxP8A8wqf4kPKJ04AEAgAAAiMWP8Audb6V/UiB+GS41ofRT85E1jKWVjq9Oiv5kRPwyjqtD/w13aXuBJYu5VPqfmV8n8XPj0+p+ZAAAAAAAAAAAAAAAAAADeuq7ZVpZbIrlP0XSed22GVaejHZzvmSLtZLNGnFQiskvHpYGVCjGEVCKyS2GVSooptvJI+ykks3qSK/eNtdR5LVFbFv6WBhbrY6j3RWxepLXL+y7WQBPXL+z7WBznFeq8Kn103/LBnTjmmNo5W6b38G/5Yr0Ok03mk96XkBkEAgAAAr2O55WSS3yivHM1vhrD7GrLfNeEf1HxDqZWeEd9Tyi2bXw8p5WRv5py8MkBhi1/aQX4fUgyXxTLOvluivVkQAAAAAAAAAAAAAADYsNklVmoRXW+ZLezCy2eVSShFZt+HSy63ZYI0YaK1t8p737AZ2CxxpQUI9r5297NhvnPpCXpb9LiRfF53v/QDC87dpvRjyV4mgAAJy439m/q9EQZM3C+LJdK8gKN8QYZWvPfCPhmi/wBgnnSpvfCPkilfEqnlWpS3wa7pfqWzD1TSstB/+OPgsgJAIBAAABSfiRV/YQ+t/wBK9yxYMpaNio9Kcu+TZTviBV0rTGPywS722dBumjoUKUPlpwXdFAVTEE87RU6Ml3RRHmxeFTSq1Hvk/M1wAAAAAAAAAAAGdGlKclGKzb2I+U4NtRSzb1JFxuW61Rjm9dR7Xu6EB6XRdsaMd83yn6LoN8EXelvyzhF6+d7ugDzvW37acX1v0RFAAAAAJW4Za5roXhn7kUb9yyyqZb0wIf4mUuLQnuco96T9CSwTV0rHT/C5ruk36mPxBo6Vk0vknF+cfU0vh3WzoVIfLPP+JL2AtgQCAAGFeooxlJ7IpvuWYHNL2fDXjKK56sYLqTUfRnUa0lGEnzRi33I5lg6k6tujN68nOo+vXl4s6FftXRoT6Vl36gKTJ56z4AAAAAAAAAAPsVm8lrbPhargujQyq1FxnyV8vT1gelxXTwa05r7R/wAq3dZMA0byt2gtFcp+HSBhedv0eJHlc73fqQbDeetgAAAAAAGxd88qsH05d+o1z7CWTT3MCZxLZ9OyV48+g2uuPG9Cm/DqvlVq0/mgn/C//o6E0pRy5pLzRy7DcuBt8YP5pU325r2A6aEAgBDYutPB2Sq+eSUF+Z5PwzJkpfxFteqlRXPnN9mpeoD4aWXXWq/TBeLf+UnMW1soQhvbfd/ufcEWTg7HB8885vt2eCRG4mr6VZx5opLt2sCJAAAAAAAAAJ/D9z6WVWouL92O/pfQB64fufLKtUXTGL82WIGtbrWqcc9rexAY3hbVTW+T2L1ZXpzbbbebZ9q1HJuTebZiAAAAAAAAAAAFku2pnSj0LLuObYupOjb5TWrNwqLwb8Uy+3FU1Sju195W/iXZP2Nbrg/NeoFvo1FKMZLZJJrtWZmiEwda+EslPfHOD/K9XhkTaAHMcQ1XaLc4R18ZU49jyfjmdOZy22Up2O2aW3Rnpxb+9Ft+OWaA6rRpqEFFLVGKSXRFZehQbTUcpyk9rbfiXuw2uNWnGrB5xkk17dZWcSXfoT4SK4s3r6JfqBDAAAAAABK3HdLqvSlqpr+Z7kB63DdHCPhJriLYvmfsWxI+RikkksktiPO011COk/8AcDG12lU45vsW8rtes5ycntPtqtDnLSfYtyPIAAAAAAAAAAAAAA3LpqZVV05o98YWPhbJUXPFaa/Lrfhme1zWXJcI9r2dW88sVXsrPZ5PVpzzjBbc21rfUkBV/h1bMpVKL50prs1PzRekc9wBd8pVnX1qNNNdcpLZ3eh0JACCxdc39opaUV9pDNx6VzxJ0AUDBF+8DP8As9R5Qm9Tf3JbMuhM6DaqEakHCWtNf6aKFja4NFu00lxX+0SWx/N1byUwViPhYqz1X9pFcWTfLS5vqXiBoW+xypTcJdj3reaxeb0u+NaGi9UlyXufsUq0UJQk4SWTQHmAbt1XdKtPJaorlS3fqB6XPdjrSzeagtr39CLnSpqKUYrJLYjGz0IwioRWSRlUmoptvJId5WqqKcpPJIrlstTqSzezmW4zt9sdR7orYvVmqAAAAAAAAAAAAAADbu6yactfJW32PKyWZ1JZLte4sdCkoRUVqS/1mwFWpGEXKTUYxWbexJI5be9uqW61JQTyb0acdy52/NkjjXEXCy/s9J/ZxfGa++9y6F4k1g64eAhwtRfazWz5I7ut84EzdVgjQpRpR2La97e1m2gEAAAGM4JpprNNZNb0znGJ7ilZqirUs+Dbzi1tpvbk/RnSTzr0Yzi4SSlGSyafOgIbCeJVaIqnUaVZLqU1vXT0Ere12RrR3TWyXo+g55iG4alkmqtNy4PPOMltg9z9y14VxTGulSqtRrcz2KfVufQBH2e6qkqvBNZNcp8yW8uFjssacFCK1Lvb3s9w2B8lJJZvUkV+8ba5vJclbOnpZnedu03ox5K8f0NAAAAAAAAAAAAAAAHtZbNKbyXa+ZHrYrDKo89kd/sT1GjGEcorJf61sD5ZrPGEdFdr3lMxnijlWag96qTXjGPqzHFuLeVQs76J1F4qHua2EsMaWVorri6nCD+90y6PMD2wZhzZaay6acX/AFP0LsEAAQCAAAAAAMKtNSTjJJprJp60zn+JcLyot1qObp7Wtsoe66ToYApuGMZbKNpfRGp6T9y7SSlHmcWu9MpuI8IKedWzpRltcNkZfTuZB3JiKvZJcFUTlBPJwlqcfp3eQF1tt1uOuGuO7nXuRxP3Ve1G0R0qUk98dko9aMrXd8J6+TLevUCvA2LTY5w2rVvWtGuAAAAAAADcst3Tnr5Md79EBqRTepErYrq+9U/h9zfstjhDYte97TQvzENGzLKT0qnNBbe3cgJKvWhTg5SahCK1t6kjnmJsWTr50aGcab1N7JT9l0GhbbfarfVUUm90I6ox6Xn5suGHcMU7PlOeU6u/mj9PuBGYXwnllWtC17Y093TL2LmAAAAAIBAAAAAAAAACMvm46NoXHWUuaa1SXuiTAHMLwue1WOfCQctFPVUhmv4lzduosNxY5i8oWlaL/wCotj+pc3YW2UU1k1mnzbSsXzg2lUzlRapT3bYPs5gLXRrQnHSjKM4vnTUkzUtN1QlrjxX4dxzX++WGf34L+KnL0fmWi6MdU5ZRrx4N/MuNHtW1Ab1osFSHNmt61mqWizWmFSOlTnGcXzxakjCvYqctsVnvWpgVpI3bPdlSW3irp9ibo2aEOTFLp5+88rdeNGitKrUjBdL1vqW1gfLNd0Ia8tJ737GVut1KjHTqzjBdL29S5ynXvjzbGzw/PL0j7kFZbrtlsnpy0mn+8nmor6d/YBLX5jec84WdOEdmm+U/pX3TRubC9e0PhKrlCD1uUtc5dWfmy1XLhahQylJcJU+aS1J/hjzE8BqXbd1KhDQpxUVzvnfS3zm2AAAAAAAAgEAAAAAAAAAAAAAAYVaUZJxklJPams0Vm9cFUZ5yovgpbtcoe6LSAOX17tttjlpx04pffg3KPb+qJW7ceVY6q0FU/FHKEu1bC9kTeGHLNW1yppS+aPEfhqYFTvLG9oqcWklST3ceb7Xs7DVseHLXaHpzzin9+o3n2LaX277ms9H9nTin8z40u9m+BX7qwjZ6WUpLhZ75bF1R2d5PpZakfQAAAAAAAAAAAAIBAfQAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAH/9k=";
                }
                tournament.RegisterEndDate = model.RegisterEndDate;
                tournament.TournamentStartDate = model.TournamentStartDate;
                tournament.TournamentEndDate = model.TournamentEndDate;
                tournament.FootballFieldAddress = model.FootballFieldAddress;
                tournament.Description = String.IsNullOrEmpty(model.Description) ? "" : model.Description.Trim();
                tournament.MatchMinutes = model.MatchMinutes;
                tournament.FootballTeamNumber = model.FootballTeamNumber;
                tournament.FootballPlayerMaxNumber = model.FootballPlayerMaxNumber;
                tournament.UserId = model.UserId;
                tournament.TournamentTypeId = model.TournamentTypeEnum == TournamentTypeEnum.KnockoutStage ? 1 : model.TournamentTypeEnum == TournamentTypeEnum.CircleStage ? 2 : 3;
                tournament.FootballFieldTypeId = model.TournamentFootballFieldTypeEnum == TournamentFootballFieldTypeEnum.Field5 ? 1 : model.TournamentFootballFieldTypeEnum == TournamentFootballFieldTypeEnum.Field7 ? 2 : model.TournamentFootballFieldTypeEnum == TournamentFootballFieldTypeEnum.Field9 ? 3 : 4;
                tournament.DateCreate = DateTime.Now;
                tournament.Status = true;
                tournament.StatusTnm = "";

                Tournament tournamentCreated = await _tournamentService.AddAsync(tournament);
                if (tournamentCreated != null)
                {
                    return CreatedAtAction("GetTournamentById", new { id = tournamentCreated.Id }, _mapper.Map<TournamentVM>(tournamentCreated));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a tournament</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<TournamentVM>> UpdateTournament([FromForm] TournamentUM model)
        {
            Tournament currentTournament = await _tournamentService.GetByIdAsync(model.Id);
            if (currentTournament == null)
            {
                return NotFound("Can not found tournament");
            }
            if (!String.IsNullOrEmpty(model.TournamentName))
            {
                if (!currentTournament.TournamentName.ToUpper().Equals(model.TournamentName.ToUpper()) && _tournamentService.GetList().Where(s => s.TournamentName.Trim().ToUpper().Equals(model.TournamentName.Trim().ToUpper())).FirstOrDefault() != null)
                {
                    return BadRequest(new
                    {
                        message = "Tournament Name is duplicated!"
                    });
                }
            }
            try
            {
                if (!String.IsNullOrEmpty(model.TournamentAvatar.ToString()))
                {
                    string fileUrl = await _uploadFileService.UploadFile(model.TournamentAvatar, "service", "service-detail");
                    currentTournament.TournamentAvatar = fileUrl;
                }
                currentTournament.TournamentName = String.IsNullOrEmpty(model.TournamentName) ? currentTournament.TournamentName : model.TournamentName.Trim();
                currentTournament.Mode = model.Mode == TournamentModeEnum.PUBLIC ? "PUBLIC" : model.Mode == TournamentModeEnum.PRIVATE ? "PRIVATE" : currentTournament.Mode;
                currentTournament.RegisterEndDate = String.IsNullOrEmpty(model.RegisterEndDate.ToString()) ? currentTournament.RegisterEndDate : model.RegisterEndDate;
                currentTournament.TournamentStartDate = String.IsNullOrEmpty(model.TournamentStartDate.ToString()) ? currentTournament.TournamentStartDate : model.TournamentStartDate;
                currentTournament.TournamentEndDate = String.IsNullOrEmpty(model.TournamentEndDate.ToString()) ? currentTournament.TournamentEndDate : model.TournamentEndDate;
                currentTournament.FootballFieldAddress = String.IsNullOrEmpty(model.FootballFieldAddress) ? currentTournament.FootballFieldAddress : model.FootballFieldAddress.Trim();
                currentTournament.Description = String.IsNullOrEmpty(model.Description) ? currentTournament.Description : model.Description.Trim();
                currentTournament.MatchMinutes = String.IsNullOrEmpty(model.MatchMinutes.ToString()) ? currentTournament.MatchMinutes : model.MatchMinutes;
                currentTournament.FootballTeamNumber = String.IsNullOrEmpty(model.FootballTeamNumber.ToString()) ? currentTournament.FootballTeamNumber : model.FootballTeamNumber;
                currentTournament.FootballPlayerMaxNumber = String.IsNullOrEmpty(model.FootballPlayerMaxNumber.ToString()) ? currentTournament.FootballPlayerMaxNumber : model.FootballPlayerMaxNumber;
                currentTournament.TournamentTypeId = model.TournamentTypeEnum == TournamentTypeEnum.KnockoutStage ? 1 : model.TournamentTypeEnum == TournamentTypeEnum.CircleStage ? 2 : model.TournamentTypeEnum == TournamentTypeEnum.GroupStage ? 3 : currentTournament.TournamentTypeId;
                currentTournament.FootballFieldTypeId = model.TournamentFootballFieldTypeEnum == TournamentFootballFieldTypeEnum.Field5 ? 1 : model.TournamentFootballFieldTypeEnum == TournamentFootballFieldTypeEnum.Field7 ? 2 : model.TournamentFootballFieldTypeEnum == TournamentFootballFieldTypeEnum.Field9 ? 3 : model.TournamentFootballFieldTypeEnum == TournamentFootballFieldTypeEnum.Field11 ? 4 : currentTournament.FootballFieldTypeId;
                currentTournament.DateUpdate = DateTime.Now;

                bool isUpdated = await _tournamentService.UpdateAsync(currentTournament);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<TournamentVM>(currentTournament));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Change status tournament By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("{id}")]
        public async Task<ActionResult> ChangeStatusTournament(int id)
        {
            Tournament currentTournament = await _tournamentService.GetByIdAsync(id);
            if (currentTournament == null)
            {
                return NotFound(new
                {
                    message = "Can not found tournament by id: " + id
                });
            }
            try
            {
                currentTournament.Status = !currentTournament.Status;
                bool isDeleted = await _tournamentService.UpdateAsync(currentTournament);
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

        /// <summary>Delete tournament By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            Tournament currentTournament = await _tournamentService.GetByIdAsync(id);
            if (currentTournament == null)
            {
                return NotFound(new
                {
                    message = "Can not found tournament by id: " + id
                });
            }
            try
            {
                currentTournament.Status = false;
                currentTournament.DateDelete = DateTime.Now;
                bool isDeleted = await _tournamentService.UpdateAsync(currentTournament);
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
    }
}
