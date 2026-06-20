using Kodnix.Character;

namespace NahidaImpact.Util;

public class ConsoleManager
{
    public static readonly string PrefixContent = "[NahidaImpact]> ";
    public static readonly string Prefix = $"\u001b[38;2;255;192;203m{PrefixContent}\u001b[0m";
    private static readonly int HistoryMaxCount = 10;

    public static List<char> Input { get; set; } = [];
    private static int CursorIndex { get; set; } = 0;
    private static readonly List<string> InputHistory = [];
    private static int HistoryIndex = -1;

    /// <summary>True when stdin/stdout are a real terminal; false when redirected (headless / piped / service).</summary>
    public static bool Interactive { get; private set; }

    public static event Action<string>? OnConsoleExecuteCommand;

    public static void InitConsole()
    {
        // Cursor-driven input line only works on a real terminal. When stdout/stdin are
        // redirected (background, piped, service, headless), fall back to plain line I/O.
        Interactive = !Console.IsOutputRedirected && !Console.IsInputRedirected;

        try
        {
            Console.Title = ConfigManager.Config.GameServer.GameServerName;
        }
        catch
        {
            // No real console attached (output redirected / headless) — title unavailable.
        }
    }

    public static int GetWidth(string str)
        => str.ToCharArray().Sum(EastAsianWidth.GetLength);

    public static void RedrawInput(List<char> input, bool hasPrefix = true)
        => RedrawInput(new string([.. input]), hasPrefix);

    public static void RedrawInput(string input, bool hasPrefix = true)
    {
        if (!Interactive) return;

        var length = GetWidth(input);
        if (hasPrefix)
        {
            input = Prefix + input;
            length += GetWidth(PrefixContent);
        }

        if (Console.GetCursorPosition().Left > 0)
            Console.SetCursorPosition(0, Console.CursorTop);

        Console.Write(input + new string(' ', Console.BufferWidth - length));
        Console.SetCursorPosition(length, Console.CursorTop);
    }

    #region Handlers

    public static void HandleEnter()
    {
        var input = new string([.. Input]);
        if (string.IsNullOrWhiteSpace(input)) return;

        Console.WriteLine();
        Input = [];
        CursorIndex = 0;
        if (InputHistory.Count >= HistoryMaxCount) 
            InputHistory.RemoveAt(0);
        InputHistory.Add(input);
        HistoryIndex = InputHistory.Count;

        Dispatch(input);
    }

    private static void Dispatch(string input)
    {
        if (input.StartsWith('/')) input = input[1..].Trim();
        OnConsoleExecuteCommand?.Invoke(input);
    }

    public static void HandleBackspace()
    {
        if (CursorIndex <= 0) return;
        CursorIndex--;
        var targetWidth = GetWidth(Input[CursorIndex].ToString());
        Input.RemoveAt(CursorIndex);

        var (left, _) = Console.GetCursorPosition();
        Console.SetCursorPosition(left - targetWidth, Console.CursorTop);
        var remain = new string([.. Input.Skip(CursorIndex)]);
        Console.Write(remain + new string(' ', targetWidth));
        Console.SetCursorPosition(left - targetWidth, Console.CursorTop);
    }

    public static void HandleUpArrow()
    {
        if (InputHistory.Count == 0) return;
        
        if (HistoryIndex > 0)
        {
            HistoryIndex--;
            var history = InputHistory[HistoryIndex];
            Input = [.. history];
            CursorIndex = Input.Count;
            RedrawInput(Input);
        }
    }

    public static void HandleDownArrow()
    {
        if (HistoryIndex >= InputHistory.Count) return;
        
        HistoryIndex++;
        if (HistoryIndex >= InputHistory.Count)
        {
            HistoryIndex = InputHistory.Count;
            Input = [];
            CursorIndex = 0;
        }
        else 
        {
            var history = InputHistory[HistoryIndex];
            Input = [.. history];
            CursorIndex = Input.Count;
        }
        RedrawInput(Input);
    }

    public static void HandleLeftArrow()
    {
        if (CursorIndex <= 0) return;
        
        var (left, _) = Console.GetCursorPosition();
        CursorIndex--;
        Console.SetCursorPosition(left - GetWidth(Input[CursorIndex].ToString()), Console.CursorTop);
    }

    public static void HandleRightArrow()
    {
        if (CursorIndex >= Input.Count) return;
        
        var (left, _) = Console.GetCursorPosition();
        CursorIndex++;
        Console.SetCursorPosition(left + GetWidth(Input[CursorIndex - 1].ToString()), Console.CursorTop);
    }

    public static void HandleInput(ConsoleKeyInfo keyInfo)
    {
        if (char.IsControl(keyInfo.KeyChar)) return;
        if (Input.Count >= (Console.BufferWidth - PrefixContent.Length)) return;
        HandleInput(keyInfo.KeyChar);
    }

    public static void HandleInput(char keyChar)
    {
        Input.Insert(CursorIndex, keyChar);
        CursorIndex++;

        var (left, _) = Console.GetCursorPosition();
        Console.Write(new string([.. Input.Skip(CursorIndex - 1)]));
        Console.SetCursorPosition(left + GetWidth(keyChar.ToString()), Console.CursorTop);
    }

    #endregion

    public static void ListenConsole()
    {
        if (!Interactive)
        {
            ListenNonInteractive();
            return;
        }

        while (true)
        {
            ConsoleKeyInfo keyInfo;
            try { keyInfo = Console.ReadKey(true); }
            catch (InvalidOperationException)
            {
                // Console became unavailable mid-run: fall back instead of busy-looping.
                ListenNonInteractive();
                return;
            }

            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    HandleEnter();
                    break;
                case ConsoleKey.Backspace:
                    HandleBackspace();
                    break;
                case ConsoleKey.LeftArrow:
                    HandleLeftArrow();
                    break;
                case ConsoleKey.RightArrow:
                    HandleRightArrow();
                    break;
                case ConsoleKey.UpArrow:
                    HandleUpArrow();
                    break;
                case ConsoleKey.DownArrow:
                    HandleDownArrow();
                    break;
                default:
                    HandleInput(keyInfo);
                    break;
            }
        }
    }

    /// <summary>
    /// Headless / redirected mode: read whole lines instead of driving the cursor, then keep
    /// the process alive so the network threads keep serving even when there is no stdin.
    /// </summary>
    private static void ListenNonInteractive()
    {
        while (true)
        {
            string? line;
            try { line = Console.ReadLine(); }
            catch { break; }
            if (line == null) break; // stdin closed / EOF

            if (string.IsNullOrWhiteSpace(line)) continue;
            Dispatch(line.Trim());
        }

        // No more console input — block here so Main() doesn't return and kill the server.
        Thread.Sleep(Timeout.Infinite);
    }
}