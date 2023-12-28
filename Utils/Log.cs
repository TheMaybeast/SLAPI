using Rage;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace SLAPI.Utils;

internal class Log
{
    private static bool LogCreated;
    public const string Path = @"SLAPI.log";

    public Log()
    {
        if (LogCreated) return;
        var message = "SLAPI v" + Assembly.GetExecutingAssembly().GetName().Version;
        message += Environment.NewLine;
        message += "-----------------------------------------------------------";
        message += Environment.NewLine;
        using (var writer = new StreamWriter(Path, false))
        {
            writer.WriteLine(message);
            writer.Close();
        }
        LogCreated = true;
    }
}

internal static class LogExtensions
{
    internal static void ToLog(this string log, bool toConsole = false)
    {
        if (toConsole) Game.LogTrivial(log);
        using var writer = new StreamWriter(Log.Path, true);

        writer.WriteLine("[" + DateTime.Now.ToString(CultureInfo.InvariantCulture) + "] " + log);
        writer.Close();
    }
}
