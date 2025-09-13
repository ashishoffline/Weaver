using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Data.Common;

/// <summary>
/// Provides extension methods for <see cref="DbDataReader"/> to safely retrieve nullable values.
/// </summary>
/// <remarks>
/// These methods simplify reading database fields that may contain <c>DBNull</c>,
/// returning nullable types (e.g., <c>int?</c>, <c>string?</c>) instead of throwing exceptions.
/// </remarks>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "In externally visible method 'int DataReaderExtensions.GetOrdinalSafe(DbDataReader reader, string name)', validate parameter 'reader' is non-null before using it. If appropriate, throw an 'ArgumentNullException' when the argument is 'null'. (https://learn.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules/ca1062)")]
public static class DataReaderExtensions
{
    /// <summary>
    /// Safely retrieves the column ordinal (zero-based index) for the specified column name 
    /// from the <see cref="DbDataReader"/>.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="DbDataReader"/> instance to look up the column from.
    /// </param>
    /// <param name="name">
    /// The name of the column whose ordinal is to be retrieved.
    /// </param>
    /// <returns>
    /// The zero-based column ordinal if the column exists; otherwise <c>-1</c>.
    /// </returns>
    /// <remarks>
    /// Unlike <see cref="DbDataReader.GetOrdinal(string)"/>, this method does not throw an 
    /// <see cref="IndexOutOfRangeException"/> when the column is not found.
    /// </remarks>
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
    
    /// <summary>
    /// Retrieves a nullable <see cref="short"/> (Int16) value from the specified column ordinal 
    /// of the <see cref="DbDataReader"/>.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="DbDataReader"/> instance to read from.
    /// </param>
    /// <param name="ordinal">
    /// The zero-based column ordinal of the value to retrieve.
    /// </param>
    /// <returns>
    /// The <see cref="short"/> value at the specified column if it is not <see cref="DBNull.Value"/>;
    /// otherwise <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// This method provides a safe way to handle database columns that may contain NULL values, 
    /// returning <see langword="null"/> instead of throwing an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short? GetNullableInt16(this DbDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? default : reader.GetInt16(ordinal);
    }


    /// <summary>
    /// Retrieves a nullable <see cref="int"/> value from the specified column ordinal 
    /// of the <see cref="DbDataReader"/>.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="DbDataReader"/> instance to read from.
    /// </param>
    /// <param name="ordinal">
    /// The zero-based column ordinal of the value to retrieve.
    /// </param>
    /// <returns>
    /// The <see cref="int"/> value at the specified column if it is not <see cref="DBNull.Value"/>;
    /// otherwise <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// This method provides a safe way to handle database columns that may contain NULL values, 
    /// returning <see langword="null"/> instead of throwing an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int? GetNullableInt32(this DbDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? default : reader.GetInt32(ordinal);
    }

    
    /// <summary>
    /// Retrieves a nullable <see cref="long"/> (Int64) value from the specified column ordinal 
    /// of the <see cref="DbDataReader"/>.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="DbDataReader"/> instance to read from.
    /// </param>
    /// <param name="ordinal">
    /// The zero-based column ordinal of the value to retrieve.
    /// </param>
    /// <returns>
    /// The <see cref="long"/> value at the specified column if it is not <see cref="DBNull.Value"/>;
    /// otherwise <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// This method provides a safe way to handle database columns that may contain NULL values, 
    /// returning <see langword="null"/> instead of throwing an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long? GetNullableInt64(this DbDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? default : reader.GetInt64(ordinal);
    }

    
    /// <summary>
    /// Retrieves a nullable <see cref="double"/> value from the specified column ordinal 
    /// of the <see cref="DbDataReader"/>.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="DbDataReader"/> instance to read from.
    /// </param>
    /// <param name="ordinal">
    /// The zero-based column ordinal of the value to retrieve.
    /// </param>
    /// <returns>
    /// The <see cref="double"/> value at the specified column if it is not <see cref="DBNull.Value"/>;
    /// otherwise <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// This method provides a safe way to handle database columns that may contain NULL values, 
    /// returning <see langword="null"/> instead of throwing an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double? GetNullableDouble(this DbDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? default : reader.GetDouble(ordinal);
    }


    /// <summary>
    /// Retrieves a nullable <see cref="string"/> value from the specified column ordinal 
    /// of the <see cref="DbDataReader"/>.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="DbDataReader"/> instance to read from.
    /// </param>
    /// <param name="ordinal">
    /// The zero-based column ordinal of the value to retrieve.
    /// </param>
    /// <returns>
    /// The <see cref="string"/> value at the specified column if it is not <see cref="DBNull.Value"/>;
    /// otherwise <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// This method provides a safe way to handle database columns that may contain NULL values, 
    /// returning <see langword="null"/> instead of throwing an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetNullableString(this DbDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? default : reader.GetString(ordinal);
    }


    /// <summary>
    /// Retrieves a nullable <see cref="bool"/> value from the specified column ordinal 
    /// of the <see cref="DbDataReader"/>.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="DbDataReader"/> instance to read from.
    /// </param>
    /// <param name="ordinal">
    /// The zero-based column ordinal of the value to retrieve.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/> value at the specified column if it is not <see cref="DBNull.Value"/>;
    /// otherwise <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// This method provides a safe way to handle database columns that may contain NULL values, 
    /// returning <see langword="null"/> instead of throwing an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool? GetNullableBoolean(this DbDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? default : reader.GetBoolean(ordinal);
    }
    
    /// <summary>
    /// Retrieves a nullable <see cref="byte"/> value from the specified column ordinal 
    /// of the <see cref="DbDataReader"/>.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="DbDataReader"/> instance to read from.
    /// </param>
    /// <param name="ordinal">
    /// The zero-based column ordinal of the value to retrieve.
    /// </param>
    /// <returns>
    /// The <see cref="byte"/> value at the specified column if it is not <see cref="DBNull.Value"/>;
    /// otherwise <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// This method provides a safe way to handle database columns that may contain NULL values, 
    /// returning <see langword="null"/> instead of throwing an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte? GetNullableByte(this DbDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? default : reader.GetByte(ordinal);
    }
    
    /// <summary>
    /// Retrieves a nullable <see cref="char"/> value from the specified column ordinal 
    /// of the <see cref="DbDataReader"/>.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="DbDataReader"/> instance to read from.
    /// </param>
    /// <param name="ordinal">
    /// The zero-based column ordinal of the value to retrieve.
    /// </param>
    /// <returns>
    /// The <see cref="char"/> value at the specified column if it is not <see cref="DBNull.Value"/>;
    /// otherwise <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// This method provides a safe way to handle database columns that may contain NULL values, 
    /// returning <see langword="null"/> instead of throwing an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static char? GetNullablechar(this DbDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? default : reader.GetChar(ordinal);
    }
    
    /// <summary>
    /// Retrieves a nullable <see cref="decimal"/> value from the specified column ordinal 
    /// of the <see cref="DbDataReader"/>.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="DbDataReader"/> instance to read from.
    /// </param>
    /// <param name="ordinal">
    /// The zero-based column ordinal of the value to retrieve.
    /// </param>
    /// <returns>
    /// The <see cref="decimal"/> value at the specified column if it is not <see cref="DBNull.Value"/>;
    /// otherwise <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// This method provides a safe way to handle database columns that may contain NULL values, 
    /// returning <see langword="null"/> instead of throwing an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal? GetNullableDecimal(this DbDataReader reader, int ordinal)
    {
        return reader!.IsDBNull(ordinal) ? default : reader.GetDecimal(ordinal);
    }
    
    /// <summary>
    /// Retrieves a nullable <see cref="DateTime"/> value from the specified column ordinal 
    /// of the <see cref="DbDataReader"/>.
    /// </summary>
    /// <param name="reader">
    /// The <see cref="DbDataReader"/> instance to read from.
    /// </param>
    /// <param name="ordinal">
    /// The zero-based column ordinal of the value to retrieve.
    /// </param>
    /// <returns>
    /// The <see cref="DateTime"/> value at the specified column if it is not <see cref="DBNull.Value"/>;
    /// otherwise <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// This method provides a safe way to handle database columns that may contain NULL values, 
    /// returning <see langword="null"/> instead of throwing an exception.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime? GetNullableDateTime(this DbDataReader reader, int ordinal)
    {
        return reader.IsDBNull(ordinal) ? default : reader.GetDateTime(ordinal);
    }

}
