using System;
using Unity;

namespace DijnetHelper.Logic.Factory
{
    public abstract class Factory
    {
        protected readonly IUnityContainer Container;

        protected Factory(IUnityContainer container)
        {
            Container = container ?? throw new ArgumentNullException(nameof(container));
        }
    }
}