﻿using MbientLab.MetaWear.Core;
using MbientLab.MetaWear.Core.DataProcessor;
using MbientLab.MetaWear.Data;
using MbientLab.MetaWear.Impl;
using MbientLab.MetaWear.Peripheral;
using MbientLab.MetaWear.Peripheral.SerialPassthrough;
using MbientLab.MetaWear.Sensor;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace MbientLab.MetaWear.Test {
    [TestFixture]
    class DeserializeTest : UnitTestBase {
        public DeserializeTest() : base() {

        }

        [SetUp]
        public override void SetUp() {
            metawear = new MetaWearBoard(platform, platform);
        }

        [Test]
        public async Task ScheduleAndRemoveAsync() {
            byte[][] expected = {
                new byte[] { 0x0c, 0x05, 0x0 },
                new byte[] { 0x0a, 0x04, 0x0 },
                new byte[] { 0x0a, 0x04, 0x1 }
            };

            platform.fileSuffix = "scheduled_task";
            await metawear.DeserializeAsync();
            await metawear.InitializeAsync();
            metawear.LookupScheduledTask(0).Remove();

            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }

        [Test]
        public async Task DisconnectEventAsync() {
            byte[][] expected = {
                new byte[] { 0x0a, 0x04, 0x0 },
                new byte[] { 0x0a, 0x04, 0x1 }
            };

            platform.fileSuffix = "observer_rev2";
            await metawear.DeserializeAsync();
            await metawear.InitializeAsync();
            var observer = metawear.LookupObserver(0);

            observer.Remove();

            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }

        [Test]
        public async Task GpioAnalogAsync() {
            byte[][] expected = {
                new byte[] {0x05, 0x86, 0x02, 0xff, 0xff, 0x00, 0xff},
                new byte[] {0x05, 0x87, 0x03, 0xff, 0xff, 0x00, 0xff}
            };

            platform.fileSuffix = "gpio_analog";
            await metawear.DeserializeAsync();
            await metawear.InitializeAsync();

            var gpio = metawear.GetModule<IGpio>();

            gpio.Pins[2].AbsoluteReference.Read();
            gpio.Pins[3].Adc.Read();
            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }

        [Test]
        public async Task ReadWhoAmIDataAsync() {
            byte[][] expected = { new byte[] { 0x0d, 0x81, 0x1c, 0x0d, 0x0a, 0x01 } };

            platform.fileSuffix = "i2c_stream";
            await metawear.DeserializeAsync();
            await metawear.InitializeAsync();

            var i2c = metawear.GetModule<ISerialPassthrough>().I2C(0xa, 0x1);
            i2c.Read(0x1c, 0xd);
            
            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }

        [Test]
        public async Task GpioFeedbackAsync() {
            byte[][] expected = {
                new byte[] {0x09, 0x06, 0x00},
                new byte[] {0x09, 0x06, 0x01},
                new byte[] {0x09, 0x06, 0x02},
                new byte[] {0x09, 0x06, 0x03},
                new byte[] {0x09, 0x06, 0x04},
                new byte[] {0x09, 0x06, 0x05},
                new byte[] {0x09, 0x06, 0x06},
                new byte[] {0x09, 0x06, 0x07},
                new byte[] {0x0a, 0x04, 0x00},
                new byte[] {0x0a, 0x04, 0x01},
                new byte[] {0x0a, 0x04, 0x02},
                new byte[] {0x0a, 0x04, 0x03},
                new byte[] {0x0a, 0x04, 0x04},
                new byte[] {0x0a, 0x04, 0x05},
                new byte[] {0x0a, 0x04, 0x06}
            };

            platform.fileSuffix = "gpio_feedback";
            await metawear.DeserializeAsync();
            await metawear.InitializeAsync();
            metawear.LookupRoute(0).Remove();

            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }

        [Test]
        public async Task EditMultiCompAsync() {
            byte[][] expected = {
                new byte[] {0x09, 0x05, 0x00, 0x06, 0x12, 0x80, 0x00, 0x00, 0x01}
            };

            platform.fileSuffix = "multi_comparator";
            await metawear.DeserializeAsync();
            await metawear.InitializeAsync();

            metawear.GetModule<IDataProcessor>().Edit<IComparatorEditor>("multi_comp").Modify(Builder.Comparison.Lt, 128, 256);

            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }

        [Test]
        public async Task Bmi160SpiRead() {
            byte[][] expected = { new byte[] { 0x0d, 0x82, 0x0a, 0x00, 0x0b, 0x07, 0x76, 0xe4, 0xda } };

            platform.fileSuffix = "spi_stream";
            await metawear.DeserializeAsync();
            await metawear.InitializeAsync();

            var spi = metawear.GetModule<ISerialPassthrough>().SPI(0xe, 0x5);
            spi.Read(10, 0, 11, 7, 3, SpiFrequency._8_MHz, lsbFirst: false, data: new byte[] { 0xda });

            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }

        [Test]
        public async Task ReadTemperatureAsync() {
            byte[][] expected = new byte[][] {
                new byte[] { 0x4, 0x81, 0x3 }
            };

            platform.fileSuffix = "temperature";
            await metawear.DeserializeAsync();
            await metawear.InitializeAsync();

            var sensor = metawear.GetModule<ITemperature>().Sensors[0x3];
            sensor.Read();

            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }

        [Test]
        public async Task PreserveObjectReferences() {
            // Tests that object references are preserved in serialization
            platform.fileSuffix = "bmi160_acc_route";
            await metawear.DeserializeAsync();
            await metawear.InitializeAsync();

            Acceleration expected = new Acceleration(
                    BitConverter.ToSingle(new byte[] { 0x00, 0xa8, 0xef, 0xbf }, 0),
                    BitConverter.ToSingle(new byte[] { 0x00, 0xd8, 0x3a, 0xc0 }, 0),
                    BitConverter.ToSingle(new byte[] { 0x00, 0x58, 0xbf, 0xbf }, 0)), 
                actual = null;
            metawear.GetModule<IAccelerometer>().Configure(range: 16f);
            metawear.LookupRoute(0).AttachSubscriber(0, data => actual = data.Value<Acceleration>());
            platform.sendMockResponse(new byte[] { 0x03, 0x04, 0x16, 0xc4, 0x94, 0xa2, 0x2a, 0xd0 });

            Assert.That(actual, Is.Not.EqualTo(expected));
        }

        [Test]
        public async Task ReadBufferStateAsync() {
            byte[][] expected = {
                new byte[] {0x09, 0x84, 0x02}
            };

            platform.fileSuffix = "activity_buffer";
            await metawear.DeserializeAsync();

            metawear.GetModule<IDataProcessor>().State("rms_buffer").Read();

            Assert.That(platform.GetCommands(), Is.EqualTo(expected));
        }
    }
}
