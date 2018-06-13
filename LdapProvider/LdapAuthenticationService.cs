using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.AccountManagement;
using ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;

namespace LdapProvider
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class LdapAuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;

        public LdapAuthenticationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool ValidateCredentials(string usr, string pwd)
        {
            using (var context = new PrincipalContext(ContextType.Domain, _configuration["Ldap:Domain"]))
            {
                //Username and password for authentication.
                return context.ValidateCredentials(usr, pwd);
            }
        }
    }
}