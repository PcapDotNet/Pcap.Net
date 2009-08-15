using System.Reflection;

namespace PcapDotNet.Base
{
    public static class MorePropertyInfo
    {
        public static object GetValue(this PropertyInfo propertyInfo, object obj)
        {
            return propertyInfo.GetValue(obj, null);
        }
    }
}