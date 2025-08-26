using TL.AspNet.Security.Services.PasswordHashers;

namespace TL.Hasher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter username:");
            string username = Console.ReadLine();


            Console.WriteLine("Enter password:");
            string password = Console.ReadLine();
            string hash = new SHA256PasswordHasher().HashPassword(password, username);
            Console.WriteLine(hash);
            Console.ReadLine();
        }
    }
}
