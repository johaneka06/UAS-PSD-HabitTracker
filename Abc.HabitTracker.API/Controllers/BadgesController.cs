using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Abc.HabitTracker.Database.Postgresql;

namespace Abc.HabitTracker.Api.Controllers
{
    [ApiController]
    public class BadgesController : ControllerBase
    {
        private readonly ILogger<BadgesController> _logger;

        public BadgesController(ILogger<BadgesController> logger)
        {
            _logger = logger;
        }

        [HttpGet("api/v1/users/{userID}/badges")]
        public ActionResult<IEnumerable<Badge>> All(Guid userID)
        {
            DotNetEnv.Env.Load();

            PostGresUnitOfWork uow = new PostGresUnitOfWork(System.Environment.GetEnvironmentVariable("CONN_STR"));

            List<Badge> badgeList = uow.BadgeRepo.GetBadges(userID);

            if(badgeList.Count == 0) return NoContent();

            ActionResult<IEnumerable<Badge>> list = new ActionResult<IEnumerable<Badge>>(badgeList);

            return list;
        }
    }
}
