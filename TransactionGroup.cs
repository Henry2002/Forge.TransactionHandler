using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Transactions;

namespace Forge.TransactionHandler;


public sealed class TransactionGroup(string name, IList<DbContext> contexts) : 
    ITransactionGroup
    
{
    readonly string _name = name;

    readonly IList<DbContext> _contexts = contexts;

    public async Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        if(_contexts.Count == 1)
        {
            return await _contexts[0].SaveChangesAsync(token);
        }

        using TransactionScope scope = new();

        int numberOfWrites = 0;

        foreach(var context in _contexts)
        {
            numberOfWrites += await context.SaveChangesAsync(token);
        }

        scope.Complete();

        return numberOfWrites;
    }

    public int SaveChanges()
    {
        if (_contexts.Count == 1)
        {
            return _contexts[0].SaveChanges();
        }

        using TransactionScope scope = new();

        int numberOfWrites = 0;

        foreach (var context in _contexts)
        {
            numberOfWrites += context.SaveChanges();
        }

        scope.Complete();

        return numberOfWrites;
    }

    public string GetName()
    {
        return _name;
    }
    public IEnumerator<DbContext> GetEnumerator() => _contexts.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _contexts.GetEnumerator();

}
