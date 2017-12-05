﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace MbientLab.MetaWear.Platform {
    /// <summary>
    /// IO operations used by the API, must be implemented by the target platform
    /// </summary>
    public interface ILibraryIO {
        /// <summary>
        /// Save the data to the local device
        /// </summary>
        /// <param name="key">Key value identifying the data</param>
        /// <param name="data">Data to save</param>
        /// <returns>Null</returns>
        Task LocalSaveAsync(string key, byte[] data);
        /// <summary>
        /// Retrieves locally saved data from the host device
        /// </summary>
        /// <param name="key">Key value identifying the data</param>
        /// <returns>Stream to read the data</returns>
        Task<Stream> LocalLoadAsync(string key);

        /// <summary>
        /// Outputs a warn level message to the logger
        /// </summary>
        /// <param name="tag">Value identifying the message</param>
        /// <param name="message">Message to log</param>
        /// <param name="e">Additional information to provide to the logger</param>
        void LogWarn(String tag, String message, Exception e);
    }
}
