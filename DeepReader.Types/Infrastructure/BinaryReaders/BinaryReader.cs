using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepReader.Types.Eosio.Chain;
using DeepReader.Types.EosTypes;
using DeepReader.Types.Fc.Crypto;

namespace DeepReader.Types.Infrastructure.BinaryReaders
{
    // TODO make these Methods MethodImpl.AgressiveInline

    /// <summary>
    /// class wraps BinaryReader in the IBufferReader-interface
    /// </summary>
    public class BinaryReader : IBufferReader
    {
        private readonly System.IO.BinaryReader _binaryReader;

        public static implicit operator BinaryReader(System.IO.BinaryReader binaryReader)
        {
            return new BinaryReader(binaryReader);
        }

        public BinaryReader(System.IO.BinaryReader binaryReader)
        {
            _binaryReader = binaryReader;
        }

        public unsafe BinaryReader(ref byte* data, int length)
        {
            _binaryReader =
                new System.IO.BinaryReader(new UnmanagedMemoryStream(data, length, length, FileAccess.Read));
        }

        public int Length => (int)_binaryReader.BaseStream.Length;
        public int Position => (int)_binaryReader.BaseStream.Position;

        public bool ReadBoolean()
        {
            return _binaryReader.ReadBoolean();
        }

        public byte ReadByte()
        {
            return _binaryReader.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            return _binaryReader.ReadBytes(count);
        }

        public decimal ReadDecimal()
        {
            return _binaryReader.ReadDecimal();
        }

        public double ReadDouble()
        {
            return _binaryReader.ReadDouble();
        }

        public short ReadInt16()
        {
            return _binaryReader.ReadInt16();
        }

        public int ReadInt32()
        {
            return _binaryReader.ReadInt32();
        }

        public long ReadInt64()
        {
            return _binaryReader.ReadInt64();
        }

        public sbyte ReadSByte()
        {
            return _binaryReader.ReadSByte();
        }

        public float ReadSingle()
        {
            return _binaryReader.ReadSingle();
        }

        public ReadOnlySpan<byte> ReadSpan(int count)
        {
            return _binaryReader.ReadBytes(count);
        }

        public ushort ReadUInt16()
        {
            return _binaryReader.ReadUInt16();
        }

        public uint ReadUInt32()
        {
            return _binaryReader.ReadUInt32();
        }

        public ulong ReadUInt64()
        {
            return _binaryReader.ReadUInt64();
        }

        public char[] ReadChars(int count)
        {
            return _binaryReader.ReadChars(count);
        }

        public char ReadChar()
        {
            return _binaryReader.ReadChar();
        }

        public Signature ReadSignature()
        {
            throw new NotImplementedException();
        }

        public Checksum160 ReadChecksum160()
        {
            throw new NotImplementedException();
        }

        public Checksum256 ReadChecksum256()
        {
            throw new NotImplementedException();
        }

        public TransactionId ReadTransactionId()
        {
            throw new NotImplementedException();
        }

        public Checksum512 ReadChecksum512()
        {
            throw new NotImplementedException();
        }

        public ushort ReadVarUint16()
        {
            throw new NotImplementedException();
        }

        public short ReadVarInt16()
        {
            throw new NotImplementedException();
        }

        public uint ReadVarUint32()
        {
            throw new NotImplementedException();
        }

        public int ReadVarInt32()
        {
            throw new NotImplementedException();
        }

        public ulong ReadVarUint64()
        {
            throw new NotImplementedException();
        }

        public long ReadVarInt64()
        {
            throw new NotImplementedException();
        }

        public Uint128 ReadUInt128()
        {
            throw new NotImplementedException();
        }

        public Int128 ReadInt128()
        {
            throw new NotImplementedException();
        }

        public string ReadString()
        {
            throw new NotImplementedException();
        }

        public Bytes ReadBytes()
        {
            throw new NotImplementedException();
        }

        public PublicKey ReadPublicKey()
        {
            throw new NotImplementedException();
        }

        public ActionDataBytes ReadActionDataBytes()
        {
            throw new NotImplementedException();
        }

        public float ReadFloat32()
        {
            throw new NotImplementedException();
        }

        public double ReadFloat64()
        {
            throw new NotImplementedException();
        }

        public Float128 ReadFloat128()
        {
            throw new NotImplementedException();
        }

        public Asset ReadAsset()
        {
            throw new NotImplementedException();
        }

        public Symbol ReadSymbol()
        {
            throw new NotImplementedException();
        }

        public SymbolCode ReadSymbolCode()
        {
            throw new NotImplementedException();
        }

        public int Read7BitEncodedInt()
        {
           return _binaryReader.Read7BitEncodedInt();
        }

        public long Read7BitEncodedInt64()
        {
            return _binaryReader.Read7BitEncodedInt64();
        }
    }
}
