using System.Reflection;

namespace Infrastructure.Mapping
{
    internal class PropertyMapping : IPropertyMapping
    {
        public string PropertyName { get; set; }
        public string ColumnName { get; set; }
        public bool IsPk { get; set; }
        public bool IsFk { get; set; }
        public bool IsDbGenerated { get; set; }
        public bool IsNavigation { get; set; }
        public string RelatedProperty { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
    }
}