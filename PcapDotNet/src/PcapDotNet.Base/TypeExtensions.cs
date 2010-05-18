using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for Type.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns all the possible values for the given enum type.
        /// </summary>
        public static IEnumerable<T> GetEnumValues<T>(this Type type)
        {
            return (IEnumerable<T>)Enum.GetValues(type);
        }
    }

    /// <summary>
    /// Extension methods for MemberInfo.
    /// </summary>
    public static class MemberInfoExtensions
    {
        /// <summary>
        /// When overridden in a derived class, returns a sequence of custom attributes identified by System.Type.
        /// </summary>
        /// <typeparam name="T">TThe type of attribute to search for. Only attributes that are assignable to this type are returned.</typeparam>
        /// <param name="memberInfo">The memberInfo to look the attributes on.</param>
        /// <param name="inherit">Specifies whether to search this member's inheritance chain to find the attributes.</param>
        /// <returns>A sequence of custom attributes applied to this member, or a sequence with zero (0) elements if no attributes have been applied.</returns>
        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo memberInfo, bool inherit) where T : Attribute
        {
            if (memberInfo == null)
                throw new ArgumentNullException("memberInfo");

            return memberInfo.GetCustomAttributes(typeof(T), inherit).Cast<T>();
        }
    }
}