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
        Func<TransactionHandlerGroupConfig, TransactionHandlerGroupConfig>? builder = null)
    {
        var groupConfig = new TransactionHandlerGroupConfig(groupName);

        builder?.Invoke(groupConfig);

        GroupConfigurations[groupName] = groupConfig;

        return this;
    }

    public TransactionHandlerConfig DisableLogging()
    {
        IsLoggingDisabled = true;

        return this;
    }
}
