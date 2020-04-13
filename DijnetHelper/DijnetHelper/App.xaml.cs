using Xamarin.Forms;

namespace DijnetHelper
{
    public partial class App : Application
    {
        public App(Page startPage)
        {
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
