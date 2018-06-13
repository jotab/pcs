using System.Collections.Generic;

namespace Infrastructure.Mapping
{
    public interface IEntityMapping
    {
        string Name { get; }
        string TableName { get; }
        string Schema { get; }
        string FullTableName { get; }
        IEnumerable<IPropertyMapping> Properties { get; }
        IEnumerable<IPropertyMapping> ValueProperties { get; }
        IEnumerable<IPropertyMapping> Pks { get; }
        IEnumerable<IPropertyMapping> Fks { get; }
        IEnumerable<IPropertyMapping> NavigationProperties { get; }
    }
}