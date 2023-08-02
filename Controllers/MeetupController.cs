using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMeetupAPI.Authorization;
using MMeetupAPI.Controllers.Filters;
using MMeetupAPI.Entities;
using MMeetupAPI.Models;
using System.Linq.Expressions;
using System.Security.Claims;

namespace MMeetupAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [ServiceFilter(typeof(TimeTrackFilter))]
    [AllowAnonymous]
    public class MeetupController : ControllerBase
    {
        private readonly MeetupContext meetupContext;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public MeetupController(MeetupContext meetupContext, IMapper mapper, IAuthorizationService authorizationService)
        {
            this.meetupContext = meetupContext;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        [HttpGet]
        //[NationalityFilter("German,Russian")]
        public ActionResult<List<MeetupDetailsDto>> GetAll([FromQuery] MeetupQuery query)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var baseQuery = this.meetupContext.Meetups
                .Include(m => m.Lectures)
                .Include(m => m.Location)
                .Where(m => query.SearchPhrase == null || (m.Organizer.ToLower().Contains(query.SearchPhrase.ToLower()) || m.Name.ToLower().Contains(query.SearchPhrase.ToLower())));

            if (!string.IsNullOrEmpty(query.SortBy))
            {
                var propertySelectors = new Dictionary<string, Expression<Func<Meetup, Object>>>()
                {
                    { nameof(Meetup.Name), meetup => meetup.Name },
                    { nameof(Meetup.Date), meetup => meetup.Date },
                    { nameof(Meetup.Organizer), meetup => meetup.Organizer },
                };

                var propertySelector = propertySelectors[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.ASC ?
                    baseQuery.OrderBy(propertySelector) :
                    baseQuery.OrderByDescending(propertySelector);
            }

            var meetups =
                baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();

            var totalCount = baseQuery.Count();

            var meeetupDtos = this.mapper.Map<List<MeetupDetailsDto>>(meetups);


            var result = new PagedResult<MeetupDetailsDto>(meeetupDtos, totalCount, query.PageNumber, query.PageSize);

            return Ok(result);
        }

        [HttpGet("{name}")]
        [NationalityFilter("German,Russian")]
        //[Authorize(Policy = "HasNationality")]
        public ActionResult<MeetupDetailsDto> Get(string name)
        {
            var meetup = this.meetupContext.Meetups
                .Include(m => m.Location)
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == name.ToLower());

            if (meetup == null)
            {
                return NotFound();
            }

            var meetupDto = this.mapper.Map<MeetupDetailsDto>(meetup);

            return Ok(meetupDto);
        }

        [HttpPost]
        //[Authorize(Roles = "Admin,Moderator")]
        public ActionResult Post([FromBody] MeetupDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var meetup = this.mapper.Map<Meetup>(model);

            var userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

            meetup.CreatedById = int.Parse(userId);

            this.meetupContext.Add(meetup);
            this.meetupContext.SaveChanges();

            var key = meetup.Name.Replace(" ", "-").ToLower();

            return Created("api/meetup/" + key, null);
        }

        [HttpPut("{name}")]
        public ActionResult Put(string name, [FromBody] MeetupDto model)
        {
            var meetup = this.meetupContext.Meetups
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == name.ToLower());

            if (meetup == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            meetup.Name = model.Name;
            meetup.Organizer = model.Organizer;
            meetup.Date = model.Date;
            meetup.IsPrivate = model.IsPrivate;

            this.meetupContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{name}")]
        public ActionResult delete(string name)
        {
            var meetup = this.meetupContext.Meetups
                .Include(m => m.Location)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == name.ToLower());

            if (meetup == null)
            {
                return NotFound();
            }

            var authorizationResult = this.authorizationService.AuthorizeAsync(User, meetup, new ResourceOperationRequirement(OperationType.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            this.meetupContext.Remove(meetup);
            this.meetupContext.SaveChanges();

            return NoContent();
        }
    }
}
