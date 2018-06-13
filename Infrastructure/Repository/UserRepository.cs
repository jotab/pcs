using System.Data;
using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Model;
using Infrastructure.Mapping;

namespace Infrastructure.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IDbConnection connection, IMappingCache mappingCache)
            : base(connection, mappingCache)
        {
        }

        public User FindByName(string userName)
        {
            throw new System.NotImplementedException();
        }
    }
}