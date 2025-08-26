using TL.Travel.DataAccess.Base;

namespace TL.Travel.Mockups
{
    public static class InitializeMockups
    {
        private static bool isInitialized = false;

        public static void Initialize(BaseTLTravelDbContext db)
        {
            if (!isInitialized)
            {
                if (db is null)
                {
                    throw new ArgumentNullException(nameof(db));
                }

                db.Operators.AddRange(OperatorMockups.Operators);
                db.SaveChanges();
                isInitialized = true;
            }
        }
    }
}
