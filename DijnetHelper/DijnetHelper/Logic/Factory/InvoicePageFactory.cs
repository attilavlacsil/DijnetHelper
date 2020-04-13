using DijnetHelper.Pages;
using Unity;

namespace DijnetHelper.Logic.Factory
{
    public class InvoicePageFactory : Factory
    {
        public InvoicePageFactory(IUnityContainer container) 
            : base(container)
        {
        }

        public BillPage Create()
        {
            return Container.Resolve<BillPage>();
        }
    }
}