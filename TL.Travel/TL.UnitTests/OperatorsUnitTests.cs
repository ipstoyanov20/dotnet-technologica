using Microsoft.Extensions.DependencyInjection;
using Moq;
using TL.AspNet.Security.Abstractions.Services;
using TL.Travel.DataAccess.Base;
using TL.Travel.Infrastructure;
using TL.Travel.Interfaces;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, DisableTestParallelization = true)]

namespace TL.UnitTests
{
    public class OperatorsUnitTests : BaseUnitTests
    {
        private IPasswordHasher passwordHasher;
        private IOperatorService operatorService;
        private Mock<IPasswordHasher> mock;

        public OperatorsUnitTests()
            : base()
        {
            BaseTLTravelDbContext db = serviceProvider.GetRequiredService<BaseTLTravelDbContext>();

            mock = new Mock<IPasswordHasher>();
            mock.Setup(x => x.HashPassword(It.IsAny<string>(), It.IsAny<string>())).Returns("1234");
            mock.Setup(x => x.HashPassword("123", "123")).Returns("321");

            this.passwordHasher = mock.Object;
            this.operatorService = new OperatorService(db, mock.Object);
        }

        [Fact]
        public void TestHasAnyRecords()
        {
            Assert.NotEmpty(operatorService.GetAll());
        }

        [Fact]
        public void TestCanAddRecord()
        {
            Assert.Equal(2, operatorService.GetAll().Count);

            int id = operatorService.AddOrUpdate(new Travel.DomainModels.Operators.EditOperatorDTO
            {
                // Id = Random.Shared.Next(),
                Email = "iivanov@technologica.com",
                Name = "Ivelin",
                Password = "1234",
                Phone = "24624642524",
                Address = "Sofia"
            });

            mock.Verify(x => x.HashPassword(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeast(5));
            Assert.NotEqual(0, id);
            Assert.Equal(3, operatorService.GetAll().Count);
        }

        [Fact]
        public void TestUpdateOperator()
        {
            Assert.Equal(2, operatorService.GetAll().Count);

            int id = operatorService.AddOrUpdate(new Travel.DomainModels.Operators.EditOperatorDTO
            {
                Id = 2,
                Email = "iivanov@technologica.com",
                Name = "Ivelin",
                Password = "1234",
                Phone = "24624642524",
                Address = "Sofia"
            });

            Assert.NotEqual(0, id);
        }


        [Fact]
        public void TestPasswordHasher()
        {
            string hashedPassword = passwordHasher.HashPassword("123", "123");
            Assert.Equal("321", hashedPassword);



            hashedPassword = passwordHasher.HashPassword("any string", "any string");
            Assert.Equal("1234", hashedPassword);
        }
    }
}
