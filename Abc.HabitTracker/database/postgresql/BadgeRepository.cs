using System;
using System.Collections.Generic;
using Npgsql;

using Abc.HabitTracker;

namespace Abc.HabitTracker.Database.Postgresql
{
    public class BadgeRepository : IBadgeRepository
    {
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        public BadgeRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            this._connection = connection;
            this._transaction = transaction;
        }

        public List<Badge> GetBadges(Guid userId)
        {
            string query = "SELECT badge_id, user_id, badge_name, description, created_at FROM badge WHERE user_id = @id";
            List<Badge> badgeList = new List<Badge>();

            using(var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", userId);
                using(var reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        Badge currentBadge = new Badge(reader.GetGuid(0), new BadgeName(reader.GetString(2)), reader.GetString(3), reader.GetGuid(1), reader.GetDateTime(4));
                        badgeList.Add(currentBadge);
                    }
                }
            }

            return badgeList;
        }

        public void GiveBadge(Guid userId, Badge badge)
        {
            string query = "INSERT INTO badge (badge_id, user_id, badge_name, description) VALUES (@id, @user, @name, @desc)";

            using(var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", badge.BadgeID);
                cmd.Parameters.AddWithValue("user", userId);
                cmd.Parameters.AddWithValue("name", badge.BadgeName);
                cmd.Parameters.AddWithValue("desc", badge.BadgeDesc);

                cmd.ExecuteNonQuery();
            }
        }
    }
}