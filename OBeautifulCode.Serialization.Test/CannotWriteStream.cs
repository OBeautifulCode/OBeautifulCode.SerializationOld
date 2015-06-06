// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CannotWriteStream.cs" company="OBeautifulCode">
//   Copyright 2014 OBeautifulCode
// </copyright>
// <summary>
//   Stream that cannot be written to.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Serialization.Test
{
    using System;
    using System.IO;

    /// <summary>
    /// Stream that cannot be written to.
    /// </summary>
    [Serializable]
    public class CannotWriteStream : MemoryStream
    {
        /// <summary>
        /// Returns a value indicating whether the stream can be read.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }
        
        /// <summary>
        /// Writes to the stream.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Writes a byte.
        /// </summary>
        /// <param name="value">The byte to write.</param>
        public override void WriteByte(byte value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Begins writing to stream.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Always throws <see cref="NotSupportedException"/>
        /// </returns>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }
    }
}