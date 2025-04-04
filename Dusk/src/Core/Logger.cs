using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Dusk.Core;

public static class Logger
{
    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();
    
    private static StreamWriter _logOutput;
    
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }
    
    private static readonly ConsoleColor[] LevelColors =
    [
        ConsoleColor.Gray,     // Debug
        ConsoleColor.White,    // Info
        ConsoleColor.Yellow,   // Warning
        ConsoleColor.Red,      // Error
        ConsoleColor.DarkRed   // Critical
    ];
    
    public static bool ShowTimestamps { get; set; } = true;
    public static bool ShowLogLevel { get; set; } = true;
    public static bool UseColors { get; set; } = true;
    public static LogLevel MinimumLogLevel { get; set; } = LogLevel.Debug;
    public static bool IsInitialized { get; private set; }
    
    public static void Initialize()
    {
        if (IsInitialized) return;
        
        if (!AllocConsole()) return;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        IsInitialized = true;
        
        _logOutput = new StreamWriter(Console.OpenStandardOutput())
        {
            AutoFlush = true
        };
            
        Console.SetOut(_logOutput);

        string asciiArt = @"    ____             __  
   / __ \__  _______/ /__
  / / / / / / / ___/ //_/
 / /_/ / /_/ (__  ) ,<   
/_____/\__,_/____/_/|_|  
                         ";
        _logOutput.WriteLine(asciiArt);
    }

    private static void Log(string message, LogLevel level = LogLevel.Info)
    {
        if (!IsInitialized) Initialize();
        if (level < MinimumLogLevel) return;

        if (UseColors)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = LevelColors[(int)level];
            _logOutput.WriteLine(FormatMessage(message, level));
            Console.ForegroundColor = originalColor;
        }
        else
        {
            _logOutput.WriteLine(FormatMessage(message, level));
        }
    }
    
    private static string FormatMessage(string message, LogLevel level)
    {
        var parts = new System.Collections.Generic.List<string>();
        
        if (ShowTimestamps)
        {
            parts.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}]");
        }
        
        if (ShowLogLevel)
        {
            parts.Add($"[{level.ToString().ToUpper()}]");
        }
        
        parts.Add(message);
        
        return string.Join(" ", parts);
    }
    
    public static void Debug(string message) => Log(message, LogLevel.Debug);
    public static void Info(string message) => Log(message, LogLevel.Info);
    public static void Warning(string message) => Log(message, LogLevel.Warning);
    public static void Error(string message) => Log(message, LogLevel.Error);
    public static void Critical(string message) => Log(message, LogLevel.Critical);
}