using System;
using System.Reflection;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for PropertyInfo.
    /// </summary>
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Returns the value of the given instance's non-indexed property.
        /// </summary>
        public static object GetValue(this PropertyInfo propertyInfo, object instanceWithProperty)
        {
            if (propertyInfo == null) 
                throw new ArgumentNullException("propertyInfo");

            return propertyInfo.GetValue(instanceWithProperty, null);
        }
    }
}