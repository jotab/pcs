using ApplicationCore.Model;

namespace ApplicationCore.Interfaces.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        User FindByName(string userName);
    }
}