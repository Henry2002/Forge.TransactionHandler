using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using static Forge.TransactionHandler.ServiceConfiguration.TransactionHandlerGroupConfig;

namespace Forge.TransactionHandler.ServiceConfiguration;

public sealed class TransactionHandlerGroupConfig(string groupName) :
    IPredicateConfig

{
    Expression<Func<HttpContext, bool>>? _saveIf;

    Func<HttpContext, bool>? _saveIfCompiled;
    internal string GroupName { get; } = groupName;

    internal Func<HttpContext, bool> ShouldSave
    {
        get
        {
            if (_saveIfCompiled is null)
            {
                _saveIf ??= (context) => context.Response.StatusCode <= 299
                    && context.Response.StatusCode >= 200;

                while (_saveIf.CanReduce)
                {
                    _saveIf.Reduce();
                }

                _saveIfCompiled = _saveIf.Compile();
            }

            return _saveIfCompiled;
        }
    }
    /// <summary>
    /// configures the saving condition for this <see cref="ITransactionGroup"/>
    /// </summary>
    /// <param name="saveIf"></param>
    /// <returns></returns>
    public IPredicateConfig SaveIf(Expression<Func<HttpContext, bool>> saveIf)
    {
        _saveIf = saveIf;

        return this;
    }

    
    public IPredicateConfig And(Expression<Func<HttpContext, bool>> predicate)
    {
        var binary = Expression.AndAlso(predicate.Body, Expression.Invoke(_saveIf!, predicate.Parameters[0]));

        _saveIf = Expression.Lambda<Func<HttpContext, bool>>(binary, predicate.Parameters[0]);

        return this;
    }

    
    public IPredicateConfig Or(Expression<Func<HttpContext, bool>> predicate)
    {
        var binary = Expression.OrElse(predicate.Body, Expression.Invoke(_saveIf!, predicate.Parameters[0]));

        _saveIf = Expression.Lambda<Func<HttpContext, bool>>(binary, predicate.Parameters[0]);

        return this;
    }

    public interface IPredicateConfig
    {
        /// <summary>
        /// Adds to the saving condition with the || operator
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IPredicateConfig Or(Expression<Func<HttpContext, bool>> predicate);

        /// <summary>
        /// Adds to the saving condition with the && operator
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IPredicateConfig And(Expression<Func<HttpContext, bool>> predicate);
    }
}
