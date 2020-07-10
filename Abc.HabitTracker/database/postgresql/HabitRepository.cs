using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Npgsql;
using NpgsqlTypes;

using Abc.HabitTracker;

namespace Abc.HabitTracker.Database.Postgresql
{
    public class HabitRepository : IHabitRepository
    {
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        public HabitRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            this._connection = connection;
            this._transaction = transaction;
        }
        public Habit InsertNewHabit(Habit newHabit)
        {
            string query = "INSERT INTO habit (habit_id, user_id, habit_name, days_off) VALUES (@id, @user, @name, @off)";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", newHabit.HabitId);
                cmd.Parameters.AddWithValue("user", newHabit.UserID);
                cmd.Parameters.AddWithValue("name", newHabit.HabitName);
                cmd.Parameters.AddWithValue("off", newHabit.DaysOff);

                cmd.ExecuteNonQuery();
            }

            return newHabit;
        }

        public Habit Log(Guid userId, Guid habitId, Habit newHabit)
        {
            if (GetHabit(userId, habitId) == null) return null;

            string query = @"INSERT INTO habit_log (log_id, habit_id, user_id, current_streak, longest_streak) VALUES
                (@log, @habit, @user, @current, @longest)";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("log", Guid.NewGuid());
                cmd.Parameters.AddWithValue("habit", habitId);
                cmd.Parameters.AddWithValue("user", userId);
                cmd.Parameters.AddWithValue("current", newHabit.CurrentStreak);
                cmd.Parameters.AddWithValue("longest", newHabit.LongestStreak);

                cmd.ExecuteNonQuery();
            }

            int count = 0;
            query = "SELECT COUNT(1) FROM habit_log WHERE habit_id = @habit AND user_id = @user";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habit", habitId);
                cmd.Parameters.AddWithValue("user", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read()) count = reader.GetInt32(0);
                }
            }

            if (count % 100 == 0) CreateSnapshot(habitId, userId);

            return GetHabit(userId, habitId);
        }

        public List<Habit> GetAllHabit(Guid userId)
        {
            List<Habit> habitList = new List<Habit>();
            List<Guid> habitIdList = new List<Guid>();

            string query = "SELECT habit_id FROM habit WHERE user_id = @user AND deleted_at IS NULL";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("user", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        habitIdList.Add(reader.GetGuid(0));
                    }
                }
            }

            foreach (Guid habitId in habitIdList)
            {
                habitList.Add(GetHabit(userId, habitId));
            }

            return habitList;
        }

        public Habit DeleteHabit(Guid userId, Guid habitId)
        {
            string query;
            Habit currentHabit = GetHabit(userId, habitId);

            if (currentHabit == null) return null;

            query = "UPDATE habit SET deleted_at = CURRENT_TIMESTAMP WHERE habit_id = @habit AND user_id = @user AND deleted_at IS NULL";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habit", habitId);
                cmd.Parameters.AddWithValue("user", userId);

                cmd.ExecuteNonQuery();
            }

            return currentHabit;
        }

        public Habit GetHabit(Guid userId, Guid habitId)
        {
            int totalCount = GetCount(habitId, userId);
            int longest = GetMax(habitId, userId);
            int current = 0;
            List<string> daysOff = new List<string>();
            string habit_name = "";
            DateTime created = new DateTime();

            string query = @"SELECT habit_name, 
                    UNNEST(CASE WHEN days_off <> '{}' THEN days_off ELSE '{null}' END), created_at
                FROM habit WHERE habit_id = @habit AND user_id = @user AND habit_name IS NOT NULL AND deleted_at IS NULL";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habit", habitId);
                cmd.Parameters.AddWithValue("user", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        habit_name = reader.GetString(0);
                        //Try get daysOff
                        try
                        {
                            daysOff.Add(reader.GetString(1));
                        }
                        #pragma warning disable 168 //Disable unused variable warning
                        catch (Exception e) { }

                        created = reader.GetDateTime(2);
                    }
                }
            }

            List<DateTime> log_date = new List<DateTime>();

            query = "SELECT log_date FROM habit_log WHERE habit_id = @habit AND user_id = @user";
            try
            {
                using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
                {
                    cmd.Parameters.AddWithValue("habit", habitId);
                    cmd.Parameters.AddWithValue("user", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            log_date.Add(reader.GetDateTime(0));
                        }

                    }
                }
            }
            catch (Exception e) { }

            Habit currentHabit = null;
            try
            {
                currentHabit = new Habit(habitId, userId, new HabitName(habit_name), new DaysOff(daysOff.ToArray()), current, longest, totalCount, log_date.ToArray(), created);
            }
            catch (Exception e)
            {
                return null;
            }

            return currentHabit;
        }

        public Habit UpdateHabit(Guid userId, Guid habitId, string name)
        {
            string query = "UPDATE habit SET habit_name = @name, updated_at = CURRENT_TIMESTAMP WHERE habit_id = @habit AND user_id = @user AND deleted_at IS NULL";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habit", habitId);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("user", userId);

                cmd.ExecuteNonQuery();
            }

            return GetHabit(userId, habitId);
        }

        public Habit UpdateHabit(Guid userId, Guid habitId, string name, string[] daysOff)
        {
            string query = @"UPDATE habit SET habit_name = @name, days_off = @daysOff, updated_at = CURRENT_TIMESTAMP WHERE habit_id = @habit AND user_id = @user AND deleted_at IS NULL";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habit", habitId);
                cmd.Parameters.AddWithValue("name", name);
                cmd.Parameters.AddWithValue("daysOff", daysOff);
                cmd.Parameters.AddWithValue("user", userId);

                cmd.ExecuteNonQuery();
            }

            return GetHabit(userId, habitId);
        }

        private int GetCount(Guid habitId, Guid userId)
        {
            int count = 0;

            string query = "SELECT log_count, last_snapshot_time FROM log_snapshot WHERE last_habit_id = @id AND user_id = @user ORDER BY current_snapshot_at DESC LIMIT 1";
            NpgsqlDateTime lastCreated = new NpgsqlDateTime(0);

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", habitId);
                cmd.Parameters.AddWithValue("user", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count = reader.GetInt32(0);
                        lastCreated = reader.GetTimeStamp(1);
                    }
                }
            }

            query = @"SELECT coalesce(COUNT(log_id), 0) FROM habit_log WHERE habit_id = @id AND user_id = @user AND log_date > @created
                GROUP BY habit_id, user_id";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", habitId);
                cmd.Parameters.AddWithValue("user", userId);
                cmd.Parameters.AddWithValue("created", lastCreated);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count += reader.GetInt32(0);
                    }
                }
            }
            return count;
        }

        private int GetMax(Guid habitId, Guid userId)
        {
            int count = 0;

            string query = @"SELECT coalesce(MAX(longest_streak), 0) FROM habit_log 
                WHERE (habit_id = @habit AND user_id = @user)";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habit", habitId);
                cmd.Parameters.AddWithValue("user", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }
                }
            }

            return count;
        }

        private int GetCurrent(Guid habitId, Guid userId)
        {
            int count = 0;
            string query = "SELECT current_streak FROM habit_log WHERE habit_id = @habitId AND user_id = @userId ORDER BY log_date DESC LIMIT 1";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("habitId", habitId);
                cmd.Parameters.AddWithValue("userId", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        count = reader.GetInt32(0);
                    }
                }
            }

            return count;
        }

        private void CreateSnapshot(Guid habitId, Guid userId)
        {
            string query =
                @"SELECT (log_id, habit_id, user_id, log_date) 
                FROM habit_log 
                WHERE habit_id = @id AND user_id = @user
                ORDER BY log_date DESC LIMIT 1";

            Guid habit_id;
            Guid user_id;
            Guid log_id;
            NpgsqlDateTime lastCreation = new NpgsqlDateTime(0);

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("id", habitId);
                cmd.Parameters.AddWithValue("user", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        log_id = reader.GetGuid(0);
                        habit_id = reader.GetGuid(1);
                        user_id = reader.GetGuid(2);
                        lastCreation = reader.GetTimeStamp(3);
                    }
                }
            }

            int count = GetCount(habitId, userId);
            int longestStreak = GetMax(habitId, userId);
            int currentStreak = GetCurrent(habitId, userId);

            query = @"INSERT INTO log_snapshot (snapshot_id, last_snapshot_id, last_habit_id, user_id, log_count, longest_streak, current_streak, last_snapshot_time) VALUES
                (@snapshotId, @lastLog, @lastHabit, @user, @logCount, @longestStreak, @currentStreak, @lastCreated)";

            using (var cmd = new NpgsqlCommand(query, _connection, _transaction))
            {
                cmd.Parameters.AddWithValue("snapshotId", Guid.NewGuid());
                cmd.Parameters.AddWithValue("lastLog", log_id);
                cmd.Parameters.AddWithValue("lastHabit", habit_id);
                cmd.Parameters.AddWithValue("user", userId);
                cmd.Parameters.AddWithValue("logCount", count);
                cmd.Parameters.AddWithValue("longestStreak", longestStreak);
                cmd.Parameters.AddWithValue("currentStreak", currentStreak);
                cmd.Parameters.AddWithValue("lastCreated", lastCreation);

                cmd.ExecuteNonQuery();
            }
        }
    }
}