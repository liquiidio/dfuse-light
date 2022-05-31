using DeepReader.Types.Fc;
using HotChocolate.Language;

namespace DeepReader.Apis.GraphQl.CustomScalarTypes
{
    internal class VarInt32Type : ScalarType
    {
        public VarInt32Type() : base("VarInt32")
        {
            Description = "Represents a VarInt32 as a string";
        }

        public override Type RuntimeType => typeof(VarInt32);

        public override bool IsInstanceOfType(IValueNode valueSyntax)
        {
            if (valueSyntax == null)
                throw new ArgumentNullException(nameof(valueSyntax));
            return valueSyntax is StringValueNode;
        }

        public override object? ParseLiteral(IValueNode valueSyntax)
        {
            if (valueSyntax is StringValueNode stringValueNode)
            {
                return (VarInt32)Convert.ToInt32(stringValueNode.Value);
            }
            throw new SerializationException("The specified value has to be of type string", this);
        }

        public override IValueNode ParseResult(object? resultValue)
        {
            return ParseValue(resultValue);
        }

        public override IValueNode ParseValue(object? runtimeValue)
        {
            if (runtimeValue is VarInt32 v)
            {
                return new StringValueNode(null, v.Value.ToString(), false);
            }
            throw new SerializationException($"The specified value has to be of type {typeof(VarInt32)}", this);
        }

        public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
        {
            runtimeValue = null;

            if (resultValue == null)
                throw new ArgumentNullException(nameof(resultValue));

            if (resultValue is string s)
            {
                runtimeValue = (VarInt32)int.Parse(s);
                return true;
            }
            return false;
        }

        public override bool TrySerialize(object? runtimeValue, out object? resultValue)
        {
            resultValue = null;

            if (runtimeValue == null)
                throw new ArgumentNullException(nameof(runtimeValue));

            if (runtimeValue is VarInt32 v)
            {
                resultValue = v.Value;
                return true;
            }
            return false;
        }
    }
}