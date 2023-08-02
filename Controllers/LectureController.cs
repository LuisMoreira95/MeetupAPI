using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MMeetupAPI.Entities;
using MMeetupAPI.Models;

namespace MMeetupAPI.Controllers
{
    [Route("api/meetup/{meetupName}/lecture")]
    public class LectureController : ControllerBase
    {
        private readonly MeetupContext meetupContext;
        private readonly IMapper mapper;
        private readonly ILogger<LectureController> logger;

        public LectureController(MeetupContext meetupContext, IMapper mapper, ILogger<LectureController> logger)
        {
            this.meetupContext = meetupContext;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(string meetupName, int id)
        {

            var meetup = this.meetupContext.Meetups
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == meetupName);


            if (meetup == null)
            {
                return NotFound();
            }

            var lecture = meetup.Lectures.FirstOrDefault(l => l.Id == id);

            if (lecture == null)
            {
                NotFound();
            }

            this.meetupContext.Lectures.Remove(lecture);
            this.meetupContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete]
        public ActionResult Delete(string meetupName)
        {
            var meetup = this.meetupContext.Meetups
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == meetupName);


            if (meetup == null)
            {
                return NotFound();
            }

            this.logger.LogWarning($"Lectures for meetup {meetupName} were deleted");

            this.meetupContext.Lectures.RemoveRange(meetup.Lectures);
            this.meetupContext.SaveChanges();

            return NoContent();
        }

        [HttpGet]
        public ActionResult Get(string meetupName)
        {

            var meetup = this.meetupContext.Meetups
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == meetupName);


            if (meetup == null)
            {
                return NotFound();
            }

            var lectures = this.mapper.Map<List<LectureDto>>(meetup.Lectures);

            return Ok(lectures);

        }

        [HttpPost]
        public ActionResult Post(string meetupName, [FromBody] LectureDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var meetup = this.meetupContext.Meetups
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == meetupName);


            if (meetup == null)
            {
                return NotFound();
            }

            var lecture = this.mapper.Map<Lecture>(model);
            meetup.Lectures.Add(lecture);
            this.meetupContext.SaveChanges();


            return Created($"api/meetup/{meetupName}", null);
        }
    }
}
