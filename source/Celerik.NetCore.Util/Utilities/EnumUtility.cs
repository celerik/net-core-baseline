﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;

namespace Celerik.NetCore.Util
{
    /// <summary>
    /// Provides enum utilities.
    /// </summary>
    public static class EnumUtility
    {
        /// <summary>
        /// Gets an enum value from its DescriptionAttribute.
        /// </summary>
        /// <typeparam name="TEnum">Enumeration type.</typeparam>
        /// <param name="description">Description of the enum value to search.</param>
        /// <returns>Enum value for the given DescriptionAttribute.</returns>
        /// <param name="defaultVal">Value to return in case the description doesn't
        /// exist in the enum.</param>
        /// <exception cref="InvalidOperationException">TEnum is not an enum.</exception>
        public static TEnum GetValue<TEnum>(string description, TEnum defaultVal = default)
            where TEnum : struct, IConvertible
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
                throw new InvalidOperationException(UtilResources.Get("EnumUtility.NotAnEnum", type));

            foreach (var field in type.GetFields())
            {
                if (Attribute.GetCustomAttribute(
                    field, typeof(DescriptionAttribute)
                ) is DescriptionAttribute attribute)
                {
                    if (attribute.Description == description)
                        return (TEnum)field.GetValue(null);
                }

                if (field.Name == description)
                    return (TEnum)field.GetValue(null);
            }

            return defaultVal;
        }

        /// <summary>
        /// Gets an attribute belonging to this enumeration value. Returns null
        /// if the attribute does not exists.
        /// </summary>
        /// <typeparam name="TAttribute">The attribute type to get.</typeparam>
        /// <param name="value">Enum value.</param>
        /// <returns>Attribute belonging to this enumeration value.</returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            if (value == null)
                throw new ArgumentException(UtilResources.Get("EnumUtility.GetAttribute.NullValue"));

            var type = typeof(TAttribute);
            var memberInfo = value.GetType().GetMember(value.ToString());

            if (memberInfo.Length > 0)
                return memberInfo[0].GetCustomAttributes(type, inherit: false)
                    .OfType<TAttribute>().FirstOrDefault();

            return null;
        }

        /// <summary>
        /// Gets the code of this enumeration. In case the
        /// enumeration does not have a CodeAttribute, the enum
        /// name is returned.
        /// </summary>
        /// <param name="value">Enumeration value.</param>
        /// <returns>Code of this enumeration.</returns>
        public static string GetCode(this Enum value) =>
            value.GetAttribute<CodeAttribute>()?.Code ??
            value.ToString();

        /// <summary>
        /// Gets the description of this enumeration. In case the
        /// enumeration does not have a DescriptionAttribute, the enum
        /// name is returned.
        /// </summary>
        /// <param name="value">Enumeration value.</param>
        /// <returns>Description of this enumeration.</returns>
        public static string GetDescription(this Enum value) =>
            value.GetAttribute<DescriptionAttribute>()?.Description ??
            value.ToString();

        /// <summary>
        /// Gets the enum code from its integer value.
        /// </summary>
        /// <typeparam name="TEnum">Enumeration type.</typeparam>
        /// <param name="value">Enumeration integer value.</param>
        /// <param name="defaultVal">Value to return in case the value doesn't
        /// exist in the enum.</param>
        /// <returns>Enum code from its integer value.</returns>
        /// <exception cref="InvalidOperationException">TEnum is not an enum.</exception>
        public static string GetCode<TEnum>(int value, string defaultVal = null)
            where TEnum : struct, IConvertible
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
                throw new InvalidOperationException(UtilResources.Get("EnumUtility.NotAnEnum", type));
            if (!Enum.IsDefined(typeof(TEnum), value))
                return defaultVal;

            return ((Enum)Enum.ToObject(type, value)).GetCode();
        }

        /// <summary>
        /// Gets the enum description from its integer value.
        /// </summary>
        /// <typeparam name="TEnum">Enumeration type.</typeparam>
        /// <param name="value">Enumeration integer value.</param>
        /// <param name="defaultVal">Value to return in case the value doesn't
        /// exist in the enum.</param>
        /// <returns>Enum description from its integer value.</returns>
        /// <exception cref="InvalidOperationException">TEnum is not an enum.</exception>
        public static string GetDescription<TEnum>(int value, string defaultVal = null)
            where TEnum : struct, IConvertible
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
                throw new InvalidOperationException(UtilResources.Get("EnumUtility.NotAnEnum", type));
            if (!Enum.IsDefined(typeof(TEnum), value))
                return defaultVal;

            return ((Enum)Enum.ToObject(type, value)).GetDescription();
        }

        /// <summary>
        /// Gets the minimun value of an enum.
        /// </summary>
        /// <typeparam name="TEnum">Enumeration type.</typeparam>
        /// <returns>Minimun value of the enum.</returns>
        /// <exception cref="InvalidOperationException">TEnum is not an enum.</exception>
        public static int GetMin<TEnum>()
            where TEnum : struct, IConvertible
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
                throw new InvalidOperationException(UtilResources.Get("EnumUtility.NotAnEnum", type));

            return Enum.GetValues(typeof(TEnum)).Cast<int>().Min();
        }

        /// <summary>
        /// Gets the maximun value of an enum.
        /// </summary>
        /// <typeparam name="TEnum">Enumeration type.</typeparam>
        /// <returns>Maximun value of the enum.</returns>
        /// <exception cref="InvalidOperationException">TEnum is not an enum.</exception>
        public static int GetMax<TEnum>()
            where TEnum : struct, IConvertible
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
                throw new InvalidOperationException(UtilResources.Get("EnumUtility.NotAnEnum", type));

            return Enum.GetValues(typeof(TEnum)).Cast<int>().Max();
        }

        /// <summary>
        /// Converts an enum to a list of strings.
        /// </summary>
        /// <typeparam name="TEnum">Enumeration type.</typeparam>
        /// <returns>Enumeration converted to a list of strings.</returns>
        /// <exception cref="InvalidOperationException">TEnum is not an enum.</exception>
        public static List<string> ToList<TEnum>() where TEnum : struct, IConvertible
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
                throw new InvalidOperationException(UtilResources.Get("EnumUtility.NotAnEnum", type));

            var list = new List<string>();

            foreach (var field in type.GetFields().OrderBy(field => field.MetadataToken))
            {
                if (field.IsStatic)
                {
                    var description = Attribute.GetCustomAttribute(
                        field, typeof(DescriptionAttribute)
                    ) is DescriptionAttribute descAttribute
                        ? descAttribute.Description
                        : field.Name;

                    list.Add(description);
                }
            }

            return list;
        }

        /// <summary>
        /// Converts an enum to a list of objects.
        /// </summary>
        /// <typeparam name="TEnum">Enumeration type.</typeparam>
        /// <typeparam name="TList">List type.</typeparam>
        /// <param name="valueProp">Property name for the vale in the list.</param>
        /// <param name="descriptionProp">Property name for the description in the list.</param>
        /// <param name="codeProp">Property name for the code in the list.</param>
        /// <returns>Enumeration converted to a list of objects.</returns>
        /// <exception cref="InvalidOperationException">TEnum is not an enum.</exception>
        public static List<TList> ToList<TEnum, TList>(
            string valueProp,
            string descriptionProp,
            string codeProp = null)
                where TEnum : struct, IConvertible
                where TList : class
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
                throw new InvalidOperationException(UtilResources.Get("EnumUtility.NotAnEnum", type));

            var list = new List<TList>();

            foreach (var field in type.GetFields())
            {
                if (field.IsStatic)
                {
                    dynamic expando = new ExpandoObject();
                    var dictionary = expando as IDictionary<string, object>;

                    dictionary[valueProp] = (int)field.GetValue(null);

                    dictionary[descriptionProp] = Attribute.GetCustomAttribute(
                            field, typeof(DescriptionAttribute)
                        ) is DescriptionAttribute descAttribute
                            ? descAttribute.Description
                            : field.Name;

                    if (codeProp != null)
                    {
                        dictionary[codeProp] = Attribute.GetCustomAttribute(
                            field, typeof(CodeAttribute)
                        ) is CodeAttribute codeAttribute
                            ? codeAttribute.Code
                            : null;
                    }

                    var json = JsonConvert.SerializeObject(expando);
                    var obj = JsonConvert.DeserializeObject<TList>(json);

                    list.Add(obj);
                }
            }

            return list.AsQueryable().OrderBy(valueProp).ToList();
        }
    }
}
