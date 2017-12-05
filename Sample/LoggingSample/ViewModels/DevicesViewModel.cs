using LoggingSample.Models;
using LoggingSample.Pages;
using MetaWear.NetStandard;
using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LoggingSample.ViewModels
{
    public class DevicesViewModel : BaseViewModel
    {
        private readonly MWScanner _scanner;

        private ObservableCollection<MetaWearModel> _devices;

        public ObservableCollection<MetaWearModel> Devices
        {
            get => _devices;
            private set => SetField(ref _devices, value);
        }

        public DevicesViewModel()
            : base()
        {
            _scanner = new MWScanner();
            Devices = new ObservableCollection<MetaWearModel>();

            if (CrossBleAdapter.Current.Status == AdapterStatus.PoweredOn)
            {
                StartScan();
            }
            else
            {
                IDisposable check = null;
                check = CrossBleAdapter.Current.WhenStatusChanged().Subscribe(status =>
                {
                    if (status == AdapterStatus.PoweredOn)
                    {
                        check.Dispose();
                        StartScan();
                    }
                });
            }
        }

        private void StartScan()
        {
            _scanner.StartScanning(device =>
            {
                if (DeviceAlreadyFound(device))
                    return false;

                var model = new MetaWearModel(device);
                Devices.Add(model);

                return false;
            });
        }

        public async void TapItem(MetaWearModel model)
        {
            _scanner.StopScanning();

            await Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(new ConnectedDevicePage(MetaWear.NetStandard.Application.GetMetaWearBoard(model.Device)));
        }

        private bool DeviceAlreadyFound(MWDevice device)
        {
            return Devices.Any(x => x.Id == device.Uuid.ToString("N"));
        }
    }
}
