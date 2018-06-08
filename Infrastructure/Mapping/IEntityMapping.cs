using System.Collections.Generic;

namespace Infrastructure.Mapping
{
    internal interface IEntityMapping
    {
        string TableName { get; }
        string Schema { get; }
        string FullTableName { get; }
        IEnumerable<IPropertyMapping> Properties { get; }
        IEnumerable<IPropertyMapping> Pks { get; }
    }
}