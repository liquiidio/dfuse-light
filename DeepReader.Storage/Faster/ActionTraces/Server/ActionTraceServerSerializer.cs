using DeepReader.Storage.Faster.ActionTraces.Base;
using DeepReader.Storage.Faster.ActionTraces.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.core;

namespace DeepReader.Storage.Faster.ActionTraces.Server;

public sealed class ActionTraceServerSerializer : IServerSerializer<ulong, ActionTrace, ActionTraceInput, ActionTraceOutput>
{
    public unsafe bool Write(ref ulong k, ref byte* dst, int length)
    {
        throw new NotImplementedException();
    }

    public unsafe bool Write(ref ActionTrace v, ref byte* dst, int length)
    {
        throw new NotImplementedException();
    }

    public unsafe bool Write(ref ActionTraceOutput o, ref byte* dst, int length)
    {
        throw new NotImplementedException();
    }

    public int GetLength(ref ActionTraceOutput o)
    {
        throw new NotImplementedException();
    }

    public unsafe ref ulong ReadKeyByRef(ref byte* src)
    {
        throw new NotImplementedException();
    }

    public unsafe ref ActionTrace ReadValueByRef(ref byte* src)
    {
        // UnmanagedMemoryStream  ? We don't have a length ... but we could write the length as first 4 bytes of every message ?!
        throw new NotImplementedException();
    }

    public unsafe ref ActionTraceInput ReadInputByRef(ref byte* src)
    {
        throw new NotImplementedException();
    }

    public unsafe ref ActionTraceOutput AsRefOutput(byte* src, int length)
    {
        throw new NotImplementedException();
    }

    public unsafe void SkipOutput(ref byte* src)
    {
        throw new NotImplementedException();
    }
}