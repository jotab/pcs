using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using ApplicationCore.Interfaces;
using ApplicationCore.Model;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace Infrastructure.Repository
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class UnitOfWork : IUnityOfWork
    {
        private readonly IConfiguration _configuration;
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        #region Repository fields

        private IRepository<User> _useRepository;

        #endregion

        #region Repository properties

        public IRepository<User> UserRepository =>
            _useRepository ?? (_useRepository = new Repository<User>(Connection));

        #endregion

        private IDbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    var oracleConnection =
                        new OracleConnection(_configuration.GetConnectionString("DefaultConnection"));
                    _connection = oracleConnection;
                    _connection.Open();
                    _transaction = _connection.BeginTransaction();
                }
                else if (_connection.State == ConnectionState.Closed)
                {
                    _connection.Open();
                }

                return _connection;
            }
        }

        public UnitOfWork(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Commit()
        {
            _transaction?.Commit();
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _connection?.Dispose();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}