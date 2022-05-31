using DeepReader.Types.EosTypes;
using HotChocolate.Language;

namespace DeepReader.Apis.GraphQl.CustomScalarTypes
{
    internal class TimestampType : ScalarType
    {
        public TimestampType() : base("Timestamp")
        {
            Description = "Represents a Timestamp as a string";
        }

        public override Type RuntimeType => typeof(Timestamp);

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
                Timestamp t = Convert.ToUInt32(DateTime.Parse(stringValueNode.Value).Ticks);
                return t;
            }
            throw new SerializationException("The specified value has to be of type string", this);
        }

        public override IValueNode ParseResult(object? resultValue)
        {
            return ParseValue(resultValue);
        }

        public override IValueNode ParseValue(object? runtimeValue)
        {
            if (runtimeValue is Timestamp t)
            {
                return new StringValueNode(null, t.ToDateTime().ToString(), false);
            }
            throw new SerializationException($"The specified value has to be of type {typeof(Timestamp)}", this);
        }

        public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
        {
            runtimeValue = null;

            if (resultValue == null)
                throw new ArgumentNullException(nameof(resultValue));

            if (resultValue is string s)
            {
                Timestamp t = Convert.ToUInt32(DateTime.Parse(s).Ticks);
                return true;
            }

            return false;
        }

        public override bool TrySerialize(object? runtimeValue, out object? resultValue)
        {
            resultValue = null;

            if (runtimeValue == null)
                throw new ArgumentNullException(nameof(runtimeValue));

            if (runtimeValue is Timestamp t)
            {
                resultValue = t.ToDateTime().ToString();
                return true;
            }

            return false;
        }
    }
}