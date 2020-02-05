using MSS_DEMO.Core.Import;
using MSS_DEMO.Core.Interface;
using MSS_DEMO.Repository;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace MSS_DEMO
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();
            container.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType<IGetRow, GetRow>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}