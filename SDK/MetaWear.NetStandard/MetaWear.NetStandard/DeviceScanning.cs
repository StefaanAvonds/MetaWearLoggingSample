using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Text;

namespace MetaWear.NetStandard
{
    class DeviceScanning
    {
        private Dictionary<Guid, MWDevice> seenDevices = new Dictionary<Guid, MWDevice>();

        private IDisposable scan;

        void StartScanning(Func<MWDevice, bool> whenFound)
        {
            StopScanning();
            scan = CrossBleAdapter.Current.ScanOrListen().Subscribe(scanResult =>
            {
                var name = scanResult.Device.Name;
                var devGuid = scanResult.Device.Uuid;
                if (!seenDevices.ContainsKey(devGuid))
                //if (!seenDevices.Contains(scanResult.Device))
                //if (config.ServiceUuids.Aggregate(true, (acc, e) => acc & scanResult.AdvertisementData.ServiceUuids.Contains(e)))
                {
                    // TODO: Verify this is actually a MetaWear device
                    var uuids = scanResult.AdvertisementData.ServiceUuids;
                    foreach (Guid uuid in uuids)
                    {
                        var st = uuid.ToString();
                    }
                    var s = scanResult.AdvertisementData.ToString();
                    //var serviceGuid = config.ServiceUuids.First();
                    //var service = await scanResult.Device.GetKnownService(serviceGuid).FirstOrDefaultAsync();
                    //if (service != null)
                    {
                        MWDevice mwdevice = new MWDevice(scanResult.Device);
                        seenDevices.Add(devGuid, mwdevice);
                        if (whenFound(mwdevice))
                            StopScanning();
                    }
                }
            });
        }

        private void StopScanning()
        {
            scan?.Dispose();
            scan = null;
        }
    }
}
