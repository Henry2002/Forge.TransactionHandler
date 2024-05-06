using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Forge.TransactionHandler;

public interface ITransactionGroup : IEnumerable<DbContext>
{

    /// <summary>
    /// Asynchronously saves changes to all <see cref="DbContext"/> in this <see cref="ITransactionGroup"/>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public Task<int> SaveChangesAsync(CancellationToken token = default);

    /// <summary>
    /// Saves changes to all <see cref="DbContext"/> in this <see cref="ITransactionGroup"/>
    /// </summary>
    /// <returns></returns>
    public int SaveChanges();

    /// <summary>
    /// Gets the name of this transaction group
    /// </summary>
    /// <returns></returns>
    string GetName();


}
