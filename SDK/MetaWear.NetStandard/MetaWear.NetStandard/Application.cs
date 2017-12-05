﻿using MbientLab.MetaWear;
using MbientLab.MetaWear.Impl;
using MbientLab.MetaWear.Platform;
using Plugin.BluetoothLE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace MetaWear.NetStandard
{
    public class Application {
        private class IO : ILibraryIO {

            private ISettings AppSettings => CrossSettings.Current;
            private string macAddr;

            public IO(ulong macAddr) {
                this.macAddr = macAddr.ToString("X");
            }

            public IO(string name)
            {
                this.macAddr = name;
            }

            public Task<Stream> LocalLoadAsync(string key) {
                Stream str = null;
                //String res = AppSettings.GetValueOrDefault(key, null, macAddr);
                //if (res != null)
                //{
                //    byte[] byteArray = Convert.FromBase64String(res);
                //    str = new MemoryStream(byteArray);
                //}
                return Task.FromResult(str);
            }

            public void LogWarn(string tag, string message, Exception e) {
                Console.WriteLine(string.Format("{0}: {1}\r\n{2}", tag, message, e.StackTrace));
            }

            public Task LocalSaveAsync(string key, byte[] data) {
                String s = Convert.ToBase64String(data);
                //AppSettings.AddOrUpdateValue(key, s, macAddr);
                return Task.CompletedTask;
            }
        }

        private static Dictionary<String, MetaWearBoard> btleDevices = new Dictionary<String, MetaWearBoard>();

        /// <summary>
        /// Instantiates an <see cref="IMetaWearBoard"/> object corresponding to the BluetoothLE device
        /// </summary>
        /// <param name="device">BluetoothLE device object corresponding to the target MetaWear board</param>
        /// <returns><see cref="IMetaWearBoard"/> object</returns>
        public static IMetaWearBoard GetMetaWearBoard(MWDevice device) {
            if (btleDevices.TryGetValue(device.Name, out var board)) {
                return board;
            }

            board = new MetaWearBoard(new BLEBridge(device), new IO(device.Name));
            btleDevices.Add(device.Name, board);
            return board;
        }
        /// <summary>
        /// Removes the <see cref="IMetaWearBoard"/> object corresponding to the BluetoothLE device
        /// </summary>
        /// <param name="device">BluetoothLE device object corresponding to the target MetaWear board</param>
        public static void RemoveMetaWearBoard(IDevice device) {
            btleDevices.Remove(device.Name);
        }
        /// <summary>
        /// Clears cached information specific to the BluetoothLE device
        /// </summary>
        /// <param name="device">BluetoothLE device to clear</param>
        /// <returns>Null task</returns>
        public static Task ClearDeviceCacheAsync(IDevice device) {
            var macAddr = device.Name;
            CrossSettings.Current.Clear(macAddr);
            return Task.CompletedTask;
        }
    }
}
