using DeepReader.Types.EosTypes;
using HotChocolate.Language;

namespace DeepReader.Apis.GraphQl.CustomScalarTypes
{
    internal class ActionDataBytesType : ScalarType
    {
        public ActionDataBytesType() : base("ActionDataBytes")
        {
            Description = "Represents a ActionDataBytes as a string to the user";
        }

        public override Type RuntimeType => typeof(ActionDataBytes);

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
                return (ActionDataBytes) Convert.FromBase64String(stringValueNode.Value);
            }
            throw new SerializationException("The specified value has to be of type string", this);
        }

        public override IValueNode ParseResult(object? resultValue)
        {
            return ParseValue(resultValue);
        }

        public override IValueNode ParseValue(object? runtimeValue)
        {
            if (runtimeValue is ActionDataBytes c)
            {
                return new StringValueNode(null, Convert.ToBase64String(c.Binary), false);
            }
            throw new SerializationException($"The specified value has to be of type {typeof(ActionDataBytes)}", this);
        }

        public override bool TryDeserialize(object? resultValue, out object? runtimeValue)
        {
            runtimeValue = null;

            if (resultValue == null)
                throw new ArgumentNullException(nameof(resultValue));

            if (resultValue is string s)
            {
                runtimeValue = (ActionDataBytes)Convert.FromBase64String(s);
                return true;
            }

            return false;
        }

        public override bool TrySerialize(object? runtimeValue, out object? resultValue)
        {
            resultValue = null;

            if (runtimeValue == null)
                throw new ArgumentNullException(nameof(runtimeValue));

            if (runtimeValue is ActionDataBytes c)
            {
                resultValue = Convert.ToBase64String(c.Binary);
                return true;
            }

            return false;
        }
    }
}