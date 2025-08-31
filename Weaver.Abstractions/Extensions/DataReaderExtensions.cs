using System.Runtime.CompilerServices;

namespace System.Data.Common
{
    public static class DataReaderExtensions
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int? GetNullableInt32(this DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? default : reader.GetInt32(ordinal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double? GetNullableDouble(this DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? default : reader.GetDouble(ordinal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetNullableString(this DbDataReader reader, int ordinal)
        {
            return reader.IsDBNull(ordinal) ? default : reader.GetString(ordinal);
        }
    }
}
