namespace AiKnowledgeExchange.SharedKernel;

using CliFx.Exceptions;

internal static class DirectoryValidation
{
    public static void ValidateDirectory(DirectoryInfo directoryInfo, string optionName, string displayName)
    {
        if (!Directory.Exists(directoryInfo.FullName) && !File.Exists(directoryInfo.FullName))
        {
            throw new CommandException($"{displayName} does not exist: '{directoryInfo.FullName}'");
        }

        if (File.Exists(directoryInfo.FullName))
        {
            throw new CommandException(
                $"Path for {optionName} is a file, but must be a directory: '{directoryInfo.FullName}'"
            );
        }

        if (Directory.Exists(directoryInfo.FullName))
        {
            return;
        }

        throw new CommandException($"{displayName} does not exist: '{directoryInfo.FullName}'");
    }
}
