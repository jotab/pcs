using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Mapping
{
    internal class EntityMapping : IEntityMapping
    {
        public string TableName { get; set; }
        public string Schema { get; set; }
        public IEnumerable<IPropertyMapping> Properties { get; set; }        
        
        public IEnumerable<IPropertyMapping> Pks
        {
            get { return Properties.Where(propertyMapping => propertyMapping.IsPk); }
        }

        public string FullTableName => string.IsNullOrEmpty(Schema?.Trim())
            ? $"[{TableName}]"
            : $"[{Schema}].[{TableName}]";
    }
}