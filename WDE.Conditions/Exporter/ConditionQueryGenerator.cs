using System.Collections.Generic;
using System.Linq;
using WDE.Common.Database;
using WDE.Module.Attributes;
using WDE.SqlQueryGenerator;

namespace WDE.Conditions.Exporter
{
    [AutoRegister]
    public class ConditionQueryGenerator : IConditionQueryGenerator
    {
        public IQuery BuildDeleteQuery(IDatabaseProvider.ConditionKey key)
        {
            return Queries.Table("conditions")
                .Where(row => row.Column<int>("SourceTypeOrReferenceId") == key.SourceType &&
                              (!key.SourceGroup.HasValue || row.Column<int>("SourceGroup") == key.SourceGroup.Value) &&
                              (!key.SourceEntry.HasValue || row.Column<int>("SourceEntry") == key.SourceEntry.Value) &&
                              (!key.SourceId.HasValue || row.Column<int>("SourceId") == key.SourceId.Value))
                .Delete();
        }

        public IQuery BuildInsertQuery(IList<IConditionLine> conditions)
        {
            return Queries.Table("conditions")
                .BulkInsert(conditions.Select(c => c.ToSqlObject()));
        }
    }
}
