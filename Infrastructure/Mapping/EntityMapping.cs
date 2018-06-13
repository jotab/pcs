using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Mapping
{
    internal class EntityMapping : IEntityMapping
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public string Schema { get; set; }
        public IEnumerable<IPropertyMapping> Properties { get; set; }

        public IEnumerable<IPropertyMapping> ValueProperties
        {
            get { return Properties.Where(propertyMapping => !propertyMapping.IsNavigation); }
        }

        public IEnumerable<IPropertyMapping> Pks
        {
            get { return Properties.Where(propertyMapping => propertyMapping.IsPk); }
        }

        public IEnumerable<IPropertyMapping> Fks
        {
            get { return Properties.Where(propertyMapping => propertyMapping.IsFk); }
        }

        public IEnumerable<IPropertyMapping> NavigationProperties
        {
            get { return Properties.Where(propertyMapping => propertyMapping.IsNavigation); }
        }

        public string FullTableName => string.IsNullOrEmpty(Schema?.Trim())
            ? $"[{TableName}]"
            : $"[{Schema}].[{TableName}]";
    }
}