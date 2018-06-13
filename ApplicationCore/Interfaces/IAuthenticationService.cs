namespace ApplicationCore.Interfaces
{
    public interface IAuthenticationService
    {
        bool ValidateCredentials(string user, string password);
    }
}