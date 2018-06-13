using System.Collections.Generic;
using System.Data;
using ApplicationCore.Interfaces.Repository;
using ApplicationCore.Model;
using Dapper;
using Infrastructure.Extensions;
using Infrastructure.Helpers;
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
            var userNameMapping = Mapping.GetPropertyByName(nameof(User.NormalizedUserName));

            var select =
                $"{Mapping.GetSelectSql()} {SqlTerm.Where} {userNameMapping.ColumnName} = :{userNameMapping.PropertyName}";
            var parameters = new DynamicParameters();
            parameters.Add($"@{userNameMapping.PropertyName}", userName);

            var user = Connection.QuerySingleOrDefault<User>(@select, parameters);
            user.Roles = new List<Role>();
            return user;
        }
    }
}