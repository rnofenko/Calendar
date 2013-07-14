using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Bs.Calendar.Mvc.Server
{
    public class MvcDependencyResolver : IDependencyResolver
    {
        public object GetService(Type serviceType)
        {
            try
            {
                return Core.Resolver.Resolve(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return Core.Resolver.ResolveAll(serviceType);
            }
            catch
            {
                return new List<object>();
            }
        }
    }
}