using System;
using System.Reflection;

namespace Xunit
{
    class AppDomainManager_NoAppDomain : IAppDomainManager
    {
        readonly string assemblyFileName;

        public AppDomainManager_NoAppDomain(string assemblyFileName)
        {
            Guard.ArgumentNotNullOrEmpty(nameof(assemblyFileName), assemblyFileName);

            this.assemblyFileName = assemblyFileName;
        }

        public bool HasAppDomain => false;

        public TObject? CreateObject<TObject>(AssemblyName assemblyName, string typeName, params object?[]? args)
            where TObject : class
        {
            Guard.ArgumentNotNull(nameof(assemblyName), assemblyName);
            Guard.ArgumentNotNullOrEmpty(nameof(typeName), typeName);

            try
            {
#if NETFRAMEWORK
                var type = Assembly.Load(assemblyName).GetType(typeName, throwOnError: true);
#else
                var type = Type.GetType($"{typeName}, {assemblyName.FullName}", throwOnError: true);
#endif
                if (type == null)
                    return default;

                return (TObject?)Activator.CreateInstance(type, args);
            }
            catch (TargetInvocationException ex)
            {
                ex.InnerException!.RethrowWithNoStackTraceLoss();
                return default;
            }
        }

#if NETFRAMEWORK
        public TObject? CreateObjectFrom<TObject>(string assemblyLocation, string typeName, params object?[]? args)
            where TObject : class
        {
            Guard.ArgumentNotNullOrEmpty(nameof(assemblyLocation), assemblyLocation);
            Guard.ArgumentNotNullOrEmpty(nameof(typeName), typeName);

            try
            {
                var type = Assembly.LoadFrom(assemblyLocation).GetType(typeName, throwOnError: true);
                if (type == null)
                    return default;

                return (TObject?)Activator.CreateInstance(type, args);
            }
            catch (TargetInvocationException ex)
            {
                ex.InnerException!.RethrowWithNoStackTraceLoss();
                return default;
            }
        }
#endif

        public void Dispose() { }
    }
}
