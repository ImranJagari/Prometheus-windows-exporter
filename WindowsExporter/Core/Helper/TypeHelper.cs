using System.Reflection;

namespace WindowsExporter.Core.Helper
{
    public static class TypeHelper
    {
        public static IEnumerable<Type> GetTypesFromBase<TBase>()
            where TBase : class
        {
            var asm = Assembly.GetExecutingAssembly();
            return asm.GetTypes().Where(_ => _.IsAssignableTo(typeof(TBase)) && !_.IsAbstract && !_.IsInterface);
        }
    }
}
