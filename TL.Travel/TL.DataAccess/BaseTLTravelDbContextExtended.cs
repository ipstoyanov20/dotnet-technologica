using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TL.AspNet.Security.Database.Abstractions.DbContexts;
using TL.DataAccess.Models;

namespace TL.Travel.DataAccess.Base
{
    public partial class BaseTLTravelDbContext : IPasswordUserDbContext<Operator, decimal>
    {

        public static string AuditUserName
        {
            get
            {
                string auditUserName = Thread.CurrentPrincipal?.Identity?.Name;
                return !string.IsNullOrEmpty(auditUserName) ? auditUserName : "SYSTEM";
            }
        }

        public DbSet<Operator> SecurityUsers => this.Operators;

        private static readonly string[] AuditableProperties = new string[]
        {
            "CreatedBy",
            "CreatedOn",
            "UpdatedBy",
            "UpdatedOn"
        };


        public override int SaveChanges()
        {
            ApplyAudit(this.ChangeTracker);

            return base.SaveChanges();
        }


        private static void ApplyAudit(ChangeTracker changeTracker)
        {
            foreach (EntityEntry entry in changeTracker.Entries().Where(t => t.State == EntityState.Added && IsAuditableEntity(t.Entity)))
            {
                AddedEntity(entry.Entity);

                //Валидация трябва да се случва след set-ване на одит полетата
                var validationContext = new ValidationContext(entry.Entity);
                Validator.ValidateObject(entry.Entity, validationContext);
            }

            foreach (EntityEntry entry in changeTracker.Entries().Where(t => t.State == EntityState.Modified && IsAuditableEntity(t.Entity)))
            {
                ModifiedEntity(entry.Entity);

                var validationContext = new ValidationContext(entry.Entity);
                Validator.ValidateObject(entry.Entity, validationContext);
            }
        }

        private static void AddedEntity(object entity)
        {
            Type t = entity.GetType();
            t.GetProperty("CreatedBy").SetValue(entity, AuditUserName);
            t.GetProperty("CreatedOn").SetValue(entity, DateTime.Now);
        }

        private static void ModifiedEntity(object entity)
        {
            Type t = entity.GetType();
            t.GetProperty("UpdatedBy").SetValue(entity, AuditUserName);
            t.GetProperty("UpdatedOn").SetValue(entity, DateTime.Now);
        }

        private static bool IsAuditableEntity(object entity)
        {
            return AuditableProperties.All(x => entity.GetType().GetProperties().Select(y => y.Name).Contains(x));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
    }
}
