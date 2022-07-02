using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepReader.Types.Infrastructure.BinaryWriters
{
    public class UnsafeBinaryUnmanagedWriter : IBufferWriter
    {
        private UnmanagedMemoryStream unmanagedMemoryStream;

        public UnsafeBinaryUnmanagedWriter(UnmanagedMemoryStream unmanagedMemoryStream)
        {
            this.unmanagedMemoryStream = unmanagedMemoryStream;
        }

        public long Length { get; }
        public long Position { get; set; }
        public void Write(bool value)
        {
            throw new NotImplementedException();
        }

        public void Write(byte value)
        {
            throw new NotImplementedException();
        }

        public void Write(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public void Write(byte[] buffer, int offset, int length)
        {
            throw new NotImplementedException();
        }

        public void Write(decimal value)
        {
            throw new NotImplementedException();
        }

        public void Write(double value)
        {
            throw new NotImplementedException();
        }

        public void Write(short value)
        {
            throw new NotImplementedException();
        }

        public void Write(int value)
        {
            throw new NotImplementedException();
        }

        public void Write(long value)
        {
            throw new NotImplementedException();
        }

        public void Write(sbyte value)
        {
            throw new NotImplementedException();
        }

        public void Write(float value)
        {
            throw new NotImplementedException();
        }

        public void Write(in ReadOnlySpan<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public void Write(ushort value)
        {
            throw new NotImplementedException();
        }

        public void Write(uint value)
        {
            throw new NotImplementedException();
        }

        public void Write(ulong value)
        {
            throw new NotImplementedException();
        }

        public void Write7BitEncodedInt(int value)
        {
            throw new NotImplementedException();
        }

        public void Write(in ReadOnlySpan<char> value)
        {
            throw new NotImplementedException();
        }

        public ReadOnlySpan<byte> ToReadOnlySpan()
        {
            throw new NotImplementedException();
        }
    }
}
