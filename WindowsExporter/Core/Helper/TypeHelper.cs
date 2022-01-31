using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WindowsExporter.Core.Helper
{
    public static class TypeHelper
    {
        public static IEnumerable<Type> GetTypesFromBase<TBase>()
            where TBase : class
        {
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetTypes().Where(_ => typeof(TBase).IsAssignableFrom(_) && !_.IsAbstract && !_.IsInterface);
        }
    }
}
