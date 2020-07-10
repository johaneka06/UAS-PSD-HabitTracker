using System;

namespace Abc.HabitTracker
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        void RollBack();
    }
}