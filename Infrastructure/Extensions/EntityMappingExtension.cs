using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using Infrastructure.Helpers;
using Infrastructure.Mapping;

namespace Infrastructure.Extensions
{
    internal static class EntityMappingExtension
    {
        #region Entity mapping metadata

        private static string GetTableName(this Type type)
        {
            return type.GetAttributeValue((TableAttribute table) => table.Name) ?? type.Name;
        }

        private static string GetSchemaName(this Type type)
        {
            return type.GetAttributeValue((TableAttribute table) => table.Schema);
        }

        private static IEnumerable<IPropertyMapping> GetPropertyMappings(this IReflect type)
        {
            var properties = type
                .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
            return properties
                .Where(property => property.GetAttributeValue((NotMappedAttribute key) => key) == null)
                .Select(property =>
                    {
                        var fk = property.GetAttributeValue((ForeignKeyAttribute key) => key);

                        var map = new PropertyMapping
                        {
                            PropertyName = property.Name,
                            ColumnName = property.GetAttributeValue((ColumnAttribute column) => column.Name) ??
                                         property.Name,
                            IsPk = property.GetAttributeValue((KeyAttribute key) => key) != null,
                            IsDbGenerated = property.GetAttributeValue((DatabaseGeneratedAttribute dbGenerated) =>
                                dbGenerated.DatabaseGeneratedOption != DatabaseGeneratedOption.None),
                            IsFk = fk != null,
                            RelatedProperty = fk?.Name,
                            IsNavigation = !IsValueProperty(property),
                            PropertyInfo = property
                        };

                        return map;
                    }
                );
        }

        private static bool IsValueProperty(PropertyInfo info)
        {
            return info.PropertyType.IsPrimitive || info.PropertyType.IsValueType ||
                   info.PropertyType == typeof(string);
        }

        public static object GetPropertyValue<T>(this T entity, IPropertyMapping propertyMap)
        {
            return propertyMap.PropertyInfo.GetValue(entity);
        }

        public static void SetPropertyValue<T>(this T entity, IPropertyMapping propertyMap, object value)
        {
            propertyMap.PropertyInfo.SetValue(entity, value);
        }

        private static IEnumerable<string> GetColumnNames(this IEnumerable<IPropertyMapping> propertyMappings)
        {
            return propertyMappings.Select(mapping => mapping.ColumnName);
        }

        private static IEnumerable<string> GetPropertyNames(this IEnumerable<IPropertyMapping> propertyMappings)
        {
            return propertyMappings.Select(mapping => mapping.PropertyName);
        }

        public static IPropertyMapping GetPropertyByName(this IEntityMapping mapping, string propertyName)
        {
            return mapping.Properties.FirstOrDefault(propertyMapping =>
                propertyMapping.PropertyName.Equals(propertyName));
        }

        public static IEntityMapping CreateEntityMapping(this Type type)
        {
            return new EntityMapping
            {
                Name = type.Name,
                TableName = type.GetTableName(),
                Properties = type.GetPropertyMappings(),
                Schema = type.GetSchemaName()
            };
        }

        #endregion

        #region Entity mapping SQL creation

        public static string GetSelectSql(this IEntityMapping mapping)
        {
            return
                $"{SqlTerm.Select} {string.Join(",", mapping.ValueProperties.GetColumnNames())} {SqlTerm.From} {mapping.TableName} ";
        }

        public static string GetDeleteSql(this IEntityMapping mapping)
        {
            return $"{SqlTerm.Delete} {SqlTerm.From} {mapping.TableName} ";
        }

        public static string GetInsertSql(this IEntityMapping mapping)
        {
            var properties = mapping.ValueProperties.Where(propertyMapping => !propertyMapping.IsDbGenerated).ToList();
            return
                $"{SqlTerm.Insert} {SqlTerm.Into} {mapping.TableName} ({string.Join(",", properties.GetColumnNames())}) {SqlTerm.Values} (:{string.Join(",", properties.GetPropertyNames())}) ";
        }

        public static string GetUpdateSql(this IEntityMapping mapping)
        {
            var setStatements = mapping.ValueProperties
                .Where(propertyMapping => !propertyMapping.IsDbGenerated && !propertyMapping.IsPk)
                .Select(propertyMapping => $"{propertyMapping.ColumnName} = :{propertyMapping.PropertyName}");
            var keyComparator = mapping.Pks
                .Select(propertyMapping => $"{propertyMapping.ColumnName} = :{propertyMapping.PropertyName}");

            return
                $"{SqlTerm.Update} {mapping.TableName} {SqlTerm.Set} {string.Join(",", setStatements)} {SqlTerm.Where} {string.Join(" AND ", keyComparator)}";
        }

        #endregion
    }
}