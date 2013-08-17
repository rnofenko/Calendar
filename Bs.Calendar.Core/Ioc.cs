using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace Bs.Calendar.Core
{
    public class Ioc
    {
        private static readonly IUnityContainer _container = new UnityContainer();

        public static object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public static T Resolve<T>()
        {
            //Construct new instance of type

            return (T)Resolve(typeof(T));
        }

        public static IEnumerable<object> ResolveAll(Type type)
        {
            return _container.ResolveAll(type);
        }

        public static void RegisterType<TInterface, TClass>()
        {
            _container.RegisterType(typeof(TInterface), typeof(TClass));
        }

        public static void RegisterInstance<TInterface>(object instance)
        {
            _container.RegisterInstance(typeof(TInterface), instance);
        }

        public static void RegisterInstance(Type type, object instance)
        {
            _container.RegisterInstance(type, instance);
        }

        public static void BuildUp<TInjected>(object instance)
        {
            //Inject existing object

            _container.BuildUp(typeof (TInjected), instance);
        }
    }
}
