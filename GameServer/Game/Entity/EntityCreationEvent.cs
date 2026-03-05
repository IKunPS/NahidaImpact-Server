using System;
using System.Reflection;

namespace NahidaImpact.GameServer.Game.Entity;

public static class EntityCreationEvent
{
    public static T Call<T>(Type[] argTypes, object[] args) where T : BaseEntity
    {
        try
        {
            var constructor = typeof(T).GetConstructor(argTypes);
            if (constructor == null)
                throw new InvalidOperationException($"No constructor found for type {typeof(T).Name} with specified parameters");
            
            var entity = (T)constructor.Invoke(args);
            return entity;
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Failed to create entity of type {typeof(T).Name}: {ex.Message}");
            return null;
        }
    }
    
    public static T Call<T>(params object[] args) where T : BaseEntity
    {
        var argTypes = new Type[args.Length];
        for (int i = 0; i < args.Length; i++)
        {
            argTypes[i] = args[i]?.GetType() ?? typeof(object);
        }
        
        return Call<T>(argTypes, args);
    }
}