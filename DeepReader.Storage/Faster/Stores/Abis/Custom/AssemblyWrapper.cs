using System.Reflection;

namespace DeepReader.Storage.Faster.Stores.Abis.Custom;

public class AssemblyWrapper
{
    private byte[]? _binary;

    private Assembly? _assembly;

    public Assembly Assembly => _assembly ??= (_binary != null ? Assembly.Load(_binary) : null);

    public byte[] Binary => _binary ??= AssemblyToByteArray();

    public AssemblyWrapper(Assembly assembly)
    {
        _assembly = assembly;
    }

    public AssemblyWrapper(byte[] binary)
    {
        _binary = binary;
    }

    byte[] AssemblyToByteArray()
    {
        var generator = new Lokad.ILPack.AssemblyGenerator();
        return generator.GenerateAssemblyBytes(_assembly);
    }

    public static implicit operator Assembly(AssemblyWrapper wrapper)
    {
        return wrapper.Assembly;
    }
}