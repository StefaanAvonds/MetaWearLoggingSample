using LoggingSample.ViewModels;
using MbientLab.MetaWear;
using Xamarin.Forms.Xaml;

namespace LoggingSample.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConnectedDevicePage : BaseContentPage<ConnectedDeviceViewModel>
	{
		public ConnectedDevicePage(IMetaWearBoard board)
            : base()
		{
			InitializeComponent();
            ViewModel.SetBoard(board);
        }
    }
}