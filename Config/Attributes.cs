namespace Map_Configs_GoldKingZ;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)] public class CommentAttribute : Attribute { public string Text; public CommentAttribute(string t) => Text = t; }
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)] public class InfoAttribute : Attribute { public string Key; public InfoAttribute(string k) => Key = k; }
[AttributeUsage(AttributeTargets.Property)] public class BreakLineAttribute : Attribute { public string Text; public BreakLineAttribute(string t) => Text = t; }
[AttributeUsage(AttributeTargets.Property)] public class RangeAttribute : Attribute { public double Min, Max; public RangeAttribute(double min, double max) { Min = min; Max = max; } }

public enum Format { Flag, Command }

public static class Formats
{
    public static string[] Get(Format f) => f == Format.Flag
        ? new[] { "SteamIDs", "Flags", "Groups" }
        : new[] { "Console_Commands", "Chat_Commands" };
}

[AttributeUsage(AttributeTargets.Property)]
public class StringAttribute : Attribute
{
    public string[] Keys;
    public StringAttribute(params string[] keys) => Keys = keys;
    public StringAttribute(Format format) => Keys = Formats.Get(format);
}
