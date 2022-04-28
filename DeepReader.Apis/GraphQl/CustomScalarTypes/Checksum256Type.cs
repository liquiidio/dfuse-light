using DeepReader.Types.EosTypes;
using HotChocolate.Language;

namespace DeepReader.Apis.GraphQl.CustomScalarTypes
{
    internal class Checksum256Type : ScalarType
    {
        public Checksum256Type() : base("Checksum256")
        {
            Description = "Represents a Checksum256 as a string to the user";
        }

        public override Type RuntimeType => typeof(Checksum256);

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
                return (Checksum256) stringValueNode.Value;
            }
            throw new SerializationException("The specified value has to be of type string", this);
        }

        public override IValueNode ParseResult(object? resultValue)
        {
            return ParseValue(resultValue);
        }

        public override IValueNode ParseValue(object? runtimeValue)
        {
            if (runtimeValue is Checksum256 c)
            {
                return new StringValueNode(null, c.StringVal, false);
            }
            throw new SerializationException($"The specified value has to be of type {typeof(Checksum256)}", this);
        }

        public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
        {
            if (resultValue == null)
                throw new ArgumentNullException(nameof(resultValue));

            runtimeValue = null;

            if (resultValue is string s)
            {
                runtimeValue = (Checksum256)s;
                return true;
            }

            return false;
        }

        public override bool TrySerialize(object? runtimeValue, out object? resultValue)
        {
            if (runtimeValue == null)
                throw new ArgumentNullException(nameof(runtimeValue));

            resultValue = null;

            if (runtimeValue is Checksum256 c)
            {
                resultValue = c.StringVal;
                return true;
            }

            return false;
        }
    }
}