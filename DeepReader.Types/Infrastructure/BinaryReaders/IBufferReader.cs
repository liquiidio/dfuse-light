using System.Text;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;
using DeepReader.Types.Helpers;
using Serilog;

namespace DeepReader.Types.Infrastructure.BinaryReaders
{
    public interface IBufferReader
    {
        /// <summary>
        /// Gets the effective length of the readable region of the stream.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets or sets the current reading position within the stream.
        /// </summary>
        int Position { get; }

        /// <summary>
        /// Reads a boolean value from the current binary stream and advances the current position within the stream by one byte.
        /// </summary>
        bool ReadBoolean();

        /// <summary>
        /// Reads the next byte from the current binary stream and advances the current position within the stream by one byte.
        /// </summary>
        byte ReadByte();

        /// <summary>
        /// Reads the specified number of bytes from the current binary stream into a byte array and advances the current position within the stream by that number of bytes.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        byte[] ReadBytes(int count);

        /// <summary>
        /// Reads a decimal value from the current binary stream and advances the current position within the stream by sixteen bytes.
        /// </summary>
        decimal ReadDecimal();

        /// <summary>
        /// Reads a double-precision floating-point number from the current binary stream and advances the current position within the stream by eight bytes.
        /// </summary>
        double ReadDouble();

        /// <summary>
        /// Reads a 16-bit signed integer from the current binary stream and advances the current position within the stream by two bytes.
        /// </summary>
        short ReadInt16();

        /// <summary>
        /// Reads a 32-bit signed integer from the current binary stream and advances the current position within the stream by four bytes.
        /// </summary>
        int ReadInt32();

        /// <summary>
        /// Reads a 63-bit signed integer from the current binary stream and advances the current position within the stream by eight bytes.
        /// </summary>
        long ReadInt64();

        /// <summary>
        /// Reads a signed byte from the current binary stream and advances the current position within the stream by one byte.
        /// </summary>
        sbyte ReadSByte();

        /// <summary>
        /// Reads single-precision floating-point number from the current binary stream and advances the current position within the stream by four bytes.
        /// </summary>
        float ReadSingle();

        /// <summary>
        /// Reads a span of bytes from the current binary stream and advances the current position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="count">The number of bytes to read.</param>
        ReadOnlySpan<byte> ReadSpan(int count);

        /// <summary>
        /// Reads a 16-bit unsigned integer from the current binary stream and advances the current position within the stream by two bytes.
        /// </summary>
        ushort ReadUInt16();

        /// <summary>
        /// Reads a 32-bit unsigned integer from the current binary stream and advances the current position within the stream by four bytes.
        /// </summary>
        uint ReadUInt32();

        /// <summary>
        /// Reads 64-bit unsigned integer from the current binary stream and advances the current position within the stream by eight bytes.
        /// </summary>
        ulong ReadUInt64();

        char[] ReadChars(int count);

        char ReadChar();


        #region EosTypes

        Signature ReadSignature();

        Checksum160 ReadChecksum160();

        Checksum256 ReadChecksum256();

        TransactionId ReadTransactionId();

        Checksum512 ReadChecksum512();

        ushort ReadVarUint16();

        short ReadVarInt16();

        uint ReadVarUint32();

        int ReadVarInt32();

        ulong ReadVarUint64();

        long ReadVarInt64();

        Uint128 ReadUInt128();

        Int128 ReadInt128();

        string ReadString();

        Bytes ReadBytes();

        PublicKey ReadPublicKey();

        ActionDataBytes ReadActionDataBytes();

        float ReadFloat32();

        double ReadFloat64();

        Float128 ReadFloat128();

        Asset ReadAsset();

        Symbol ReadSymbol();

        SymbolCode ReadSymbolCode();

        int Read7BitEncodedInt();

        long Read7BitEncodedInt64();

        #endregion

    }
}