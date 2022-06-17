using DeepReader.Storage.Faster.ActionTraces.Base;
using DeepReader.Storage.Faster.ActionTraces.Standalone;
using DeepReader.Types.StorageTypes;
using FASTER.common;
using FASTER.core;

namespace DeepReader.Storage.Faster.ActionTraces.Client;

public sealed class ActionTraceClientSerializer : IClientSerializer<ulong, ActionTrace, ActionTraceInput, ActionTraceOutput>
{
    /// <summary>
    /// Write element to given destination, with length bytes of space available
    /// </summary>
    /// <param name="k">Element to write</param>
    /// <param name="dst">Destination memory</param>
    /// <param name="length">Space (bytes) available at destination</param>
    /// <returns>True if write succeeded, false if not (insufficient space)</returns>
    public unsafe bool Write(ref ulong k, ref byte* dst, int length)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Write element to given destination, with length bytes of space available
    /// </summary>
    /// <param name="v">Element to write</param>
    /// <param name="dst">Destination memory</param>
    /// <param name="length">Space (bytes) available at destination</param>
    /// <returns>True if write succeeded, false if not (insufficient space)</returns>
    public unsafe bool Write(ref ActionTrace v, ref byte* dst, int length)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Write element to given destination, with length bytes of space available
    /// </summary>
    /// <param name="i">Element to write</param>
    /// <param name="dst">Destination memory</param>
    /// <param name="length">Space (bytes) available at destination</param>
    /// <returns>True if write succeeded, false if not (insufficient space)</returns>
    public unsafe bool Write(ref ActionTraceInput i, ref byte* dst, int length)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Read output from source memory location, increment pointer by amount read
    /// </summary>
    /// <param name="src">Source memory location</param>
    /// <returns>Output</returns>
    public unsafe ActionTraceOutput ReadOutput(ref byte* src)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Read key from source memory location, increment pointer by amount read
    /// </summary>
    /// <param name="src">Source memory location</param>
    /// <returns>Key</returns>
    public unsafe ulong ReadKey(ref byte* src)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Read key from source memory location, increment pointer by amount read
    /// </summary>
    /// <param name="src">Source memory location</param>
    /// <returns>Key</returns>
    public unsafe ActionTrace ReadValue(ref byte* src)
    {
        throw new NotImplementedException();
    }
}