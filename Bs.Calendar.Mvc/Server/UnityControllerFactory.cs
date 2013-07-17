using System;
using System.Web.Mvc;

namespace Bs.Calendar.Mvc.Server
{
    public class UnityControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            IController result = null;

            if (null != controllerType)
            {
                result = Core.Resolver.Resolve(controllerType) as IController;
            }

            return result;
        }
    }
}