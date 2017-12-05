using LoggingSample.Models;
using LoggingSample.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LoggingSample.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DevicesPage : BaseContentPage<DevicesViewModel>
	{
		public DevicesPage()
            : base()
		{
			InitializeComponent();
		}

        private void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            ViewModel.TapItem(e.Item as MetaWearModel);
        }
    }
}