namespace Forge.TransactionHandler.ServiceConfiguration;

public sealed class TransactionHandlerConfig
{
    readonly Dictionary<string, TransactionHandlerGroupConfig> GroupConfigurations = [];
    internal bool IsLoggingDisabled { get; private set; }

   internal TransactionHandlerGroupConfig this[string groupName]
    {
        get
        {
            GroupConfigurations.TryGetValue(groupName, out var config);

            return config ?? new(groupName);
        }
    }
    public TransactionHandlerConfig HandleTransactionGroup(
        string groupName,
        Action<TransactionHandlerGroupConfig>? config = null)
    {
        var groupConfig = new TransactionHandlerGroupConfig(groupName);

        config?.Invoke(groupConfig);

        GroupConfigurations[groupName] = groupConfig;

        return this;
    }

    public TransactionHandlerConfig DisableLogging()
    {
        IsLoggingDisabled = true;

        return this;
    }
}
