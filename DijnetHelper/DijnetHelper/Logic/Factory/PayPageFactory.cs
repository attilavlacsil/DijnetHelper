using DijnetHelper.Pages;
using Unity;
using Unity.Resolution;
using Bill = DijnetHelper.Logic.Model.Bill;

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