using System;
using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Model;

namespace ApplicationCore.Interfaces
{
    public interface IUnityOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IRepository<Role> RoleRepository { get; }

        void Commit();
        void Rollback();
    }
}