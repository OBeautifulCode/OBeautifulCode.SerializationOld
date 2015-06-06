// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationHelper.cs" company="OBeautifulCode">
//   Copyright 2015 OBeautifulCode
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Serialization
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Security;

    using Conditions;

    /// <summary>
    /// Provides convenience methods related to serialization.
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// Serializes an object to a MemoryStream using the BinaryFormatter
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="seekToFirstPosition">
        /// Optional.  Determines whether the returned <see cref="MemoryStream"/> is at position 0.
        /// Default is true - it's more likely the consumer will want to evaluate over the written
        /// data since they are asking for the stream to be created by this method, unlike <see cref="SerializeToStreamAsBinary"/>
        /// </param>
        /// <returns>A memory stream that containing the serialized object, at position 0.</returns>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        /// <exception cref="SerializationException">
        /// An error has occurred during serialization, such as if an object 
        /// in the graph parameter is not marked as serializable or when
        /// the stream cannot be written to.
        /// </exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public static MemoryStream SerializeToMemoryStreamAsBinary(this object value, bool seekToFirstPosition = true)
        {
            var ms = new MemoryStream();
            try
            {
                SerializeToStreamAsBinary(value, ms, seekToFirstPosition);
                return ms;
            }
            catch
            {
                ms.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Serializes an object to a stream using the BinaryFormatter
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="seekToPreWrite">
        /// Optional.  Determines whether to seek to the position in the stream prior to when the write occurred.
        /// Default is false - it's equally likely that the consumer will want to keep writing to the stream,
        /// rather than evaluate over the written data.
        /// </param>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        /// <exception cref="ArgumentNullException">stream is null.</exception>
        /// <exception cref="ArgumentException">stream is not writable</exception>
        /// <exception cref="ArgumentException">seekToBeginningOfWrite is true but stream is not seekable.</exception>
        /// <exception cref="SerializationException">An error has occurred during serialization, such as if an object in the graph parameter is not marked as serializable.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public static void SerializeToStreamAsBinary(this object value, Stream stream, bool seekToPreWrite = false)
        {
            SerializeToStream(value, stream, new BinaryFormatter(), seekToPreWrite);
        }

        /// <summary>
        /// Deserializes an object that has been serialized to a stream in binary format.
        /// </summary>
        /// <typeparam name="T">Type of object stored in the <see cref="Stream"/>.</typeparam>
        /// <param name="value"><see cref="Stream"/> containing object to deserialize.</param>
        /// <param name="seekToFirstPosition">
        /// Optional.  Determines whether to seek to the first position before deserialization.
        /// Default is false - it's possible that the stream is not seekable.
        /// </param>
        /// <returns>Deserialized object as type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        /// <exception cref="ArgumentException">Stream does not support reading.</exception>
        /// <exception cref="ArgumentException">The serializationStream supports seeking, but its length is 0.</exception>
        /// <exception cref="ArgumentException">seekToFirstPosition is true but stream is not seekable.</exception>
        /// <exception cref="InvalidOperationException">Expected type of deserialized object does not match its actual type.</exception>
        /// <exception cref="SerializationException">
        /// The target type is a Decimal, but the value is out of range of the Decimal type.
        /// -or-
        /// The stream was not serialized in Binary
        /// -or-
        /// There was a issue with the deserialization.
        /// </exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        public static T DeserializeFromBinary<T>(this Stream value, bool seekToFirstPosition = false)
        {
            return Deserialize<T>(value, new BinaryFormatter(), seekToFirstPosition);
        }

        /// <summary>
        /// Serializes an object to a stream using a given formatter.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="formatter">The formatter to use.</param>
        /// <param name="seekToPreWrite">Determines whether to seek to the position in the stream prior to when the write occurred.</param>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        /// <exception cref="ArgumentNullException">stream is null.</exception>
        /// <exception cref="ArgumentException">stream is not writable</exception>
        /// <exception cref="ArgumentException">stream is not writable</exception>
        /// <exception cref="ArgumentNullException">formatter is null.</exception>
        /// <exception cref="SerializationException">An error has occurred during serialization, such as if an object in the graph parameter is not marked as serializable.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        private static void SerializeToStream(this object value, Stream stream, IFormatter formatter, bool seekToPreWrite)
        {
            // check parameters
            Condition.Requires(value, "value").IsNotNull();
            Condition.Requires(stream, "stream").IsNotNull();
            Condition.Requires(stream.CanWrite).IsTrue();
            Condition.Requires(formatter, "formatter").IsNotNull();
            if (seekToPreWrite && (!stream.CanSeek))
            {
                throw new ArgumentException("seekToPreWrite is true but stream is not seekable");
            }

            // serialize
            long startPosition = seekToPreWrite ? stream.Position : 0;
            formatter.Serialize(stream, value);
            if (seekToPreWrite)
            {
                stream.Seek(startPosition, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Deserializes an object that has been serialized to a stream.
        /// </summary>
        /// <typeparam name="T">Type of object stored in the <see cref="Stream"/>.</typeparam>
        /// <param name="value"><see cref="Stream"/> containing object to deserialize.</param>
        /// <param name="formatter">The formatter to use for deserialization.</param>
        /// <param name="seekToFirstPosition">Determines whether to seek to the first position before deserialization.</param>
        /// <returns>Deserialized object as type <typeparamref name="T"/>.</returns>
        /// <exception cref="ArgumentNullException">value is null.</exception>
        /// <exception cref="ArgumentException">Stream does not support reading.</exception>
        /// <exception cref="ArgumentException">The serializationStream supports seeking, but its length is 0.</exception>
        /// <exception cref="ArgumentNullException">formatter is null.</exception>
        /// <exception cref="ArgumentException">seekToFirstPosition is true but stream is not seekable.</exception>
        /// <exception cref="InvalidOperationException">Expected type of deserialized object does not match its actual type.</exception>
        /// <exception cref="SerializationException">
        /// The target type is a Decimal, but the value is out of range of the Decimal type.
        /// -or-
        /// The stream was not serialized in Binary
        /// -or-
        /// There was a issue with the deserialization.
        /// </exception>
        /// <exception cref="SecurityException">The caller does not have the required permission.</exception>
        private static T Deserialize<T>(this Stream value, IFormatter formatter, bool seekToFirstPosition)
        {
            // check parameters
            Condition.Requires(value, "value").IsNotNull();
            Condition.Requires(value.CanRead).IsTrue();
            if (value.CanSeek && (value.Length == 0))
            {
                throw new ArgumentException("The serializationStream supports seeking, but its length is 0.");
            }

            Condition.Requires(formatter, "formatter").IsNotNull();
            if (seekToFirstPosition && (!value.CanSeek))
            {
                throw new ArgumentException("seekToFirstPosition is true but stream is not seekable");
            }

            // deserialize
            if (seekToFirstPosition)
            {
                value.Seek(0, SeekOrigin.Begin);
            }

            try
            {
                return (T)formatter.Deserialize(value);
            }
            catch (InvalidCastException)
            {
                throw new InvalidOperationException("Expected type of deserialized object does not match its actual type.");
            }
        }
    }
}