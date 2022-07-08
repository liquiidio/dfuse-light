using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KGySoft.CoreLibraries;

namespace DeepReader.Types.Infrastructure.BinaryWriters
{
    // TODO make these Methods MethodImpl.AgressiveInline

    /// <summary>
    /// class wraps BinaryWriter in the IBufferWriter-interface
    /// </summary>
    public class BinaryWriter : IBufferWriter
    {
        private readonly System.IO.BinaryWriter _binaryWriter;

        public static implicit operator BinaryWriter(System.IO.BinaryWriter binaryWriter)
        {
            return new BinaryWriter(binaryWriter);
        }

        public BinaryWriter(System.IO.BinaryWriter binaryWriter)
        {
            _binaryWriter = binaryWriter;
        }

        public unsafe BinaryWriter(byte* src, long length)
        {
            _binaryWriter = new System.IO.BinaryWriter(new UnmanagedMemoryStream(src, length, length, FileAccess.Write));
        }

        public long Length => _binaryWriter.BaseStream.Length;
        public long Position
        {
            get => _binaryWriter.BaseStream.Position;
            set => _binaryWriter.BaseStream.Position = value;
        }

        public void Write(bool value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(byte value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(byte[] buffer)
        {
            _binaryWriter.Write(buffer);
        }

        public void Write(byte[] buffer, int index, int count)
        {
            _binaryWriter.Write(buffer, index, count);
        }

        public void Write(decimal value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(double value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(short value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(int value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(long value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(sbyte value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(float value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(in ReadOnlySpan<byte> buffer)
        {
            _binaryWriter.Write(buffer);
        }

        public void Write(ushort value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(uint value)
        {
            _binaryWriter.Write(value);
        }

        public void Write(ulong value)
        {
            _binaryWriter.Write(value);
        }

        public void Write7BitEncodedInt(int value)
        {
            _binaryWriter.Write7BitEncodedInt(value);
        }

        public void Write(in ReadOnlySpan<char> value)
        {
            _binaryWriter.Write(value);
        }

        public ReadOnlySpan<byte> ToReadOnlySpan()
        {
            return _binaryWriter.BaseStream.ToArray();
        }
    }
}
