using TL.Travel.DataAccess.Base;
using TL.Travel.Interfaces;

namespace TL.Travel.Infrastructure
{
    public abstract class BaseService : IService
    {
        protected BaseTLTravelDbContext Db;

        protected BaseService(BaseTLTravelDbContext dbContext)
        {
            this.Db = dbContext;
        }

        public void Dispose()
        {
            this.Db.Dispose();
        }

        public void SoftDeleteRowInDatabase<T>(object entity) where T : class
        {

            if (entity == null)
            {
                return;
            }
            var entityType = typeof(T);
            var isActiveProperty = entityType.GetProperty("IsActive");
            if (isActiveProperty != null && isActiveProperty.CanWrite)
            {
                isActiveProperty.SetValue(entity, false);
            }
            foreach (var prop in entityType.GetProperties())
            {
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    var collection = prop.GetValue(entity) as System.Collections.IEnumerable;
                    if (collection == null) continue;
                    foreach (var item in collection)
                    {
                        var itemType = item.GetType();
                        var method = typeof(BaseService).GetMethod(nameof(SoftDeleteRowInDatabase)).MakeGenericMethod(itemType);
                        method.Invoke(this, new object[] { item });
                    }
                }
            }
        }
    }
}
