using System;
using Xunit;

using Abc.HabitTracker;

namespace Abc.HabitTracker.UnitTest
{
    public class HabitTest
    {
        [Fact]
        public void HabitNameWrong()
        {
            Exception ex = null;
            String[] array = {"Mon", "Tue", "Wed"};
            try
            {
                Habit h = Habit.CreateHabit(Guid.NewGuid(), "A", array);
            }
            catch (Exception e)
            {
                ex = e;
            }
            Assert.True(ex != null);
        }

        [Fact]
        public void HabitNameCorrect()
        {
            String[] array = {"Mon", "Tue", "Wed"};
            Habit h = Habit.CreateHabit(Guid.NewGuid(), "main game", array);

            Assert.Equal("main game", h.HabitName);
            Assert.Contains(new string("Mon"), h.DaysOff);
        }

        [Fact]
        public void ChangeHabitName()
        {
            String[] array = {"Mon", "Tue", "Wed"};
            Habit h = Habit.CreateHabit(Guid.NewGuid(), "main game", array);
            
            Assert.Equal("main game", h.HabitName);

            h.ChangeHabitName(h.HabitId, h.UserID, "baca buku");

            Assert.Equal("baca buku", h.HabitName);
        }
    }
}
