using System.Diagnostics.CodeAnalysis;
using System.DirectoryServices.AccountManagement;
using ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LdapProvider
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class LdapAuthenticationService : IAuthenticationService
    {
        private readonly ILogger<LdapAuthenticationService> _logger;
        private readonly PrincipalContext _principalContext;

        public LdapAuthenticationService(IConfiguration configuration, ILogger<LdapAuthenticationService> logger)
        {
            _logger = logger;
            _principalContext = new PrincipalContext(ContextType.Domain, configuration["Ldap:DomainName"]);
        }

        public bool ValidateCredentials(string usr, string pwd)
        {
            using (_principalContext)
            {
                //Username and password for authentication.
                return _principalContext.ValidateCredentials(usr, pwd);
            }
        }
        
        public void GetUserGroups (string usr)
        {
            using (_principalContext)
            {
                UserPrincipal user;
                if ((user = UserPrincipal.FindByIdentity(_principalContext, usr)) == null) 
                    return;
                
                var groups = user.GetGroups();
                foreach (var principal in groups)
                {
                    var g = (GroupPrincipal) principal;
                    _logger.LogInformation(g.Name);
                }
            }
        }
    }
}