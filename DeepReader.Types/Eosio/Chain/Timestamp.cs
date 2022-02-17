using System.Text.Json.Serialization;
using DeepReader.Types.Fc;
using DeepReader.Types.JsonConverters;

namespace DeepReader.Types.Eosio.Chain;

[JsonConverter(typeof(TimestampJsonConverter))]
public class Timestamp : BinaryType
{
    public uint _ticks;

    public static implicit operator Timestamp(uint value)
    {
        return new Timestamp() {_ticks = value};
    }

    public DateTime ToDateTime()
    {
        return DateTimeOffset.FromUnixTimeSeconds(_ticks).DateTime;
    }

    public static Timestamp Zero => new();
}