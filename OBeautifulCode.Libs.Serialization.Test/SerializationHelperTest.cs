// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationHelperTest.cs" company="OBeautifulCode">
//   Copyright 2014 OBeautifulCode
// </copyright>
// <summary>
//   Tests the SerializationHelper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Libs.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Xml.Serialization;

    using Xunit;

    /// <summary>
    /// Tests the SerializationHelper class.
    /// </summary>
    public class SerializationHelperTest
    {
        #region Fields (Private)

        #endregion

        #region Constructors

        #endregion

        #region Properties

        #endregion

        #region Public Methods
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToStreamAsBinary_ObjectToSerializeIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var ms = new MemoryStream();

            // Act, Assert
            Assert.Throws<ArgumentNullException>(() => SerializationHelper.SerializeToStreamAsBinary(null, ms));
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToStreamAsBinary_StreamIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var objToSerialize = new Exception();

            // Act, Assert
            Assert.Throws<ArgumentNullException>(() => objToSerialize.SerializeToStreamAsBinary(null));
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToStreamAsBinary_ObjectCannotBeSerialized_ThrowsSerializationException()
        {
            // Arrange
            var objToSerialize = new CannotBeSerialized();
            var ms = new MemoryStream();

            // Act, Assert
            Assert.Throws<SerializationException>(() => objToSerialize.SerializeToStreamAsBinary(ms));
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToStreamAsBinary_StreamCannotBeWrittenTo_ThrowsArgumentException()
        {
            // Arrange
            var objToSerialize = new Exception();
            var ms = new CannotWriteStream();

            // Act, Assert
            Assert.Throws<ArgumentException>(() => objToSerialize.SerializeToStreamAsBinary(ms));
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToStreamAsBinary_SeekToPreWriteIsTrueAndStreamIsNotSeekable_ThrowsArgumentException()
        {
            // Arrange
            var objToSerialize = new Exception();
            var ms = new CannotSeekStream();

            // Act, Assert
            Assert.Throws<ArgumentException>(() => objToSerialize.SerializeToStreamAsBinary(ms, true));
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToStreamAsBinary_SerializeString_ProperlySerializesStringToBinary()
        {
            // Arrange
            const string ObjToSerialize = "my test string";
            var binFormat = new BinaryFormatter();
            object actual;

            // Act
            // de-serialize to determine if object was properly serialized.  is there a better way?
            using (var ms = new MemoryStream())
            {
                ObjToSerialize.SerializeToStreamAsBinary(ms, true);
                ms.Seek(0, SeekOrigin.Begin);
                actual = binFormat.Deserialize(ms);
            }

            // Assert
            Assert.IsType<string>(actual);
            Assert.Equal(ObjToSerialize, (string)actual);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToStreamAsBinary_SerializeDictionary_ProperlySerializesDictionaryToBinary()
        {
            // Arrange
            var objToSerialize = new Dictionary<string, int>
                                         {
                                             { "firstkey", 123 },
                                             { "secondkey", 456 }
                                         };
            var binFormat = new BinaryFormatter();
            object actual;

            // Act
            // de-serialize to determine if object was properly serialized.  is there a better way?
            using (var ms = new MemoryStream())
            {
                objToSerialize.SerializeToStreamAsBinary(ms);
                ms.Seek(0, SeekOrigin.Begin);
                actual = binFormat.Deserialize(ms);
            }

            // Assert
            Assert.IsType<Dictionary<string, int>>(actual);
            var actualAsDict = (Dictionary<string, int>)actual;
            Assert.Equal(2, actualAsDict.Keys.Count);
            Assert.Contains("firstkey", actualAsDict.Keys);
            Assert.Contains("secondkey", actualAsDict.Keys);
            Assert.Equal(123, actualAsDict["firstkey"]);
            Assert.Equal(456, actualAsDict["secondkey"]);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToStreamAsBinary_SeekToPreWriteIsTrue_ReturnsStreamToPreWritePosition()
        {
            // Arrange
            var objToSerialize = new Exception();
            var ms1 = new MemoryStream();
            var ms2 = new MemoryStream();
            var random = new Random();
            int numberOfRandomBytes = random.Next(50, 10000);
            var bytes = new byte[numberOfRandomBytes];
            random.NextBytes(bytes);
            ms2.Write(bytes, 0, bytes.Length);

            // Act
            objToSerialize.SerializeToStreamAsBinary(ms1, true);
            objToSerialize.SerializeToStreamAsBinary(ms2, true);

            // Assert
            Assert.Equal(0, ms1.Position);
            Assert.Equal(bytes.Length, ms2.Position);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToStreamAsBinary_SeekToPreWriteIsFalse_ReturnsStreamAtPostWritePosition()
        {
            // Arrange
            var objToSerialize = new Exception();
            var ms1 = new MemoryStream();
            var ms2 = new MemoryStream();
            var random = new Random();
            int numberOfRandomBytes = random.Next(50, 10000);
            var bytes = new byte[numberOfRandomBytes];
            random.NextBytes(bytes);
            ms2.Write(bytes, 0, bytes.Length);

            // Act
            // ReSharper disable RedundantArgumentDefaultValue
            objToSerialize.SerializeToStreamAsBinary(ms1, false);
            objToSerialize.SerializeToStreamAsBinary(ms2, false);
            // ReSharper restore RedundantArgumentDefaultValue

            // Assert
            Assert.Equal(ms1.Length, ms1.Position);
            Assert.Equal(bytes.Length + ms1.Length, ms2.Position);

            // Cleanup
            ms1.Dispose();
            ms2.Dispose();
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToStreamAsBinary_StreamIsNotSeekableAndSeekToPreWriteIsFalse_DoesNotThrow()
        {
            // Arrange
            var objToSerialize = new Exception();
            var ms1 = new CannotSeekStream();
            var ms2 = new CannotSeekStream();
            var random = new Random();
            int numberOfRandomBytes = random.Next(50, 10000);
            var bytes = new byte[numberOfRandomBytes];
            random.NextBytes(bytes);
            ms2.Write(bytes, 0, bytes.Length);

            // Act, Assert
            // ReSharper disable RedundantArgumentDefaultValue
            Assert.DoesNotThrow(() => objToSerialize.SerializeToStreamAsBinary(ms1, false));
            Assert.DoesNotThrow(() => objToSerialize.SerializeToStreamAsBinary(ms2, false));
            // ReSharper restore RedundantArgumentDefaultValue

            // Cleanup
            ms1.Dispose();
            ms2.Dispose();
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToMemoryStreamAsBinary_ObjectToSerializeIsNull_ThrowsArgumentNullException()
        {
            // Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => SerializationHelper.SerializeToMemoryStreamAsBinary(null));
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToMemoryStreamAsBinary_ObjectCannotBeSerialized_ThrowsSerializationException()
        {
            // Arrange
            var objToSerialize = new CannotBeSerialized();

            // Act, Assert
            Assert.Throws<SerializationException>(() => objToSerialize.SerializeToMemoryStreamAsBinary());
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToMemoryStreamAsBinary_SerializeString_ReturnsMemoryStreamWithStringProperlySerializedToBinary()
        {
            // Arrange
            const string ObjToSerialize = "my test string";
            var binFormat = new BinaryFormatter();
            object actual;

            // Act
            // de-serialize to determine if object was properly serialized.  is there a better way?
            using (var ms = ObjToSerialize.SerializeToMemoryStreamAsBinary())
            {
                ms.Seek(0, SeekOrigin.Begin);
                actual = binFormat.Deserialize(ms);
            }

            // Assert
            Assert.IsType<string>(actual);
            Assert.Equal(ObjToSerialize, (string)actual);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToMemoryStreamAsBinary_SerializeDictionary_ReturnsMemoryStreamWithDictionaryProperlySerializedToBinary()
        {
            // Arrange
            var objToSerialize = new Dictionary<string, int>
                                         {
                                             { "firstkey", 123 },
                                             { "secondkey", 456 }
                                         };
            var binFormat = new BinaryFormatter();
            object actual;

            // Act
            // de-serialize to determine if object was properly serialized.  is there a better way?
            using (MemoryStream ms = objToSerialize.SerializeToMemoryStreamAsBinary())
            {
                ms.Seek(0, SeekOrigin.Begin);
                actual = binFormat.Deserialize(ms);
            }

            // Assert
            Assert.IsType<Dictionary<string, int>>(actual);
            var actualAsDict = (Dictionary<string, int>)actual;
            Assert.Equal(2, actualAsDict.Keys.Count);
            Assert.Contains("firstkey", actualAsDict.Keys);
            Assert.Contains("secondkey", actualAsDict.Keys);
            Assert.Equal(123, actualAsDict["firstkey"]);
            Assert.Equal(456, actualAsDict["secondkey"]);
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToMemoryStreamAsBinary_SeekToFirstPositionIsTrue_ReturnsMemoryStreamWithStringProperlySerializedToBinary()
        {
            // Arrange
            var objToSerialize = new Exception();

            // Act
            // ReSharper disable RedundantArgumentDefaultValue
            var ms = objToSerialize.SerializeToMemoryStreamAsBinary(true);
            // ReSharper restore RedundantArgumentDefaultValue

            // Assert            
            Assert.Equal(0, ms.Position);

            // Cleanup
            ms.Dispose();
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void SerializeToMemoryStreamAsBinary_SeekToFirstPositionIsFalse_ReturnsMemoryStreamWithStringProperlySerializedToBinary()
        {
            // Arrange
            var objToSerialize = new Exception();

            // Act
            var ms = objToSerialize.SerializeToMemoryStreamAsBinary(false);

            // Assert            
            Assert.NotEqual(0, ms.Position);
            Assert.Equal(ms.Length, ms.Position);

            // Cleanup
            ms.Dispose();
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void DeserializeFromBinary_StreamIsNull_ThrowsArgumentNullException()
        {
            // Arrange, Act, Assert
            Assert.Throws<ArgumentNullException>(() => SerializationHelper.DeserializeFromBinary<object>(null));
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void DeserializeFromBinary_StreamDoesNotSupportReading_ThrowsArgumentException()
        {
            // Arrange
            var exception = new Exception();
            var ms = new CannotReadStream();
            exception.SerializeToMemoryStreamAsBinary().CopyTo(ms);

            // Act, Assert
            Assert.Throws<ArgumentException>(() => ms.DeserializeFromBinary<object>());

            // Cleanup
            ms.Dispose();
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void DeserializeFromBinary_StreamSupportsSeekingButLengthIsZero_ThrowsArgumentException()
        {
            // Arrange
            var ms = new MemoryStream();

            // Act, Assert
            Assert.Throws<ArgumentException>(() => ms.DeserializeFromBinary<object>());

            // Cleanup
            ms.Dispose();
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void DeserializeFromBinary_SeekToFirstPositionIsTrueButStreamIsNotSeekable_ThrowsArgumentException()
        {
            // Arrange
            var exception = new Exception();
            var ms = new CannotSeekStream();
            exception.SerializeToMemoryStreamAsBinary().CopyTo(ms);

            // Act, Assert
            Assert.Throws<ArgumentException>(() => ms.DeserializeFromBinary<object>(true));

            // Cleanup
            ms.Dispose();
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void DeserializeFromBinary_ExpectedTypeOfDeserializedObjectDoesNotMatchActualType_ThrowsInvalidOperationException()
        {
            // Arrange
            var exception = new Exception();
            var ms = new MemoryStream();
            exception.SerializeToMemoryStreamAsBinary().CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);

            // Act, Assert
            Assert.Throws<InvalidOperationException>(() => ms.DeserializeFromBinary<string>());

            // Cleanup
            ms.Dispose();
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void DeserializeFromBinary_ObjectSerializedNotUsingTheBinaryFormatter_ThrowsSerializationException()
        {
            // Arrange
            var canSerialize = new KeyValuePair<string, string>();
            var ms = new MemoryStream();
            var xmlFormatter = new XmlSerializer(typeof(KeyValuePair<string, string>));
            xmlFormatter.Serialize(ms, canSerialize);
            ms.Seek(0, SeekOrigin.Begin);

            // Act, Assert
            Assert.Throws<SerializationException>(() => ms.DeserializeFromBinary<KeyValuePair<string, string>>());

            // Cleanup
            ms.Dispose();
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void DeserializeFromBinary_SerializedObjectIsDictionary_ReturnsSameDictionary()
        {
            // Arrange
            var objToSerialize = new Dictionary<string, int>
                                         {
                                             { "firstkey", 123 },
                                             { "secondkey", 456 }
                                         };
            MemoryStream ms = objToSerialize.SerializeToMemoryStreamAsBinary();

            // Act
            var deserializedObj = ms.DeserializeFromBinary<Dictionary<string, int>>();

            // Assert
            Assert.Equal(2, deserializedObj.Keys.Count);
            Assert.Contains("firstkey", deserializedObj.Keys);
            Assert.Contains("secondkey", deserializedObj.Keys);
            Assert.Equal(123, deserializedObj["firstkey"]);
            Assert.Equal(456, deserializedObj["secondkey"]);

            // Cleanup
            ms.Dispose();
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void DeserializeFromBinary_SeekToFirstPositionIsTrueAndStreamIsAtLastPosition_EnablesStreamToDeserialize()
        {
            // Arrange
            var objToSerialize = new Dictionary<string, int>
                                         {
                                             { "firstkey", 123 },
                                             { "secondkey", 456 }
                                         };
            MemoryStream streamAtLastPositon = objToSerialize.SerializeToMemoryStreamAsBinary(false);

            // Act
            var deserializedObj = streamAtLastPositon.DeserializeFromBinary<Dictionary<string, int>>(true);

            // Assert
            Assert.Equal(2, deserializedObj.Keys.Count);
            Assert.Contains("firstkey", deserializedObj.Keys);
            Assert.Contains("secondkey", deserializedObj.Keys);
            Assert.Equal(123, deserializedObj["firstkey"]);
            Assert.Equal(456, deserializedObj["secondkey"]);

            // Cleanup
            streamAtLastPositon.Dispose();
        }

        /// <summary>
        /// Test method.
        /// </summary>
        [Fact]
        public static void DeserializeFromBinary_SeekToFirstPositionIsFalseAndStreamIsAtLastPosition_ThrowsSerializationException()
        {
            // Arrange
            var objToSerialize = new Dictionary<string, int>
                                         {
                                             { "firstkey", 123 },
                                             { "secondkey", 456 }
                                         };
            MemoryStream streamAtLastPosition = objToSerialize.SerializeToMemoryStreamAsBinary(false);

            // Act, Assert
            // ReSharper disable RedundantArgumentDefaultValue
            Assert.Throws<SerializationException>(() => streamAtLastPosition.DeserializeFromBinary<Dictionary<string, int>>(false));
            // ReSharper restore RedundantArgumentDefaultValue

            // Cleanup
            streamAtLastPosition.Dispose();
        }

        // ReSharper restore InconsistentNaming
        #endregion

        #region Internal Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Private Methods

        #endregion
    }
}