namespace AiKnowledgeExchange.SharedKernel;

using CliFx.Exceptions;

internal static class CounterNameValidation
{
    public static void ValidateCounterName(string counterName)
    {
        if (string.IsNullOrWhiteSpace(counterName))
        {
            throw new CommandException("the counter name cannot be null or whitespace.");
        }

        if (!counterName.Trim().Equals(counterName, StringComparison.Ordinal))
        {
            throw new CommandException(
                $"the counter name must not have any leading or trailing whitespace, but it was '{counterName}'."
            );
        }
    }
}
