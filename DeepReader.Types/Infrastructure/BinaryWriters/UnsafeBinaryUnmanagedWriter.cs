using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DeepReader.Types.Infrastructure.BinaryWriters
{
    public class UnsafeBinaryUnmanagedWriter : IBufferWriter
    {
        private unsafe byte* data;

        public unsafe UnsafeBinaryUnmanagedWriter(ref byte* data, int length)
        {
            this.data = data;
            this.Length = length;
        }

        public unsafe UnsafeBinaryUnmanagedWriter(ref byte* data)
        {
            this.data = data;
        }

        public long Length { get; }
        public long Position { get; set; }

        public unsafe void Write(bool value)
        {
            const int size = sizeof(bool);
            CheckLength(size);
            Unsafe.Write(data, value);
            SetPosition(size);
        }

        public unsafe void Write(byte value)
        {
            const int size = sizeof(byte);
            CheckLength(size);
            Unsafe.Write(data, value);
            SetPosition(size);
        }

        public unsafe void Write(byte[] buffer)
        {
            var size = buffer.Length;
            CheckLength(size);
            Unsafe.Write(data, buffer);
            SetPosition(size);
        }

        public unsafe void Write(byte[] buffer, int offset, int length)
        {
            CheckLength(length);
            Unsafe.Write(data + offset, buffer.Take(length));
            SetPosition(length);
        }

        public unsafe void Write(decimal value)
        {
            const int size = sizeof(decimal);
            CheckLength(size);
            Unsafe.Write(data, value);
            SetPosition(size);
        }

        public unsafe void Write(double value)
        {
            const int size = sizeof(double);
            CheckLength(size);
            Unsafe.Write(data, value);
            SetPosition(size);
        }

        public unsafe void Write(short value)
        {
            const int size = sizeof(short);
            CheckLength(size);
            Unsafe.Write(data, value);
            SetPosition(size);
        }

        public unsafe void Write(int value)
        {
            const int size = sizeof(int);
            CheckLength(size);
            Unsafe.Write(data, value);
            SetPosition(size);
        }

        public unsafe void Write(long value)
        {
            const int size = sizeof(long);
            CheckLength(size);
            Unsafe.Write(data, value);
            SetPosition(size);
        }

        public unsafe void Write(sbyte value)
        {
            const int size = sizeof(sbyte);
            CheckLength(size);
            Unsafe.Write(data, value);
            SetPosition(size);
        }

        public unsafe void Write(float value)
        {
            const int size = sizeof(float);
            CheckLength(size);
            Unsafe.Write(data, value);
            SetPosition(size);
        }

        public unsafe void Write(in ReadOnlySpan<byte> buffer)
        {
            var size = buffer.Length;
            CheckLength(size);
            Unsafe.Write(data, buffer.ToArray());
            SetPosition(size);

        }

        public unsafe void Write(ushort value)
        {
            const int size = sizeof(ushort);
            CheckLength(size);
            Unsafe.Write(data, value);
            SetPosition(size);
        }

        public unsafe void Write(uint value)
        {
            const int size = sizeof(uint);
            CheckLength(size);
            Unsafe.Write(data, value);
            SetPosition(size);
        }

        public unsafe void Write(ulong value)
        {
            const int size = sizeof(ulong);
            CheckLength(size);
            Unsafe.Write(data, value);
            SetPosition(size);
        }

        public unsafe void Write7BitEncodedInt(int value)
        {

        }

        public unsafe void Write(in ReadOnlySpan<char> value)
        {
            var size = value.Length;
            CheckLength(size);
            Unsafe.Write(data, value.ToArray());
            SetPosition(size);
        }

        public ReadOnlySpan<byte> ToReadOnlySpan()
        {
            throw new NotImplementedException();
        }

        private void CheckLength(int size)
        {
            if (Position + size > Length)
                throw new ArgumentOutOfRangeException("size", "Index out of Range");
        }

        private unsafe void SetPosition(int size)
        {
            Position += size;
            data += size;
        }
    }
}
