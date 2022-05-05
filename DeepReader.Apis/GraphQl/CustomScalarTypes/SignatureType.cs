using DeepReader.Types.Fc.Crypto;
using HotChocolate.Language;

namespace DeepReader.Apis.GraphQl.CustomScalarTypes
{
    internal class SignatureType : ScalarType
    {
        public SignatureType() : base("Signature")
        {
            Description = "Represents a Signature as a string to the user";
        }

        public override Type RuntimeType => typeof(Signature);

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
                return (Signature)stringValueNode.Value;
            }
            throw new SerializationException("The specified value has to be of type string", this);
        }

        public override IValueNode ParseResult(object? resultValue)
        {
            return ParseValue(resultValue);
        }

        public override IValueNode ParseValue(object? runtimeValue)
        {
            if (runtimeValue is Signature c)
            {
                return new StringValueNode(null, c.StringVal, false);
            }
            throw new SerializationException($"The specified value has to be of type {typeof(Signature)}", this);
        }

        public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
        {
            runtimeValue = null;

            if (resultValue == null)
                throw new ArgumentNullException(nameof(resultValue));

            if (resultValue is string s)
            {
                runtimeValue = (Signature)s;
                return true;
            }

            return false;
        }

        public override bool TrySerialize(object? runtimeValue, out object? resultValue)
        {
            resultValue = null;

            if (runtimeValue == null)
                throw new ArgumentNullException(nameof(runtimeValue));

            if (runtimeValue is Signature c)
            {
                resultValue = c.StringVal;
                return true;
            }

            return false;
        }
    }
}