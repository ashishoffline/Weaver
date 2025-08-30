using System;
using System.Data.Common;

namespace Weaver.Abstractions.Extensions
{
    public static class DbDataReaderExtensions
    {
        public static int GetOrdinalSafe(this DbDataReader reader, string name)
        {
            try
            {
                return reader.GetOrdinal(name);
            }
            catch (IndexOutOfRangeException)
            {
                return -1;
            }
        }
    }
}
