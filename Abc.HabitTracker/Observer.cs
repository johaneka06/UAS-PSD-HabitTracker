using System;

namespace Abc.HabitTracker
{
    public interface IObservable<T>
    {
        void Attach(IObserver<T> obs);
        void Broadcast(T e);
    }

    public interface IObserver<T>
    {
        void Update(T e);
    }
}