using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.Text;

namespace MetaWear.NetStandard
{
    public class MWScanner
    {
        private Dictionary<Guid, MWDevice> seenDevices = new Dictionary<Guid, MWDevice>();
        private IDisposable scan;


        public void ClearScanResults()
        {
            StopScanning();
            seenDevices.Clear();
        }

        public void AddDevice(MWDevice device) { seenDevices.Add(device.Uuid, device); }

        public void StartScanning(Func<MWDevice, bool> whenFound)
        {
            StopScanning();
            
            CrossBleAdapter.Current.WhenStatusChanged().Subscribe(status =>
            {
                if (status == AdapterStatus.PoweredOn)
                {
                    scan = CrossBleAdapter.Current.ScanOrListen().Subscribe(scanResult =>
                    {
                        var devGuid = scanResult.Device.Uuid;
                        if (!seenDevices.ContainsKey(devGuid))
                        {
                            // TODO: Verify this is actually a MetaWear device
                            //var serviceGuid = config.ServiceUuids.First();
                            //var service = await scanResult.Device.GetKnownService(serviceGuid).FirstOrDefaultAsync();
                            if (scanResult.Device.Name == "MetaWear")
                            {
                                MWDevice mwdevice = new MWDevice(scanResult.Device);
                                seenDevices.Add(devGuid, mwdevice);
                                if (whenFound(mwdevice))
                                    StopScanning();
                            }
                        }
                    });
                }
            });
        }

        public void StopScanning()
        {
            scan?.Dispose();
            scan = null;
        }
    }
}
