using DeepReader.Types.Eosio.Chain;
using HotChocolate.Language;

namespace DeepReader.Apis.GraphQl.EosTypesObjectTypes
{
    internal class TransactionIdType : ScalarType
    {
        public TransactionIdType() : base("TransactionId")
        {
            Description = "Represents a TransactionId as a string to the user";
        }

        public override Type RuntimeType => typeof(TransactionId);

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
                return (TransactionId)stringValueNode.Value;
            }
            throw new SerializationException("The specified value has to be of type string", this);
        }

        public override IValueNode ParseResult(object? resultValue)
        {
            return ParseValue(resultValue);
        }

        public override IValueNode ParseValue(object? runtimeValue)
        {
            if (runtimeValue is TransactionId c)
            {
                return new StringValueNode(null, c.StringVal, false);
            }
            throw new SerializationException($"The specified value has to be of type {typeof(TransactionId)}", this);
        }

        public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
        {
            if (resultValue == null)
                throw new ArgumentNullException(nameof(resultValue));

            runtimeValue = null;

            if (resultValue is string s)
            {
                runtimeValue = (TransactionId)s;
                return true;
            }

            return false;
        }

        public override bool TrySerialize(object? runtimeValue, out object? resultValue)
        {
            if (runtimeValue == null)
                throw new ArgumentNullException(nameof(runtimeValue));

            resultValue = null;

            if (runtimeValue is TransactionId c)
            {
                resultValue = c.StringVal;
                return true;
            }

            return false;
        }
    }
}