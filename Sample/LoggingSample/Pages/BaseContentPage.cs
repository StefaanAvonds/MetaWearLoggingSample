using LoggingSample.Interfaces;
using Xamarin.Forms;

namespace LoggingSample.Pages
{
    public abstract class BaseContentPage<TViewModel> : ContentPage
        where TViewModel : IViewModel, new()
    {
        private TViewModel _viewModel;

        public TViewModel ViewModel
        {
            get => _viewModel;
            private set => _viewModel = value;
        }

        public BaseContentPage()
        {
            ViewModel = new TViewModel();
            BindingContext = ViewModel;
        }
    }
}
