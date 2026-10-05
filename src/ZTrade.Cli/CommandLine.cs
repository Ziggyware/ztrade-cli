using System.Globalization;

namespace ZTrade.Cli;

public sealed class UsageException : Exception
{
    public UsageException(string message) : base(message) { }
}

/// <summary>Minimal <c>command --key value --flag</c> parser. Unknown options are rejected, not ignored.</summary>
public sealed class CommandLine
{
    private readonly Dictionary<string, string> _options = new(StringComparer.OrdinalIgnoreCase);
    private readonly HashSet<string> _consumed = new(StringComparer.OrdinalIgnoreCase);

    private CommandLine(string? command, Dictionary<string, string> options)
    {
        Command = command;
        _options = options;
    }

    public string? Command { get; }

    public static CommandLine Parse(IReadOnlyList<string> args)
    {
        string? command = null;
        var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < args.Count; i++)
        {
            var arg = args[i];
            if (arg.StartsWith("--", StringComparison.Ordinal))
            {
                var key = arg[2..];
                if (key.Length == 0) throw new UsageException("Empty option name '--'.");
                var hasValue = i + 1 < args.Count && !args[i + 1].StartsWith("--", StringComparison.Ordinal);
                options[key] = hasValue ? args[++i] : "true";
            }
            else if (command is null)
            {
                command = arg;
            }
            else
            {
                throw new UsageException($"Unexpected argument '{arg}'.");
            }
        }

        return new CommandLine(command, options);
    }

    public string? Get(string key)
    {
        _consumed.Add(key);
        return _options.GetValueOrDefault(key);
    }

    public string Require(string key) =>
        Get(key) ?? throw new UsageException($"Missing required option --{key}.");

    public int GetInt(string key, int fallback) =>
        Get(key) is { } v
            ? int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n)
                ? n : throw new UsageException($"--{key} must be an integer.")
            : fallback;

    public decimal GetDecimal(string key, decimal fallback) =>
        Get(key) is { } v
            ? decimal.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out var n)
                ? n : throw new UsageException($"--{key} must be a number.")
            : fallback;

    /// <summary>Call after reading every option a command supports.</summary>
    public void EnsureNoUnknownOptions()
    {
        var unknown = _options.Keys.Where(k => !_consumed.Contains(k)).ToArray();
        if (unknown.Length > 0) throw new UsageException($"Unknown option(s): {string.Join(", ", unknown.Select(u => "--" + u))}");
    }
}

