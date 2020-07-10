using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Abc.HabitTracker;
using Abc.HabitTracker.Database.Postgresql;

namespace Abc.HabitTracker.Api.Controllers
{
    [ApiController]
    public class HabitsController : ControllerBase
    {
        private readonly ILogger<HabitsController> _logger;
        private PostGresUnitOfWork uow;

        public HabitsController(ILogger<HabitsController> logger)
        {
            DotNetEnv.Env.Load();
            uow = new PostGresUnitOfWork(System.Environment.GetEnvironmentVariable("CONN_STR"));

            _logger = logger;
        }

        [HttpGet("api/v1/users/{userID}/habits")]
        public ActionResult<IEnumerable<Habit>> All(Guid userID)
        {
            List<Habit> habits = uow.HabitRepo.GetAllHabit(userID);
            return (habits != null || habits.Count != 0) ? new ActionResult<IEnumerable<Habit>>(habits) : NotFound("No habbit found for user ID: " + userID);
        }

        [HttpGet("api/v1/users/{userID}/habits/{id}")]
        public ActionResult<Habit> Get(Guid userID, Guid id)
        {
            Habit currentHabit = uow.HabitRepo.GetHabit(userID, id);
            return (currentHabit != null) ? new ActionResult<Habit>(currentHabit) : NotFound("Habit not found");
        }

        [HttpPost("api/v1/users/{userID}/habits")]
        public ActionResult<Habit> AddNewHabit(Guid userID, [FromBody] RequestData data)
        {
            string[] off = new string[0];
            if (data.DaysOff.Length != 0) off = data.DaysOff;
            Habit newHabit = Habit.CreateHabit(userID, data.Name, off);
            newHabit = uow.HabitRepo.InsertNewHabit(newHabit);
            uow.Commit();

            return newHabit;
        }

        [HttpPut("api/v1/users/{userID}/habits/{id}")]
        public ActionResult<Habit> UpdateHabit(Guid userID, Guid id, [FromBody] RequestData data)
        {
            ActionResult<Habit> updatedHabit = null;

            if (data.Name.Length < 2 || data.Name.Length > 100) return BadRequest();
            else
            {
                Habit oldHabit = uow.HabitRepo.GetHabit(userID, id);
                if(oldHabit == null) return NotFound("Habit Not Found");

                if (data.DaysOff.Length == 0 || data.DaysOff == null) uow.HabitRepo.UpdateHabit(userID, id, data.Name);
                else uow.HabitRepo.UpdateHabit(userID, id, data.Name, data.DaysOff);

                uow.Commit();
            }
            updatedHabit = Get(userID, id);

            return updatedHabit;
        }

        [HttpDelete("api/v1/users/{userID}/habits/{id}")]
        public ActionResult<Habit> DeleteHabit(Guid userID, Guid id)
        {
            Habit deletedHabit = uow.HabitRepo.DeleteHabit(userID, id);

            if (deletedHabit != null)
            {
                uow.Commit();
                return deletedHabit;
            }
            else return NotFound("Habit not found");
        }

        [HttpPost("api/v1/users/{userID}/habits/{id}/logs")]
        public ActionResult<Habit> Log(Guid userID, Guid id)
        {
            if(uow.HabitRepo.GetHabit(userID, id) == null) return NotFound("Habit not found");

            AbcTracker tracer = HabitTrackerFactory.Create(id, userID, uow.BadgeRepo, uow.HabitRepo);

            Habit newHabit, fromDB = null;
            newHabit = tracer.Log(new HabitLog(userID, id));
            try
            {
                fromDB = uow.HabitRepo.Log(userID, id, newHabit);
            }
            #pragma warning disable 168
            catch (Exception e)
            {
                uow.RollBack();
                return NotFound();
            }
            
            uow.Commit();

            return fromDB;
        }
    }
}
