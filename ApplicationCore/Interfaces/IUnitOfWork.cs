using System;
using ApplicationCore.Model;

namespace ApplicationCore.Interfaces
{
    public interface IUnityOfWork : IDisposable
    {
        IRepository<User> UserRepository { get; }

        void Commit();
        void Rollback();
    }
}