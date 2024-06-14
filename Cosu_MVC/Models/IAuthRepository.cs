namespace Cosu_MVC.Models
{
    public interface IAuthRepository
    {
        Task<string> GetTokenAsync(string email, string password);
    }
}
