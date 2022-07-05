using DeepReader.Types.Eosio.Chain;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeepReader.Storage.TiDB.ValueConverters
{
    internal class TransactionIdConverter : ValueConverter<TransactionId, byte[]>
    {
        public TransactionIdConverter() : base(
            v => v.Binary,
            v => (TransactionId)v)
        {

        }
    }
}