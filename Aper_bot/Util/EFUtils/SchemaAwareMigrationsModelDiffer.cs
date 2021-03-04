/*
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
#pragma warning disable EF1001 // Internal EF Core API usage.

namespace Aper_bot.Util.EFUtils
{
    
    public class SchemaAwareMigrationsModelDiffer : MigrationsModelDiffer
    {
        public SchemaAwareMigrationsModelDiffer(
            IRelationalTypeMappingSource typeMappingSource, 
            IMigrationsAnnotationProvider migrationsAnnotations, 
            IChangeDetector changeDetector,
            IUpdateAdapterFactory updateAdapterFactory, 
            CommandBatchPreparerDependencies commandBatchPreparerDependencies) : base(typeMappingSource, migrationsAnnotations,
            changeDetector, updateAdapterFactory, commandBatchPreparerDependencies)
        {
        }
        
        private static ReferentialAction ToReferentialAction(DeleteBehavior deleteBehavior)
            => deleteBehavior == DeleteBehavior.Cascade
                ? ReferentialAction.Cascade
                : deleteBehavior == DeleteBehavior.SetNull
                    ? ReferentialAction.SetNull
                    : ReferentialAction.Restrict;
        
        protected override IEnumerable<MigrationOperation> Diff(
            IEnumerable<IForeignKeyConstraint> source,
            IEnumerable<IForeignKeyConstraint> target,
            DiffContext diffContext)
        {
            return DiffCollection(
                source,
                target,
                diffContext,
                Diff,
                Add,
                Remove,
                (s, t, c) =>
                {
                    if (s.Name != t.Name)
                    {
                        return false;
                    }

                    if (!s.Columns.Select(p => p.Name).SequenceEqual(
                        t.Columns.Select(p => c.FindSource(p)?.Name)))
                    {
                        return false;
                    }

                    var schemaToInclude = ((SchemaAwareDiffContext) diffContext).Source.Relational().DefaultSchema;

                    if (c.FindSourceTable(s.PrincipalEntityType).Schema == schemaToInclude &&
                        c.FindSourceTable(s.PrincipalEntityType) !=
                        c.FindSource(c.FindTargetTable(t.PrincipalEntityType)))
                    {
                        return false;
                    }

                    if (t.PrincipalKey.Properties.Select(p => c.FindSource(p)?.Relational().ColumnName)
                        .First() != null && !s.PrincipalKey.Properties
                        .Select(p => p.Relational().ColumnName).SequenceEqual(
                            t.PrincipalKey.Properties.Select(p =>
                                c.FindSource(p)?.Relational().ColumnName)))
                    {
                        return false;
                    }

                    if (ToReferentialAction(s.DeleteBehavior) != ToReferentialAction(t.DeleteBehavior))
                    {
                        return false;
                    }

                    return !HasDifferences(MigrationsAnnotations.For(s), MigrationsAnnotations.For(t));
                }
            );
        }
        
        
    }
}
*/