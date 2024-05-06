using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forge.TransactionHandler;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class DisableTransactionHandlerAttribute : Attribute
{
}
