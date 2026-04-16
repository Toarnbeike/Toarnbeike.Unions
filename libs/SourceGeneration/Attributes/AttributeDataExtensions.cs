using Microsoft.CodeAnalysis;

namespace Toarnbeike.SourceGeneration.Attributes;

public static class AttributeDataExtensions
{
    extension(AttributeData attribute)
    {
        /// <summary>
        /// Get the constructor argument at the first or provided position, if present.
        /// </summary>
        /// <param name="index">The index to search, defaults to first position.</param>
        /// <returns>The <see cref="TypedConstant"/> constructor argument if present, otherwise null.</returns>
        /// <remarks>
        /// Use the strongly typed version if possible.
        /// </remarks>
        public TypedConstant? GetConstructorArgument(int index = 0) =>
            attribute.ConstructorArguments.Length > index
                ? attribute.ConstructorArguments[index] : null;

        /// <summary>
        /// Get the constructor argument at the first or provided position, if present, and convert its value to <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The expected type of the constructor argument requested. </typeparam>
        /// <param name="index">The index to search, defaults to first position.</param>
        /// <returns>The <typeparamref name="T"/> constructor argument at position <paramref name="index"/>> if present, otherwise null.</returns>
        public T? GetConstructorArgument<T> (int index = 0) => 
            attribute.GetConstructorArgument(index)?.Value is T result ? result : default;

        /// <summary>
        /// Get the constructor argument at the first or provided position, if present.
        /// </summary>
        /// <param name="index">The index to search, defaults to first position.</param>
        /// <returns>The Type as <see cref="INamedTypeSymbol"/> constructor argument at position <paramref name="index"/> if present, otherwise null.</returns>
        public INamedTypeSymbol? GetConstructorTypeArgument(int index = 0) =>
            attribute.GetConstructorArgument(index)?.Value as INamedTypeSymbol;

        /// <summary>
        /// Get a named argument from the attribute.
        /// </summary>
        /// <typeparam name="T">The expected return type of the argument.</typeparam>
        /// <param name="name">The name of the argument.</param>
        /// <returns>The value of the argument with the given <paramref name="name"/>, if present, otherwise null.</returns>
        public T? GetNamedArgumentValue<T>(string name)
        {
            var arg = attribute.NamedArguments.FirstOrDefault(kvp => kvp.Key == name);
            return arg.Value.Value is T value ? value : default;
        }
    }
}
