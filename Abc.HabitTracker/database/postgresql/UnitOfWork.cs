using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Npgsql;
using NpgsqlTypes;

namespace Abc.HabitTracker.Database.Postgresql
{
    public class PostGresUnitOfWork : IUnitOfWork
    {
        private NpgsqlConnection _connection;
        private NpgsqlTransaction _transaction;

        private IBadgeRepository _BadgeRepo;
        private IHabitRepository _habitRepo;

        public PostGresUnitOfWork(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction();
        }

        public IBadgeRepository BadgeRepo
        {
            get
            {
                if(_BadgeRepo == null) _BadgeRepo = new BadgeRepository(_connection, _transaction);
                return _BadgeRepo;
            }
        }

        public IHabitRepository HabitRepo
        {
            get
            {
                if(_habitRepo == null) _habitRepo = new HabitRepository(_connection, _transaction);
                return _habitRepo;
            }
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void RollBack()
        {
            _transaction.Rollback();
        }

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!this.disposed)
            {
                if(disposing)
                {
                    _connection.Close();
                }

                disposed = true;
            }
        }
    }
}