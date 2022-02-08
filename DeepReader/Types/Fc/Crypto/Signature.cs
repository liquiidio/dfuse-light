﻿namespace DeepReader.Types.Fc.Crypto;

public class Signature
{
    private string _value = string.Empty;

    public static implicit operator Signature(string value)
    {
        return new Signature { _value = value };
    }

    public static implicit operator string(Signature value)
    {
        return value._value;
    }

    public string ToJson()
    {
        return _value;
    }

    public static Signature Empty => new();
}

//public class SignatureConverter : JsonConverter<Signature>
//{
//    public override void WriteJson(JsonWriter writer, Signature value, JsonSerializer serializer)
//    {
//        writer.WriteValue((string)value);
//    }

//    public override Signature ReadJson(JsonReader reader, Type objectType, Signature existingValue, bool hasExistingValue, JsonSerializer serializer)
//    {
//        return (Signature)reader.Value;
//    }
//}