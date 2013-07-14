using System.Web.Mvc;
using Bs.Calendar.Core;

namespace Bs.Calendar.Mvc.Server
{
    public class DiMvc
    {
        public static void Register()
        {
            Resolver.RegisterType<IControllerFactory, UnityControllerFactory>();
        }
    }
}