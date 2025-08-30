using System.Runtime.CompilerServices;

namespace Weaver.Abstractions.Extensions
{
    public static class ArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetValidOrdinal(this int[] arr, int index, out int ordinal)
        {
            ordinal = arr[index];
            return ordinal >= 0;
        }
    }
}
