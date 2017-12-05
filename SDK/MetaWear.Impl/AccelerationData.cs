﻿using MbientLab.MetaWear.Data;
using System;

namespace MbientLab.MetaWear.Impl {
    class AccelerationData : DataBase {
        public AccelerationData(IModuleBoardBridge bridge, DataTypeBase datatype, DateTime timestamp, byte[] bytes) : 
            base(bridge, datatype, timestamp, bytes) {
        }

        public override Type[] Types => new Type[] { typeof(Acceleration) };

        public override T Value<T>() {
            var type = typeof(T);

            if (type == typeof(Acceleration)) {
                return (T) Convert.ChangeType(new Acceleration(
                    BitConverter.ToInt16(bytes, 0) / Scale, 
                    BitConverter.ToInt16(bytes, 2) / Scale, 
                    BitConverter.ToInt16(bytes, 4) / Scale), type);
            }
            return base.Value<T>();
        }
    }
}
