#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace Claims.Api.DependencyInjection;

/// <summary>
/// A helper type to make it possible to inject dependencies that implement the same interface
/// but which require different implementations to be injected into different places.
///
/// For each type that should be injected, a class derived from this one should be created with
/// the appropriate name and should be used instead of the original dependency both at the site
/// of where it is registered and also where it gets injected. Getting to the real dependency
/// can be done by simply using the "Obj" property.
/// </summary>
/// <remarks>
/// One solution for injecting different implementation for the same interface would be to just
/// register keyed dependencies, but this approach was chosen due to these reasons:
/// 1. Encountered an issue with keyed dependencies not working properly, though not sure about
/// the exact cause (got a System.InvalidOperationException with the following message:
/// 'This service descriptor is keyed. Your service provider may not support keyed services.');
/// 2. Using a dedicated source object might look a bit nicer than keyed dependency injection,
/// at least at the site where the dependency gets injected.
/// </remarks>
public abstract class SourceOf<T>
{
    public T Obj { get; init; }
}
