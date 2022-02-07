using DeepReader.JsonConverters;
using System.Text.Json.Serialization;

namespace DeepReader.Types.Eosio.Chain;

[JsonConverter(typeof(TimestampJsonConverter))]
public class Timestamp
{
    private readonly uint _binary;

    public static implicit operator Timestamp(uint value)
    {
        return new (value);
    }

    public DateTime ToDateTime()
    {
        return DateTimeOffset.FromUnixTimeSeconds(_binary).DateTime;
    }

    public Timestamp(uint binary) 
    { 
        _binary = binary; 
    }

    public static Timestamp Zero => new(0);
}