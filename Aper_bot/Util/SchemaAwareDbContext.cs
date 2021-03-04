using Microsoft.EntityFrameworkCore;

namespace Aper_bot.Util
{
    public abstract class SchemaAwareDbContext : DbContext
    {
        protected readonly string _schema;

        protected SchemaAwareDbContext(string schema)
        {
            _schema = schema;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema(_schema);

            var types = modelBuilder.Model.GetEntityTypes();
            foreach (var entityType in types)
            {
                if(entityType.GetSchema() != _schema)
                    entityType.SetIsTableExcludedFromMigrations(true);
            }

        }
    }
}