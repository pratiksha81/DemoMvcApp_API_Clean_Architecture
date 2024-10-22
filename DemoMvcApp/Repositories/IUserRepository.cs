using DemoMvcApp.Models;

namespace DemoMvcApp.Repositories
{
    public interface IUserRepository
    {
        User Authenticate(string username, string password);
        User GetById(int id);
    }

}
