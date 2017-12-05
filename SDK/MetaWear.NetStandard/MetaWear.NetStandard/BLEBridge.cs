using MbientLab.MetaWear.Platform;
using System;
using System.Threading.Tasks;
using Plugin.BluetoothLE;
using ReactiveUI;

using System.Reactive.Linq;
using System.Collections.Generic;
using System.Linq;

namespace MetaWear.NetStandard
{
    public class BLEBridge : IBluetoothLeGatt
    {
        private IDevice device;
        private object connection;

        //private Dictionary<Guid, IGattCharacteristic> characteristics;

        public BLEBridge(MWDevice mwdevice)
        {
            device = mwdevice.device;

            device.WhenStatusChanged().Subscribe(status =>
            {
                switch (status)
                {
                    case ConnectionStatus.Connecting:
                        System.Diagnostics.Debug.WriteLine("MetaWear connecting!");
                        break;
                    case ConnectionStatus.Connected:
                        System.Diagnostics.Debug.WriteLine("MetaWear connected!");
                        break;
                    case ConnectionStatus.Disconnecting:
                        System.Diagnostics.Debug.WriteLine("MetaWear disconnecting!");
                        break;
                    case ConnectionStatus.Disconnected:
                        System.Diagnostics.Debug.WriteLine("MetaWear disconnected!");
                        _notification?.Dispose();
                        _notification = null;
                        if (dcTaskSource != null)
                        {
                            dcTaskSource.SetResult(true);
                            if (OnDisconnect != null) OnDisconnect(false);
                        }
                        else
                        {
                            if (OnDisconnect != null) OnDisconnect(true);
                        }
                        break;
                };
            });

            Connect();
            //connection = device.Connect().Wait();

            // Cache all characteristics?
            //characteristics = new Dictionary<Guid, IGattCharacteristic>();
            //device.WhenAnyCharacteristicDiscovered().Subscribe(ch =>
            //{
            //    characteristics.Add(ch.Uuid, ch);
            //});
        }

        private async void Connect()
        {
            await device.Connect();
        }

        public ulong BluetoothAddress => 0; // TODO

        public Action<bool> OnDisconnect { get; set; }

        private IList<IGattCharacteristic> _advertisementCharacteristics;

        public void GetAdvertisementCharacteristics()
        {
            _advertisementCharacteristics = new List<IGattCharacteristic>();
            
            device.WhenAnyCharacteristicDiscovered().Subscribe(characteristic =>
            {
                if (characteristic.Service.Uuid != MbientLab.MetaWear.Constants.METAWEAR_GATT_SERVICE) return;

                _advertisementCharacteristics.Add(characteristic);
            });
        }

        IDisposable _notification;
        public async Task EnableNotificationsAsync(Tuple<Guid, Guid> gattChar, Action<byte[]> handler)
        {
            var characteristic = await GetGattCharacteristicAsync(gattChar);
            if (characteristic != null)
            {
                var res = await characteristic.EnableNotifications();
                if (res)
                    _notification = characteristic.WhenNotificationReceived().Subscribe(x => handler(x.Data));
            }
        }

        private async Task<IGattCharacteristic> GetGattCharacteristicAsync(Tuple<Guid, Guid> gattChar)
        {
            IGattCharacteristic characteristic = null;
            
            if (gattChar.Item1 == MbientLab.MetaWear.Constants.METAWEAR_GATT_SERVICE && _advertisementCharacteristics != null)
            {
                characteristic = _advertisementCharacteristics.FirstOrDefault(x => x.Uuid == gattChar.Item2);
            }
            else
            {
                characteristic = await device.GetKnownCharacteristics(gattChar.Item1, gattChar.Item2).FirstAsync();
            }

            return characteristic;
        }

        public async Task<byte[]> ReadCharacteristicAsync(Tuple<Guid, Guid> gattChar)
        {
            var ch = await GetGattCharacteristicAsync(gattChar);
            if (ch != null)
            {
                var res = await ch.Read();
                return res.Data;
            }
            return new byte[0];
        }

        private TaskCompletionSource<bool> dcTaskSource;
        public Task<bool> RemoteDisconnectAsync()
        {
            // Copied from Win10 implementation
            dcTaskSource = new TaskCompletionSource<bool>();
            return dcTaskSource.Task;
        }

        public async Task<bool> ServiceExistsAsync(Guid serviceGuid)
        {
            var service = await device.GetKnownService(serviceGuid).FirstOrDefaultAsync();
            return service != null;
        }

        public async Task WriteCharacteristicAsync(Tuple<Guid, Guid> gattChar, GattCharWriteType writeType, byte[] value)
        {
            var ch = await GetGattCharacteristicAsync(gattChar);
            if (writeType == GattCharWriteType.WRITE_WITH_RESPONSE)
            {
                CharacteristicResult resp = await ch.Write(value);
                //if (resp.S)
                // Note - whaddayou do with a failed write?
            }
            else
                ch.WriteWithoutResponse(value);
        }
    }
}
