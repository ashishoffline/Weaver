using System.Collections.Frozen;
using System.Reflection;
using Weaver.Abstractions;

namespace Weaver;

/// <summary>
/// Provides a cached registry of source-generated <see cref="IDbDataReaderMapper{T}"/> implementations,
/// indexed by their target type. This cache is initialized once at startup using reflection and
/// stored in a <see cref="FrozenDictionary{TKey, TValue}"/> for optimal lookup performance.
/// </summary>
internal static class MapperCache
{
    /// <summary>
    /// A frozen dictionary mapping target types to their corresponding <see cref="IDbDataReaderMapper"/> instances.
    /// </summary>
    private static readonly FrozenDictionary<Type, IDbDataReaderMapper> _mapperCache = GetMapperCache();

    /// <summary>
    /// Discovers all non-abstract, non-interface types in the current AppDomain that implement
    /// <see cref="IDbDataReaderMapper{T}"/>, have a public parameterless constructor, and are instantiable.
    /// Builds a frozen dictionary mapping each target type <c>T</c> to its corresponding mapper instance.
    /// </summary>
    /// <returns>
    /// A <see cref="FrozenDictionary{TKey, TValue}"/> containing all discovered mappers keyed by their target type.
    /// </returns>
    private static FrozenDictionary<Type, IDbDataReaderMapper> GetMapperCache()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type =>
                        typeof(IDbDataReaderMapper).IsAssignableFrom(type) &&
                        !type.IsInterface &&
                        !type.IsAbstract &&
                        type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null) is not null)
                .Select(type =>
                {
                    var mapperInterface = type.GetInterfaces()
                        .Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDbDataReaderMapper<>));

                    Type targetType = mapperInterface.GetGenericArguments()[0];
                    IDbDataReaderMapper mapperInstance = (IDbDataReaderMapper)Activator.CreateInstance(type)!;

                    return new KeyValuePair<Type, IDbDataReaderMapper>(targetType, mapperInstance);
                })
                .ToFrozenDictionary();
    }

    /// <summary>
    /// Retrieves the strongly typed <see cref="IDbDataReaderMapper{T}"/> instance for the specified target type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The target type for which a mapper is requested.</typeparam>
    /// <returns>The corresponding <see cref="IDbDataReaderMapper{T}"/> instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no mapper is registered for the specified type <typeparamref name="T"/>.
    /// Ensure that a source-generated mapper exists and is discoverable at runtime.
    /// </exception>
    internal static IDbDataReaderMapper<T> GetMapper<T>()
    {
        if (_mapperCache.TryGetValue(typeof(T), out var mapper) && mapper is IDbDataReaderMapper<T> typedMapper)
        {
            return typedMapper;
        }

        throw new InvalidOperationException($"No mapper found for type {typeof(T).FullName}. Ensure that a source-generated mapper exists for this type.");
    }
}
