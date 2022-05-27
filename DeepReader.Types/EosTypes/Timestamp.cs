using System.Text.Json.Serialization;
using DeepReader.Types.JsonConverters;
using DeepReader.Types.Other;

namespace DeepReader.Types.EosTypes;

[JsonConverter(typeof(TimestampJsonConverter))]
public sealed class Timestamp : PooledObject<Timestamp>, IEosioSerializable<Timestamp>
{
    private uint _ticks;

    public Timestamp()
    {

    }

    public Timestamp(uint ticks)
    {
        _ticks = ticks;
    }

    public static Timestamp ReadFromBinaryReader(BinaryReader reader, bool fromPool = true)
    {
        // when Faster wants to deserialize and Object, we take an Object from the Pool
        // when Faster evicts the Object we return it to the Pool
        var obj = fromPool ? TypeObjectPool.Get() : new Timestamp();

        obj._ticks = reader.ReadUInt32();

        return obj;
    }

    public void WriteToBinaryWriter(BinaryWriter writer)
    {
        writer.Write(_ticks);

        // when Faster wants to deserialize and Object, we take an Object from the Pool
        // when Faster evicts the Object we return it to the Pool
        TypeObjectPool.Return(this);
    }

    public DateTime ToDateTime()
    {
        return DateTimeOffset.FromUnixTimeSeconds(_ticks).DateTime;
    }

    public override string ToString()
    {
        return ToDateTime().ToString();
    }

    public static Timestamp Zero => new();
}