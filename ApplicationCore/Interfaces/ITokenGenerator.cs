using ApplicationCore.Model;

namespace ApplicationCore.Interfaces
{
    public interface ITokenGenerator
    {
        string GenerateJwtToken(User user);
    }
}