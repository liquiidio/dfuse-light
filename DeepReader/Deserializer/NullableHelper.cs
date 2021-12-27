using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace DeepReader.Deserializer
{
    public class NullableHelper
    {
        public class NullableHelperHelper
        {
            public static object? Helper;

            private static CustomAttributeData nullableAttribute;

            public static CustomAttributeData NullableAttribute = nullableAttribute ?? GetNullableAttribute();

            private static CustomAttributeData GetNullableAttribute()
            {
                nullableAttribute = typeof(NullableHelper).GetFields()[0].CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
                return nullableAttribute;
            }
        }


        public static bool IsNullable(PropertyInfo property) =>
            IsNullableHelper(property.PropertyType, property.DeclaringType, property.CustomAttributes);

        public static bool IsNullable(FieldInfo field) =>
            IsNullableHelper(field.FieldType, field.DeclaringType, field.CustomAttributes);

        public static bool IsNullable(ParameterInfo parameter) =>
            IsNullableHelper(parameter.ParameterType, parameter.Member, parameter.CustomAttributes);

        private static bool IsNullableHelper(Type memberType, MemberInfo? declaringType, IEnumerable<CustomAttributeData> customAttributes)
        {
            if (memberType.IsValueType)
                return Nullable.GetUnderlyingType(memberType) != null;

            var nullable = customAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
            if (nullable != null && nullable.ConstructorArguments.Count == 1)
            {
                var attributeArgument = nullable.ConstructorArguments[0];
                if (attributeArgument.ArgumentType == typeof(byte[]))
                {
                    var args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value!;
                    if (args.Count > 0 && args[0].ArgumentType == typeof(byte))
                    {
                        return (byte)args[0].Value! == 2;
                    }
                }
                else if (attributeArgument.ArgumentType == typeof(byte))
                {
                    return (byte)attributeArgument.Value! == 2;
                }
            }

            for (var type = declaringType; type != null; type = type.DeclaringType)
            {
                var context = type.CustomAttributes
                    .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
                if (context != null &&
                    context.ConstructorArguments.Count == 1 &&
                    context.ConstructorArguments[0].ArgumentType == typeof(byte))
                {
                    bool res = (byte)context.ConstructorArguments[0].Value! == 2;
                    return res;
                }
            }

            // Couldn't find a suitable attribute
            return false;
        }
    }
}
