using Microsoft.EntityFrameworkCore;
using static Forge.TransactionHandler.ServiceConfiguration.TransactionGroupConfig;

namespace Forge.TransactionHandler.ServiceConfiguration;

public sealed class TransactionGroupConfig : INameConfig, IContextConfig
{
    private string? _name;

    internal string Name 
    { 
        get 
        {
            return _name ?? 
                throw new InvalidOperationException("No name has been configured for a handled transaction group");
        } 
    }

    internal readonly HashSet<Type> Contexts = [];
    public IContextConfig Add<TContext>() where TContext : DbContext
    {
        Add(typeof(TContext));

        return this;
    }

    public IContextConfig Add(Type context)
    {
        if (!typeof(DbContext).IsAssignableFrom(context))
            throw new ArgumentException("Cannot add non DbContext to TransactionGroup");

        Contexts.Add(context);

        return this;
    }

    public IContextConfig UseName(string name)
    {
        _name = name;

        return this;
    }

    public interface INameConfig
    {
        IContextConfig UseName(string name);
        
    }

    public interface IContextConfig
    {
        IContextConfig Add<TContext>() where TContext : DbContext;

        IContextConfig Add(Type context);
    }
}