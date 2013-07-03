using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Unity.LoggingExtension
{
    public static class TypeExtensions
    {
        public static IEnumerable<T> GetTypeMembersCustomAttributes<T>(this Type type) where T : Attribute
        {
            return type.GetMembers().SelectMany(m => m.GetCustomAttributes<T>());
        }

        public static IEnumerable<T> GetTypeInterfaceCustomAttributes<T>(this Type type) where T : Attribute
        {
            return type.GetInterfaces().SelectMany(i => i.GetCustomAttributes<T>());
        }

        public static IEnumerable<T> GetTypeInterfaceMembersCustomAttributes<T>(this Type type) where T : Attribute
        {
            return type.GetInterfaces().SelectMany(i => i.GetMembers().SelectMany(m => m.GetCustomAttributes<T>()));
        }

        public static IEnumerable<T> GetMemberCustomAttributes<T>(this MemberInfo memberInfo) where T : Attribute
        {
            var result = memberInfo.GetCustomAttributes<T>().ToList();
            if (result.Any())
                return result;

            if (memberInfo.DeclaringType != null && memberInfo.DeclaringType.IsClass)
            {
                result = memberInfo.DeclaringType.GetCustomAttributes<T>().ToList();
                if (result.Any())
                    return result;

                foreach (var @interface in memberInfo.DeclaringType.GetInterfaces())
                {
                    var map = memberInfo.DeclaringType.GetInterfaceMap(@interface);
                    if (map.InterfaceMethods.Any())
                    {
                        var interfaceMethod = map.InterfaceMethods[map.TargetMethods.Cast<MemberInfo>().ToList().IndexOf(memberInfo)];
                        result = interfaceMethod.GetCustomAttributes<T>().ToList();
                        if (result.Any())
                            return result;

                        result = map.InterfaceType.GetCustomAttributes<T>().ToList();
                        if (result.Any())
                            return result;
                    }
                }
            }

            return Enumerable.Empty<T>();
        }

        public static bool HasCustomerAttributes<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes<T>().Any() ||
                   type.GetTypeMembersCustomAttributes<T>().Any() ||
                   type.GetTypeInterfaceCustomAttributes<T>().Any() ||
                   type.GetTypeInterfaceMembersCustomAttributes<T>().Any();
        }
    }
}
