using LoggingSample.Pages;
using Xamarin.Forms;

namespace LoggingSample
{
    public class App : Application
    {
        public App()
        {
            MainPage = new NavigationPage(new DevicesPage());
        }
    }
}
