using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc;
using DeepReader.Types.Fc.Crypto;
using DeepReader.Types.Helpers;
using Serilog;
using System.Buffers.Binary;
using System.Text;

namespace DeepReader.Types.Infrastructure.BinaryReaders
{
    /// <summary>
    /// Implements an <see cref="IBufferReader"/> that can read primitive data types from a byte array.
    /// </summary>
    public sealed class BinaryBufferReader : IBufferReader
    {
        private readonly byte[] _data;
        private int _relativePositon;
        private int _position;

        /// <summary>
        /// Gets the offset into the underlying byte array to start reading from.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Gets the effective length of the readable region of the underlying byte array.
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets or sets the current reading position within the underlying byte array.
        /// </summary>
        public int Position
        {
            get => _relativePositon;
            set
            {
                var newPosition = Offset + value;

                if (value < 0) throw ExceptionHelper.PositionLessThanZeroException(nameof(value));
                if (value > Length) throw ExceptionHelper.PositionGreaterThanLengthOfByteArrayException(nameof(value));

                _relativePositon = value;
                _position = newPosition;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryBufferReader"/> class based on the specified byte array.
        /// </summary>
        /// <param name="data">The byte array to read from.</param>
        public BinaryBufferReader(byte[] data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _position = 0;
            _relativePositon = 0;
            Offset = 0;
            Length = data.Length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryBufferReader"/> class based on the specified byte array.
        /// <para>A provided offset and length specifies the boundaries to use for reading.</para>
        /// </summary>
        /// <param name="data">The byte array to read from.</param>
        /// <param name="offset">The 0-based offset into the byte array at which to begin reading from.
        /// <para>Cannot exceed the bounds of the byte array.</para></param>
        /// <param name="length">The maximum number of bytes that the reader will use for reading, relative to the offset position.
        /// <para>Cannot exceed the bounds of the byte array.</para></param>
        public BinaryBufferReader(byte[] data, int offset, int length)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));

            if (offset < 0) throw ExceptionHelper.OffsetLessThanZeroException(nameof(offset));
            if (length < 0) throw ExceptionHelper.LengthLessThanZeroException(nameof(length));
            if (length > _data.Length - offset) throw ExceptionHelper.LengthGreaterThanEffectiveLengthOfByteArrayException();

            _position = offset;
            _relativePositon = 0;
            Offset = offset;
            Length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryBufferReader"/> class based on the specified byte array segment.
        /// </summary>
        /// <param name="data">The byte array segment to read from.</param>
        public BinaryBufferReader(in ArraySegment<byte> data)
        {
            _data = data.Array ?? throw new ArgumentNullException(nameof(data));
            _position = data.Offset;
            _relativePositon = 0;
            Offset = data.Offset;
            Length = data.Count;
        }


        /// <summary>
        /// Reads a boolean value from the underlying byte array and advances the current position by one byte.
        /// </summary>
        public bool ReadBoolean() => InternalReadByte() != 0;

        /// <summary>
        /// Reads the next byte from the underlying byte array and advances the current position by one byte.
        /// </summary>
        public byte ReadByte() => InternalReadByte();

        /// <summary>
        /// Reads the specified number of bytes from the underlying byte array into a new byte array and advances the current position by that number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        public byte[] ReadBytes(int count) => InternalReadSpan(count).ToArray();

        /// <summary>
        /// Reads a decimal value from the underlying byte array and advances the current position by sixteen bytes.
        /// </summary>
        public decimal ReadDecimal()
        {
            var buffer = InternalReadSpan(16);
            try
            {
                return new decimal(
#if NET6_0_OR_GREATER
                                   stackalloc
#else
                                   new
#endif
                                   []
                                   {
                    BinaryPrimitives.ReadInt32LittleEndian(buffer),          // lo
                    BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(4)), // mid
                    BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(8)), // hi
                    BinaryPrimitives.ReadInt32LittleEndian(buffer.Slice(12)) // flags
                });
            }
            catch (ArgumentException e)
            {
                // ReadDecimal cannot leak out ArgumentException
                throw ExceptionHelper.DecimalReadingException(e);
            }
        }

        /// <summary>
        /// Reads a double-precision floating-point number from the underlying byte array and advances the current position by eight bytes.
        /// </summary>
        public double ReadDouble() => BitConverter.Int64BitsToDouble(BinaryPrimitives.ReadInt64LittleEndian(InternalReadSpan(8)));

        /// <summary>
        /// Reads a 16-bit signed integer from the underlying byte array and advances the current position by two bytes.
        /// </summary>
        public short ReadInt16() => BinaryPrimitives.ReadInt16LittleEndian(InternalReadSpan(2));

        /// <summary>
        /// Reads a 32-bit signed integer from the underlying byte array and advances the current position by four bytes.
        /// </summary>
        public int ReadInt32() => BinaryPrimitives.ReadInt32LittleEndian(InternalReadSpan(4));

        /// <summary>
        /// Reads a 64-bit signed integer from the underlying byte array and advances the current position by eight bytes.
        /// </summary>
        public long ReadInt64() => BinaryPrimitives.ReadInt64LittleEndian(InternalReadSpan(8));

        /// <summary>
        /// Reads a signed byte from the underlying byte array and advances the current position by one byte.
        /// </summary>
        public sbyte ReadSByte() => (sbyte)InternalReadByte();

        /// <summary>
        /// Reads a single-precision floating-point number from the underlying byte array and advances the current position by four bytes.
        /// </summary>
#if NETSTANDARD2_0
        public virtual unsafe float ReadSingle()
        {
            var m_buffer = InternalReadSpan(4);
            uint tmpBuffer = (uint)(m_buffer[0] | m_buffer[1] << 8 | m_buffer[2] << 16 | m_buffer[3] << 24);

            return *((float*)&tmpBuffer);
        }
#else
        public float ReadSingle() => BitConverter.Int32BitsToSingle(BinaryPrimitives.ReadInt32LittleEndian(InternalReadSpan(4)));
#endif

        /// <summary>
        /// Reads a span of bytes from the underlying byte array and advances the current position by the number of bytes read.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        public ReadOnlySpan<byte> ReadSpan(int count) => InternalReadSpan(count);

        /// <summary>
        /// Reads a 16-bit unsigned integer from the underlying byte array and advances the current position by two bytes.
        /// </summary>
        public ushort ReadUInt16() => BinaryPrimitives.ReadUInt16LittleEndian(InternalReadSpan(2));

        /// <summary>
        /// Reads a 32-bit unsigned integer from the underlying byte array and advances the current position by four bytes.
        /// </summary>
        public uint ReadUInt32() => BinaryPrimitives.ReadUInt32LittleEndian(InternalReadSpan(4));

        /// <summary>
        /// Reads a 64-bit unsigned integer from the underlying byte array and advances the current position by eight bytes.
        /// </summary>
        public ulong ReadUInt64() => BinaryPrimitives.ReadUInt64LittleEndian(InternalReadSpan(8));


        /// <summary>
        /// Reads the next byte from the underlying byte array and advances the current position by one byte.
        /// </summary>
        private byte InternalReadByte()
        {
            int curPos = _position;
            int newPos = curPos + 1;
            int relPos = _relativePositon + 1;

            if ((uint)relPos > (uint)Length)
            {
                _relativePositon = Length;
                throw ExceptionHelper.EndOfDataException();
            }

            _relativePositon = relPos;
            _position = newPos;

            return _data[curPos];
        }

        /// <summary>
        /// Returns a read-only span over the specified number of bytes from the underlying byte array and advances the current position by that number of bytes.
        /// </summary>
        /// <param name="count">The size of the read-only span to return.</param>
        private ReadOnlySpan<byte> InternalReadSpan(int count)
        {
            if (count <= 0) return ReadOnlySpan<byte>.Empty;

            int curPos = _position;
            int newPos = curPos + count;
            int relPos = _relativePositon + count;

            if ((uint)relPos > (uint)Length)
            {
                _relativePositon = Length;
                throw ExceptionHelper.EndOfDataException();
            }

            _relativePositon = relPos;
            _position = newPos;

            return new ReadOnlySpan<byte>(_data, curPos, count);
        }


        #region EosTypes

        public Signature ReadSignature()
        {
            var type = ReadByte();
            var signBytes = ReadBytes(Constants.SignKeyDataSize);
            ReadByte();//read another byte

            switch (type)
            {
                case (int)KeyType.R1:
                    return CryptoHelper.SignBytesToString(signBytes, "R1", "SIG_R1_");
                case (int)KeyType.K1:
                    return CryptoHelper.SignBytesToString(signBytes, "K1", "SIG_K1_");
                default:
                    Log.Error(new Exception($"Signature type {type} not supported"), "");
                    Log.Error(new Exception(CryptoHelper.SignBytesToString(signBytes, "K1", "SIG_K1_")), "");
                    return CryptoHelper.SignBytesToString(signBytes, "K1", "SIG_K1_");  // TODO ??
            }
        }

        public Checksum160 ReadChecksum160()
        {
            return ReadBytes(20);
        }

        public Checksum256 ReadChecksum256()
        {
            return ReadBytes(32);
        }

        public TransactionId ReadTransactionId()
        {
            return ReadBytes(32);
        }

        public Checksum512 ReadChecksum512()
        {
            return ReadBytes(64);
        }

        public ushort ReadVarUint16()
        {
            ushort v = 0;
            var bit = 0;
            while (true)
            {
                var b = ReadByte();
                v |= (ushort)((b & 0x7f) << bit);
                bit += 7;
                if ((b & 0x80) == 0)
                    break;
            }
            return (ushort)(v >> 0);
        }

        public short ReadVarInt16()
        {
            short v = 0;
            var bit = 0;
            while (true)
            {
                var b = ReadByte();
                v |= (short)((b & 0x7f) << bit);
                bit += 7;
                if ((b & 0x80) == 0)
                    break;
            }
            return (short)(v >> 0);
        }

        public uint ReadVarUint32()
        {
            uint v = 0;
            var bit = 0;
            while (true)
            {
                var b = ReadByte();
                v |= (uint)((b & 0x7f) << bit);
                bit += 7;
                if ((b & 0x80) == 0)
                    break;
            }
            return v >> 0;
        }

        public int ReadVarInt32()
        {
            int v = 0;
            var bit = 0;
            while (true)
            {
                var b = ReadByte();
                v |= (b & 0x7f) << bit;
                bit += 7;
                if ((b & 0x80) == 0)
                    break;
            }
            return v >> 0;
        }

        public ulong ReadVarUint64()
        {
            ulong v = 0;
            var bit = 0;
            while (true)
            {
                var b = ReadByte();
                ulong v1 = (ulong)((b & 0x7f) << bit);
                v |= v1;
                bit += 7;
                if ((b & 0x80) == 0)
                    break;
            }
            return v >> 0;
        }

        public long ReadVarInt64()
        {
            long v = 0;
            var bit = 0;
            while (true)
            {
                var b = ReadByte();
                v |= (long)((b & 0x7f) << bit);
                bit += 7;
                if ((b & 0x80) == 0)
                    break;
            }
            return v >> 0;
        }

        public Uint128 ReadUInt128()
        {
            return ReadBytes(16);
        }

        public Int128 ReadInt128()
        {
            return ReadBytes(16);
        }

        public string ReadString()
        {
            var length = Convert.ToInt32(Read7BitEncodedInt());
            return length > 0 ? Encoding.UTF8.GetString(ReadBytes(length)) : string.Empty;
        }

        public Bytes ReadBytes()
        {
            var length = Convert.ToInt32(ReadVarUint32());
            return ReadBytes(length);
        }

        public PublicKey ReadPublicKey()
        {
            var type = ReadByte();
            var keyBytes = ReadBytes(Constants.PubKeyDataSize);

            switch (type)
            {
                case (int)KeyType.K1:
                    return CryptoHelper.PubKeyBytesToString(keyBytes, "K1");
                case (int)KeyType.R1:
                    return CryptoHelper.PubKeyBytesToString(keyBytes, "R1", "PUB_R1_");
                case (int)KeyType.WA:
                    return CryptoHelper.PubKeyBytesToString(keyBytes, "WA", "PUB_WA_");
                default:
                    Log.Error(new Exception($"public key type {type} not supported"), "");
                    Log.Error(CryptoHelper.PubKeyBytesToString(keyBytes, "R1", "PUB_R1_"));
                    return CryptoHelper.PubKeyBytesToString(keyBytes, "R1", "PUB_R1_"); // TODO ??
            }
        }

        public ActionDataBytes ReadActionDataBytes()
        {
            var length = Convert.ToInt32(ReadVarUint32());
            return new ActionDataBytes(ReadBytes(length));
        }

        public float ReadFloat32()
        {
            return BitConverter.ToSingle(ReadBytes(4));
        }

        public double ReadFloat64()
        {
            return BitConverter.ToDouble(ReadBytes(8));
        }

        public Float128 ReadFloat128()
        {
            var bytes = ReadBytes(16);
            return bytes;
        }

        public Asset ReadAsset()
        {
            var binaryAmount = ReadBytes(8);

            var symbol = ReadSymbol();
            var amount = SerializationHelper.SignedBinaryToDecimal(binaryAmount, symbol.Precision + 1);

            if (symbol.Precision > 0)
                amount = amount.Substring(0, amount.Length - symbol.Precision) + '.' + amount.Substring(amount.Length - symbol.Precision);

            return new Asset() { Symbol = symbol, Amount = long.Parse(amount), };
        }

        public Symbol ReadSymbol()
        {
            var precision = ReadByte();

            return new Symbol(ReadSymbolCode(), precision);
        }

        public SymbolCode ReadSymbolCode()
        {
            var a = ReadBytes(8);

            int len;
            for (len = 0; len < a.Length; ++len)
                if (a[len] == 0)
                    break;

            return new SymbolCode(a, string.Join("", a.Take(len)));
        }

        public int Read7BitEncodedInt()
        {
            /*
             * Copied from Microsofts BinaryReader Source-Code
             */

            // Unlike writing, we can't delegate to the 64-bit read on
            // 64-bit platforms. The reason for this is that we want to
            // stop consuming bytes if we encounter an integer overflow.

            uint result = 0;
            byte byteReadJustNow;

            // Read the integer 7 bits at a time. The high bit
            // of the byte when on means to continue reading more bytes.
            //
            // There are two failure cases: we've read more than 5 bytes,
            // or the fifth byte is about to cause integer overflow.
            // This means that we can read the first 4 bytes without
            // worrying about integer overflow.

            const int maxBytesWithoutOverflow = 4;
            for (int shift = 0; shift < maxBytesWithoutOverflow * 7; shift += 7)
            {
                // ReadByte handles end of stream cases for us.
                byteReadJustNow = ReadByte();
                result |= (byteReadJustNow & 0x7Fu) << shift;

                if (byteReadJustNow <= 0x7Fu)
                {
                    return (int)result; // early exit
                }
            }

            // Read the 5th byte. Since we already read 28 bits,
            // the value of this byte must fit within 4 bits (32 - 28),
            // and it must not have the high bit set.

            byteReadJustNow = ReadByte();
            if (byteReadJustNow > 0b_1111u)
            {
                throw new FormatException("Format_Bad7BitInt");
            }

            result |= (uint)byteReadJustNow << maxBytesWithoutOverflow * 7;
            return (int)result;

        }

        public long Read7BitEncodedInt64()
        {
            /*
             * Copied from Microsofts BinaryReader Source-Code
             */

            ulong result = 0;
            byte byteReadJustNow;

            // Read the integer 7 bits at a time. The high bit
            // of the byte when on means to continue reading more bytes.
            //
            // There are two failure cases: we've read more than 10 bytes,
            // or the tenth byte is about to cause integer overflow.
            // This means that we can read the first 9 bytes without
            // worrying about integer overflow.

            const int maxBytesWithoutOverflow = 9;
            for (int shift = 0; shift < maxBytesWithoutOverflow * 7; shift += 7)
            {
                // ReadByte handles end of stream cases for us.
                byteReadJustNow = ReadByte();
                result |= (byteReadJustNow & 0x7Ful) << shift;

                if (byteReadJustNow <= 0x7Fu)
                {
                    return (long)result; // early exit
                }
            }

            // Read the 10th byte. Since we already read 63 bits,
            // the value of this byte must fit within 1 bit (64 - 63),
            // and it must not have the high bit set.

            byteReadJustNow = ReadByte();
            if (byteReadJustNow > 0b_1u)
            {
                throw new FormatException("Format_Bad7BitInt");
            }

            result |= (ulong)byteReadJustNow << maxBytesWithoutOverflow * 7;
            return (long)result;

        }

        #endregion

        public enum KeyType
        {
            K1 = 0,
            R1 = 1,
            WA = 2,
        };

        public char[] ReadChars(int count)
        {
            // TODO 
            return Array.Empty<char>();
        }

        public char ReadChar()
        {
            // TODO
            return 'x';
        }
    }
}