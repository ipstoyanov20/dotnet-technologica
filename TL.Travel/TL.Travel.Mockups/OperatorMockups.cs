using TL.AspNet.Security.Services.PasswordHashers;
using TL.DataAccess.Models;

namespace TL.Travel.Mockups
{
    internal static class OperatorMockups
    {
        public static readonly List<Operator> Operators = new List<Operator>
        {
            new Operator
            {
                Id = 1,
                Name = "Operator",
                Address = "ул. Софийско поле",
                Email = "operator@technologica.com",
                Password = new SHA256PasswordHasher().HashPassword("1234", "operator@technologica.com"),
                Phone = "24562347635",
                IsActive = true,
                CreatedBy = "SYSTEM",
                CreatedOn = DateTime.Now
            },
            new Operator
            {
                Id = 2,
                Name = "Manager",
                Address = "ул. Червена стена",
                Email = "manager@technologica.com",
                Password = new SHA256PasswordHasher().HashPassword("1234", "manager@technologica.com"),
                Phone = "653562452",
                IsActive = true,
                CreatedBy = "SYSTEM",
                CreatedOn = DateTime.Now
            }
        };
    }
}
