using MMeetupAPI.Entities;

namespace MMeetupAPI.Identity
{
    public interface IJwtProvider
    {
        string GenerateJwtToken(User user);
    }
}
