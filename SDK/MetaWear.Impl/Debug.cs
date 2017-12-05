﻿using MbientLab.MetaWear.Core;
using static MbientLab.MetaWear.Impl.Module;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MbientLab.MetaWear.Impl {
    [DataContract]
    class Debug : ModuleImplBase, IDebug {
        public Debug(IModuleBoardBridge bridge) : base(bridge) {
        }

        public Task<bool> DisconnectAsync() {
            TaskCompletionSource<bool> taskSource = new TaskCompletionSource<bool>();
            taskSource.SetCanceled();

            Event eventModule = bridge.GetModule<Event>();

            bridge.sendCommand(new byte[] { (byte) DEBUG, 0x6 });
            return eventModule.getEventConfig() != null ? taskSource.Task : bridge.remoteDisconnect();
        }

        public Task<bool> JumpToBootloaderAsync() {
            TaskCompletionSource<bool> taskSource = new TaskCompletionSource<bool>();
            taskSource.SetCanceled();

            Event eventModule = bridge.GetModule<Event>();

            bridge.sendCommand(new byte[] { (byte)DEBUG, 0x2 });
            return eventModule.getEventConfig() != null ? taskSource.Task : bridge.remoteDisconnect();
        }

        public void ResetAfterGc() {
            bridge.sendCommand(new byte[] { (byte) DEBUG, 0x5 });
        }

        public Task<bool> ResetAsync() {
            TaskCompletionSource<bool> taskSource = new TaskCompletionSource<bool>();
            taskSource.SetCanceled();

            Event eventModule = bridge.GetModule<Event>();

            bridge.sendCommand(new byte[] { (byte)DEBUG, 0x1 });
            return eventModule.getEventConfig() != null ? taskSource.Task : bridge.remoteDisconnect();
        }
    }
}
