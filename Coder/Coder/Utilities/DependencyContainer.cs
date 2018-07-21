using System.Linq;
using Unity;

namespace Xarivu.Coder.Utilities
{
    public static class DependencyContainer
    {
        static IUnityContainer container;

        static DependencyContainer()
        {
            IUnityContainer uc = new UnityContainer();
            container = uc;
        }

        public static T Get<T>()
        {
            return container.Resolve<T>();
        }

        public static void RegisterTransient<T>()
        {
            container.RegisterType<T>();
        }

        public static void RegisterSingleton<T>(T instance)
        {
            container.RegisterInstance(typeof(T), instance);
        }

        public static bool IsTypeRegistered<TRegistered>()
        {
            return IsTypeRegistered<TRegistered, TRegistered>();
        }

        public static bool IsTypeRegistered<TRegistered, TMapped>()
        {
            return container.Registrations.FirstOrDefault(r => r.RegisteredType == typeof(TRegistered) && r.MappedToType == typeof(TMapped)) != null;
        }
    }
}
