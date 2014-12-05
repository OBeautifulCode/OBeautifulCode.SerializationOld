// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CannotSeekStream.cs" company="OBeautifulCode">
//   Copyright 2014 OBeautifulCode
// </copyright>
// <summary>
//   Stream that cannot seek.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Libs.Serialization.Test
{
    using System;
    using System.IO;

    /// <summary>
    /// Stream that cannot seek.
    /// </summary>
    [Serializable]
    public class CannotSeekStream : MemoryStream
    {
        #region Fields (Private)

        #endregion

        #region Constructors

        #endregion

        #region Properties

        /// <summary>
        /// Returns a value indicating whether the stream can be read.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the position within the stream.
        /// </summary>
        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }

            set
            {
                throw new NotSupportedException();
            }
        }
       
        #endregion

        #region Public Methods

        /// <summary>
        /// Seeks to a position within the stream.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <param name="loc">The seek origin.</param>
        /// <returns>
        /// Always throws <see cref="NotSupportedException"/>
        /// </returns>
        public override long Seek(long offset, SeekOrigin loc)
        {
            throw new NotSupportedException();
        }
        
        #endregion

        #region Internal Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Private Methods

        #endregion
    }
}