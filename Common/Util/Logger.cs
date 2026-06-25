using System.Collections.Concurrent;
using System.Diagnostics;
using Spectre.Console;

namespace NahidaImpact.Util;

public class Logger
{
    private static FileInfo? LogFile;
    private static readonly object _consoleLock = new();
    private static readonly BlockingCollection<string> _fileQueue = [];
    private static readonly Thread _fileWriterThread;
    private readonly string ModuleName;

    static Logger()
    {
        _fileWriterThread = new Thread(FileWriterLoop)
        {
            IsBackground = true,
            Name = "LogFileWriter"
        };
        _fileWriterThread.Start();
    }

    public Logger(string moduleName)
    {
        ModuleName = moduleName;
    }

    private static void FileWriterLoop()
    {
        foreach (var message in _fileQueue.GetConsumingEnumerable())
        {
            WriteToFile(message);
        }
    }

    public void Log(string message, LoggerLevel level)
    {
        var markup = $"[[[bold deepskyblue3_1]{DateTime.Now:HH:mm:ss}[/]]] " +
                     $"[[[gray]{ModuleName}[/]]] [[[{(ConsoleColor)level}]{level}[/]]] " +
                     $"{message.Replace("[", "[[").Replace("]", "]]")}";
        var plainText = $"[{DateTime.Now:HH:mm:ss}] [{ModuleName}] [{level}] {message}";

        lock (_consoleLock)
        {
            var savedInput = ConsoleManager.Input.ToList();
            ConsoleManager.RedrawInput("", false);
            AnsiConsole.MarkupLine(markup);
            ConsoleManager.RedrawInput(savedInput);
        }

        _fileQueue.Add(plainText);
    }

    public void Info(string message, Exception? e = null)
    {
        Log(message, LoggerLevel.INFO);
        if (e != null)
        {
            Log(e.Message, LoggerLevel.INFO);
            Log(e.StackTrace!, LoggerLevel.INFO);
        }
    }

    public void Warn(string message, Exception? e = null)
    {
        Log(message, LoggerLevel.WARN);
        if (e != null)
        {
            Log(e.Message, LoggerLevel.WARN);
            Log(e.StackTrace!, LoggerLevel.WARN);
        }
    }

    public void Error(string message, Exception? e = null)
    {
        Log(message, LoggerLevel.ERROR);
        if (e != null)
        {
            Log(e.Message, LoggerLevel.ERROR);
            Log(e.StackTrace!, LoggerLevel.ERROR);
        }
    }

    public void Fatal(string message, Exception? e = null)
    {
        Log(message, LoggerLevel.FATAL);
        if (e != null)
        {
            Log(e.Message, LoggerLevel.FATAL);
            Log(e.StackTrace!, LoggerLevel.FATAL);
        }
    }

    public void Debug(string message, Exception? e = null)
    {
        Log(message, LoggerLevel.DEBUG);
        if (e != null)
        {
            Log(e.Message, LoggerLevel.DEBUG);
            Log(e.StackTrace!, LoggerLevel.DEBUG);
        }
    }

    public static void SetLogFile(FileInfo file)
    {
        LogFile = file;
    }

    public static void WriteToFile(string message)
    {
        try
        {
            if (LogFile == null) return;
            using var sw = LogFile.AppendText();
            sw.WriteLine(message);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Failed to write log: {e.Message}");
        }
    }

    public static Logger GetByClassName()
    {
        return new Logger(new StackTrace().GetFrame(1)?.GetMethod()?.ReflectedType?.Name ?? "");
    }

    /// <summary>Drains pending file writes and stops the writer thread. Call once at shutdown.</summary>
    public static void Shutdown()
    {
        _fileQueue.CompleteAdding();
        if (!_fileWriterThread.Join(TimeSpan.FromSeconds(5)))
            Console.Error.WriteLine("Logger: file writer thread did not exit in time.");
    }
}

public enum LoggerLevel
{
    INFO = ConsoleColor.Cyan,
    WARN = ConsoleColor.Yellow,
    ERROR = ConsoleColor.Red,
    FATAL = ConsoleColor.DarkRed,
    DEBUG = ConsoleColor.Blue
}
