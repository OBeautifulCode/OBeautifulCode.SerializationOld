// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CannotReadStream.cs" company="OBeautifulCode">
//   Copyright 2015 OBeautifulCode
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Serialization.Test
{
    using System;
    using System.IO;

    /// <summary>
    /// Stream that cannot be read.
    /// </summary>
    [Serializable]
    public class CannotReadStream : MemoryStream
    {
        /// <summary>
        /// Returns a value indicating whether the stream can be read.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Reads from the stream.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns>
        /// Always throws <see cref="NotSupportedException"/>
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Reads a byte.
        /// </summary>
        /// <returns>
        /// Always throws <see cref="NotSupportedException"/>
        /// </returns>
        public override int ReadByte()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Begins reading from stream.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="state">The state.</param>
        /// <returns>
        /// Always throws <see cref="NotSupportedException"/>
        /// </returns>
        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }
    }
}