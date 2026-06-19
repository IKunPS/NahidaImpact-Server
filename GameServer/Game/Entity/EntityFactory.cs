using System.Reflection;

namespace NahidaImpact.GameServer.Game.Entity;

public static class EntityFactory
{
    public static T Create<T>(Type[] argTypes, object[] args) where T : BaseEntity
    {
        var constructor = typeof(T).GetConstructor(argTypes)
            ?? throw new InvalidOperationException($"No constructor found for type {typeof(T).Name} with specified parameters");

        return (T)constructor.Invoke(args);
    }

    public static T Create<T>(params object[] args) where T : BaseEntity
    {
        var argTypes = new Type[args.Length];
        for (int i = 0; i < args.Length; i++)
            argTypes[i] = args[i]?.GetType() ?? typeof(object);

        return Create<T>(argTypes, args);
    }
}