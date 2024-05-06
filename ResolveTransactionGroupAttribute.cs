using Microsoft.Extensions.DependencyInjection;

namespace Forge.TransactionHandler;
public class ResolveTransactionGroupAttribute(object key) : FromKeyedServicesAttribute(key)
{
}
