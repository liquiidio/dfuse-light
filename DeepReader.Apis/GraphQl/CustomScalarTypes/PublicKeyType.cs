using DeepReader.Types.EosTypes;
using HotChocolate.Language;

namespace DeepReader.Apis.GraphQl.CustomScalarTypes
{
    internal class PublicKeyType : ScalarType
    {
        public PublicKeyType() : base("PublicKey")
        {
            Description = "Represent a PublicKey as a string";
        }

        public override Type RuntimeType => typeof(PublicKey);

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
                return (PublicKey)stringValueNode.Value;
            }
            throw new SerializationException("The specified value has to be of type string", this);
        }

        public override IValueNode ParseResult(object? resultValue)
        {
            return ParseValue(resultValue);
        }

        public override IValueNode ParseValue(object? runtimeValue)
        {
            if (runtimeValue is PublicKey p)
            {
                return new StringValueNode(null, p.StringVal, false);
            }
            throw new SerializationException($"The specified value has to be of type {typeof(PublicKey)}", this);
        }

        public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
        {
            runtimeValue = null;

            if (resultValue == null)
                throw new ArgumentNullException(nameof(resultValue));

            if (resultValue is string s)
            {
                runtimeValue = (PublicKey)s;
                return true;
            }
            return false;
        }

        public override bool TrySerialize(object? runtimeValue, out object? resultValue)
        {
            resultValue = null;

            if (runtimeValue == null)
                throw new ArgumentNullException(nameof(runtimeValue));

            if (runtimeValue is PublicKey p)
            {
                resultValue = p.StringVal;
                return true;
            }
            return false;
        }
    }
}