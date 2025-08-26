using TL.AspNet.Security.Services;

public static class Program
{

    public static void Main(string[] args)
    {
        while (true)
        {
            Console.Clear();

            Console.Write("Email: ");
            string operatorName = Console.ReadLine();

            Console.Write("Password: ");
            string operatorPassword = Console.ReadLine();

            string hash = new PasswordHasher().HashPassword(operatorPassword, operatorName);

            Console.WriteLine(hash);

            Console.ReadLine();

        }
    }

}
