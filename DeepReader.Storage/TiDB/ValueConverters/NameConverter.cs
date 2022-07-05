﻿using DeepReader.Types.EosTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DeepReader.Storage.TiDB.ValueConverters
{
    public class NameConverter : ValueConverter<Name, ulong>
    {
        public NameConverter() : base(
            v => (ulong)v,
            v => (Name)v.ToString())
        {
        }
    }
}