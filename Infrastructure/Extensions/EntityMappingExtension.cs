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

        private static IEnumerable<IPropertyMapping> GetPropertyMappings(this Type type)
        {
            var properties = type
                .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance)
                .Where(info =>
                    info.PropertyType.IsPrimitive || info.PropertyType.IsValueType ||
                    info.PropertyType == typeof(string));
            return properties
                .Where(property => property.GetAttributeValue((NotMappedAttribute key) => key) == null)
                .Select(property =>
                    {
                        var map = new PropertyMapping
                        {
                            PropertyName = property.Name,
                            ColumnName = property.GetAttributeValue((ColumnAttribute column) => column.Name) ??
                                         property.Name,
                            IsPk = property.GetAttributeValue((KeyAttribute key) => key) != null,
                            IsDbGenerated = property.GetAttributeValue((DatabaseGeneratedAttribute dbGenerated) =>
                                dbGenerated.DatabaseGeneratedOption != DatabaseGeneratedOption.None),
                            IsFk = property.GetAttributeValue((ForeignKeyAttribute key) => key) != null
                        };

                        return map;
                    }
                );
        }

        public static object GetPropertyValue<T>(this T entity, IPropertyMapping propertyMap)
        {
            return entity.GetType().GetProperty(propertyMap.PropertyName).GetValue(entity);
        }

        public static void SetPropertyValue<T>(this T entity, IPropertyMapping propertyMap, object value)
        {
            entity.GetType().GetProperty(propertyMap.PropertyName).SetValue(entity, value);
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

        public static IEntityMapping GetEntityMapping(this Type type)
        {
            return new EntityMapping
            {
                TableName = type.GetTableName(),
                Properties = type.GetPropertyMappings(),
                Schema = type.GetSchemaName(),
            };
        }

        #endregion

        #region Entity mapping SQL creation

        public static string GetSelectSql(this IEntityMapping mapping)
        {
            return
                $"{SqlTerm.Select} {string.Join(",", mapping.Properties.GetColumnNames())} {SqlTerm.From} {mapping.TableName} ";
        }

        public static string GetDeleteSql(this IEntityMapping mapping)
        {
            return $"{SqlTerm.Delete} {SqlTerm.From} {mapping.TableName} ";
        }

        public static string GetInsertSql(this IEntityMapping mapping)
        {
            var properties = mapping.Properties.Where(propertyMapping => !propertyMapping.IsDbGenerated).ToList();
            return
                $"{SqlTerm.Insert} {SqlTerm.Into} {mapping.TableName} ({string.Join(",", properties.GetColumnNames())}) {SqlTerm.Values} (:{string.Join(",", properties.GetPropertyNames())}) ";
        }

        public static string GetUpdateSql(this IEntityMapping mapping)
        {
            var setStatements = mapping.Properties
                .Where(propertyMapping => !propertyMapping.IsDbGenerated && !propertyMapping.IsPk)
                .Select(propertyMapping => $"{propertyMapping.ColumnName} = :{propertyMapping.PropertyName}");
            var keyComparator = mapping.Properties
                .Where(propertyMapping => propertyMapping.IsPk)
                .Select(propertyMapping => $"{propertyMapping.ColumnName} = :{propertyMapping.PropertyName}");

            return
                $"{SqlTerm.Update} {mapping.TableName} {SqlTerm.Set} {string.Join(",", setStatements)} {SqlTerm.Where} {string.Join(" AND ", keyComparator)}";
        }

        #endregion
    }
}