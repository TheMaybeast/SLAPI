using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Rage;

namespace SLAPI.Utils;

internal enum LogLevel
{
    DEBUG,
    INFO,
    WARN,
    ERROR,
    FATAL
}

internal static class Log
{
    private const string Path = "SLAPI.log";

    public static void Write(string text, LogLevel logLevel = LogLevel.INFO, bool toConsole = false)
    {
        if (toConsole) Game.LogTrivial($"[{logLevel}] {text}");

        using var writer = new StreamWriter(Path, true);
        writer.WriteLine($"[{DateTime.Now.ToString(CultureInfo.InvariantCulture)}] [{logLevel}] {text}");
        writer.Close();
    }
}

internal static class LogExtensions
{
    public static void ToLog(this string message, LogLevel logLevel = LogLevel.INFO, bool toConsole = false) =>
        Log.Write(message, logLevel, toConsole);
}