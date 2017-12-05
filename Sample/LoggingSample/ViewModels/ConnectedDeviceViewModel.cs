using MbientLab.MetaWear;
using MbientLab.MetaWear.Core;
using MbientLab.MetaWear.Data;
using MbientLab.MetaWear.Sensor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace LoggingSample.ViewModels
{
    public class ConnectedDeviceViewModel : BaseViewModel
    {
        private IMetaWearBoard _board;

        private string _status;
        private bool _isInitialized;

        private ICommand _onStartLogging;

        public string Status
        {
            get => _status;
            private set => SetField(ref _status, value);
        }

        public ICommand OnStartLogging
        {
            get => _onStartLogging ?? (_onStartLogging = new Command(async () =>
            {
                if (!_isInitialized)
                    return;

                try
                {
                    await StartLogging();
                }
                catch (Exception ex)
                {
                    Status = $"Error occured: {ex.Message}!";
                }
            }));
        }

        public ConnectedDeviceViewModel()
            : base()
        {

        }

        public async Task SetBoard(IMetaWearBoard board)
        {
            _board = board;

            await _board.InitializeAsync((value, text) =>
            {
                Status = $"Progress: {value} with text {text}.";
            });

            Status = "Connection established.";
            _isInitialized = true;
        }

        private async Task StartLogging()
        {
            var logging = _board.GetModule<ILogging>();
            if (logging == null)
            {
                Status = "Logging module not found!";
                return;
            }

            var gyro = _board.GetModule<IGyroBmi160>();
            if (gyro == null)
            {
                Status = "Gyro module not found!";
                return;
            }

            gyro.Configure(MbientLab.MetaWear.Sensor.GyroBmi160.OutputDataRate._50Hz, MbientLab.MetaWear.Sensor.GyroBmi160.DataRange._1000dps);

            // Code that fails on Android, not on iOS
            await gyro.AngularVelocity.AddRouteAsync(source =>
            {
                source.Log(data =>
                {
                    var angularVelocity = data.Value<AngularVelocity>();

                    Status = angularVelocity.ToString();
                });
            });

            logging.Start(true);

            gyro.AngularVelocity.Start();
            gyro.Start();

            await Task.Delay(TimeSpan.FromSeconds(8));

            logging.Stop();

            Status = "Download started.";

            await logging.DownloadAsync(100, (entries, totalEntries) =>
            {
                Status = $"Progress download = {entries}/{totalEntries}.";
            });
            Status = "Download completed.";

            logging.ClearEntries();
        }
    }
}
