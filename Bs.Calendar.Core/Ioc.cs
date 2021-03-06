﻿using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace Bs.Calendar.Core
{
    public class Ioc
    {
        private static IUnityContainer _container = new UnityContainer();

        public static object Resolve(Type type)
        {
            return _container.Resolve(type);
        }

        public static T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public static IEnumerable<object> ResolveAll(Type type)
        {
            return _container.ResolveAll(type);
        }

        public static IEnumerable<T> ResolveAll<T>()
        {
            return _container.ResolveAll<T>();
        }

        public static void RegisterType<TInterface, TClass>()
        {
            _container.RegisterType(typeof(TInterface), typeof(TClass));
        }

        public static void RegisterType<TInterface, TClass>(string name)
        {
            _container.RegisterType(typeof (TInterface), typeof (TClass), name);
        }

        public static void RegisterInstance<TInterface>(object instance)
        {
            _container.RegisterInstance(typeof(TInterface), instance);
        }

        public static void RegisterInstance(Type type, object instance)
        {
            _container.RegisterInstance(type, instance);
        }

        public static void Reset()
        {
            _container = new UnityContainer();
        }
    }
}
