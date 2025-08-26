using TL.Travel.DomainModels.Operators;

namespace TL.Travel.Interfaces
{
    public interface IOperatorService : IService
    {

        public IQueryable<OperatorDTO> GetAll();
        OperatorDTO AddOrUpdate(EditOperatorDTO entry, int id = 0);
        OperatorDTO GetById(int id);
        void Delete(int id);
    }
}
