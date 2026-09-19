using System.Runtime.InteropServices;

namespace Map_Configs_GoldKingZ;

public sealed class Con
{
    public string Code { get; }
    private Con(string code) => Code = code;

    public override string ToString() => Code;
    public static implicit operator string(Con c) => c.Code;

    public static readonly Con Reset         = new("\x1b[0m");
    public static readonly Con Bold          = new("\x1b[1m");
    public static readonly Con Dim           = new("\x1b[2m");
    public static readonly Con Italic        = new("\x1b[3m");
    public static readonly Con Underline     = new("\x1b[4m");
    public static readonly Con Blink         = new("\x1b[5m");
    public static readonly Con Inverse       = new("\x1b[7m");
    public static readonly Con Strikethrough = new("\x1b[9m");

    public static Con Fg(int r, int g, int b) => new($"\x1b[38;2;{r};{g};{b}m");
    public static Con Bg(int r, int g, int b) => new($"\x1b[48;2;{r};{g};{b}m");

    public static readonly Con Black       = Fg(  0,   0,   0);
    public static readonly Con DarkGray    = Fg( 80,  80,  80);
    public static readonly Con Gray        = Fg(128, 128, 128);
    public static readonly Con LightGray   = Fg(192, 192, 192);
    public static readonly Con Silver      = Fg(220, 220, 220);
    public static readonly Con White       = Fg(255, 255, 255);

    public static readonly Con DarkRed     = Fg(139,   0,   0);
    public static readonly Con Red         = Fg(231,  72,  86);
    public static readonly Con LightRed    = Fg(255, 102, 102);
    public static readonly Con Crimson     = Fg(220,  20,  60);
    public static readonly Con Maroon      = Fg(128,   0,   0);
    public static readonly Con Salmon      = Fg(250, 128, 114);
    public static readonly Con Coral       = Fg(255, 127,  80);
    public static readonly Con Tomato      = Fg(255,  99,  71);
    public static readonly Con Brick       = Fg(178,  34,  34);

    public static readonly Con DarkOrange  = Fg(204,  85,   0);
    public static readonly Con Orange      = Fg(255, 165,   0);
    public static readonly Con LightOrange = Fg(255, 200, 100);
    public static readonly Con Amber       = Fg(255, 191,   0);
    public static readonly Con Peach       = Fg(255, 218, 185);
    public static readonly Con Tangerine   = Fg(242, 133,   0);

    public static readonly Con DarkYellow  = Fg(180, 160,  20);
    public static readonly Con Yellow      = Fg(249, 241, 165);
    public static readonly Con BrightYellow= Fg(255, 255,   0);
    public static readonly Con Gold        = Fg(255, 215,   0);
    public static readonly Con OrangeYellow= Fg(255, 195,   0);
    public static readonly Con Mustard     = Fg(255, 219,  88);
    public static readonly Con Khaki       = Fg(240, 230, 140);
    public static readonly Con Cream       = Fg(255, 253, 208);

    public static readonly Con DarkGreen   = Fg(  0, 100,   0);
    public static readonly Con Green       = Fg( 22, 198,  12);
    public static readonly Con LightGreen  = Fg(144, 238, 144);
    public static readonly Con Lime        = Fg(180, 255,   0);
    public static readonly Con Mint        = Fg(152, 255, 152);
    public static readonly Con Forest      = Fg( 34, 139,  34);
    public static readonly Con Olive       = Fg(128, 128,   0);
    public static readonly Con Emerald     = Fg( 80, 200, 120);
    public static readonly Con Sea         = Fg( 46, 139,  87);

    public static readonly Con DarkCyan    = Fg(  0, 139, 139);
    public static readonly Con Cyan        = Fg( 97, 214, 214);
    public static readonly Con LightCyan   = Fg(224, 255, 255);
    public static readonly Con Teal        = Fg(  0, 128, 128);
    public static readonly Con Turquoise   = Fg( 64, 224, 208);
    public static readonly Con Aqua        = Fg(  0, 255, 255);

    public static readonly Con DarkBlue    = Fg(  0,   0, 139);
    public static readonly Con Blue        = Fg( 59, 120, 255);
    public static readonly Con LightBlue   = Fg(173, 216, 230);
    public static readonly Con Navy        = Fg(  0,   0, 128);
    public static readonly Con Royal       = Fg( 65, 105, 225);
    public static readonly Con Sky         = Fg(135, 206, 235);
    public static readonly Con Steel       = Fg( 70, 130, 180);
    public static readonly Con Cobalt      = Fg(  0,  71, 171);
    public static readonly Con Cornflower  = Fg(100, 149, 237);

    public static readonly Con DarkMagenta = Fg(139,   0, 139);
    public static readonly Con Magenta     = Fg(180,   0, 158);
    public static readonly Con LightMagenta= Fg(255, 119, 255);
    public static readonly Con Purple      = Fg(128,   0, 128);
    public static readonly Con Violet      = Fg(143,   0, 255);
    public static readonly Con Indigo      = Fg( 75,   0, 130);
    public static readonly Con Lavender    = Fg(200, 162, 255);
    public static readonly Con Plum        = Fg(221, 160, 221);
    public static readonly Con Orchid      = Fg(218, 112, 214);

    public static readonly Con Pink        = Fg(255, 192, 203);
    public static readonly Con HotPink     = Fg(255, 105, 180);
    public static readonly Con DeepPink    = Fg(255,  20, 147);
    public static readonly Con Rose        = Fg(255,   0, 127);
    public static readonly Con Fuchsia     = Fg(255,   0, 255);

    public static readonly Con Brown       = Fg(139,  69,  19);
    public static readonly Con DarkBrown   = Fg( 92,  64,  51);
    public static readonly Con LightBrown  = Fg(181, 101,  29);
    public static readonly Con Tan         = Fg(210, 180, 140);
    public static readonly Con Beige       = Fg(245, 245, 220);
    public static readonly Con Sand        = Fg(194, 178, 128);
    public static readonly Con Chocolate   = Fg(210, 105,  30);
    public static readonly Con Sienna      = Fg(160,  82,  45);

    [DllImport("kernel32.dll")] private static extern bool GetConsoleMode(IntPtr h, out uint m);
    [DllImport("kernel32.dll")] private static extern bool SetConsoleMode(IntPtr h, uint m);
    [DllImport("kernel32.dll")] private static extern IntPtr GetStdHandle(int n);

    static Con()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;

        var h = GetStdHandle(-11);
        if (GetConsoleMode(h, out uint mode))
            SetConsoleMode(h, mode | 0x0004);
    }

    public static void ResetAll()
    {
        Console.Write(Reset.Code);
        Console.ResetColor();
    }

    public static void WriteLine(string text)
    {
        Console.WriteLine(text + Reset.Code);
        Console.ResetColor();
    }
}
