﻿using MbientLab.MetaWear.Core;
using MbientLab.MetaWear.Peripheral;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MbientLab.MetaWear.Test {
    [TestFixture]
    class IBeaconTest : UnitTestBase {
        private IIBeacon ibeacon;

        public IBeaconTest() : base(typeof(ISwitch), typeof(IIBeacon), typeof(IDataProcessor)) { }

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            ibeacon = metawear.GetModule<IIBeacon>();
        }

        [Test]
        public async Task SetMajorFeedbackAsync() {
            byte[][] expected = {
                new byte[] {0x09, 0x02, 0x01, 0x01, 0xff, 0x00, 0x02, 0x13},
                new byte[] {0x0a, 0x02, 0x09, 0x03, 0x00, 0x07, 0x03, 0x02, 0x09, 0x00},
                new byte[] {0x0a, 0x03, 0x00, 0x00},
                new byte[] {0x07, 0x01, 0x01}
            };

            var iswitch = metawear.GetModule<ISwitch>();
            await iswitch.State.AddRouteAsync(source => source.Count().React(token => ibeacon.SetMajor(token)));
            ibeacon.Enable();

            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }

        [Test]
        public void SetMinor() {
            byte[][] expected = {
                new byte[] {0x07, 0x04, 0x1d, 0x1d}
            };

            ibeacon.Configure(minor: 7453);
            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }

        [Test]
        public void SetMajor() {
            byte[][] expected = {
                new byte[] { 0x07, 0x03, 0x4e, 0x00 }
            };

            ibeacon.Configure(major: 78);
            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }

        [Test]
        public void SetPeriod() {
            byte[][] expected = {
                new byte[] { 0x07, 0x07, 0xb3, 0x3a }
            };

            ibeacon.Configure(period: 15027);
            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }

        [Test]
        public void SetRxPower() {
            byte[][] expected = {
                new byte[] { 0x07, 0x05, 0xc9 }
            };

            ibeacon.Configure(rxPower: -55);
            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }

        [Test]
        public void SetTxPower() {
            byte[][] expected = {
                new byte[] { 0x07, 0x06, 0xf4 }
            };

            ibeacon.Configure(txPower: -12);
            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }

        [Test]
        public void SetUuid() {
            byte[][] expected = {
                new byte[] { 0x07, 0x02, 0x5a, 0xe7, 0xba, 0xfb, 0x4c, 0x46, 0xdd, 0xd9, 0x95, 0x91, 0xcb, 0x85, 0x06, 0x90, 0x6a, 0x32 }
            };

            ibeacon.Configure(uuid: new System.Guid("326a9006-85cb-9195-d9dd-464cfbbae75a"));
            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }
    }
}
