using System;
using System.Collections.Generic;
using Xunit;
using Npgsql;

using Abc.HabitTracker;
using Abc.HabitTracker.Database.Postgresql;

namespace Abc.HabitTracker.UnitTest
{
    public class BadgeRepositoryTest
    {
        private string conenctioStr;

        public BadgeRepositoryTest()
        {
            conenctioStr = "Host=localhost;Username=postgres;Password=postgres;Database=HabitTracker;Port=5432";
        }

        [Fact]
        public void InsertNewBadgeTest()
        {
            NpgsqlConnection _connection = new NpgsqlConnection(conenctioStr);
            _connection.Open();

            IBadgeRepository repo = new BadgeRepository(_connection, null);
            Guid user = Guid.NewGuid();

            Badge b = Badge.CreateBadge("Dominating", "4+ streak", user, DateTime.Now);

            repo.GiveBadge(user, b);

            List<Badge> b2 = repo.GetBadges(user);
            Assert.True(b2.Count == 1);

            Assert.Equal(b2[0].BadgeName, "Dominating");

            _connection.Close();
        }
    }
}