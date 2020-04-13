using System.Text;
using Xamarin.Forms;

namespace DijnetHelper
{
    public partial class App : Application
    {
        public App(Page startPage)
        {
            // required to handle HTML encoding
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            InitializeComponent();

            MainPage = new NavigationPage(startPage);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
