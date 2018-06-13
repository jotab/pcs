using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using ApplicationCore.Interfaces;
using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Model;
using Infrastructure.Mapping;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;

namespace Infrastructure.Repository
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class UnitOfWork : IUnityOfWork
    {
        private readonly IConfiguration _configuration;
        private readonly IMappingCache _mappingCache;
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        #region Repository properties

        public IUserRepository UserRepository => new UserRepository(Connection, _mappingCache);
        public IRepository<Role> RoleRepository => new Repository<Role>(Connection, _mappingCache);

        #endregion

        private IDbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new OracleConnection(_configuration.GetConnectionString("DefaultConnection"));
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

        public UnitOfWork(IConfiguration configuration, IMappingCache mappingCache)
        {
            _configuration = configuration;
            _mappingCache = mappingCache;
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