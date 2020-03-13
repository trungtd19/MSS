using MSS_DEMO.Common;
using MSS_DEMO.Core.Import;
using MSS_DEMO.Core.Interface;
using MSS_DEMO.Repository;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace MSS_DEMO
{
    public static class UnityDI
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            container.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType<IGetRow, GetRow>();
            container.RegisterType(typeof (AuthorizeAttribute), typeof (CheckCredentialAttribute));
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}