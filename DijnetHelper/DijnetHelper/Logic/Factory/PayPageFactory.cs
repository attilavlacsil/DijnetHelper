using DijnetHelper.Model;
using DijnetHelper.Pages;
using Unity;
using Unity.Resolution;

namespace DijnetHelper.Logic.Factory
{
    public class PayPageFactory : Factory
    {
        public PayPageFactory(IUnityContainer container)
            : base(container)
        {
        }

        public PayPage Create(Bill bill)
        {
            return Container.Resolve<PayPage>(
                new ParameterOverride(typeof(Bill), bill));
        }
    }
}