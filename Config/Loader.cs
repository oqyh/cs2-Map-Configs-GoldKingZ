using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Map_Configs_GoldKingZ;

public partial class Configs
{
    public static string Version => $"Version : {MainPlugin.Instance?.ModuleVersion ?? "Unknown"}";
    public static string Github = "https://github.com/oqyh/cs2-Map-Configs-GoldKingZ";
    public static Configs Instance { get; private set; } = new();
    static string _file = "";
    static readonly JsonSerializerOptions Opts = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    static readonly JsonSerializerOptions Pretty = new(Opts) { WriteIndented = true };

    public static void Load(bool reload = false)
    {
        _file = Path.Combine(MainPlugin.Instance.ModuleDirectory, "config", "config.json");
        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(_file)!);

        var existed = File.Exists(_file);

        Instance = new Configs();
        Walk(Instance, ReadFile(), "");
        Save();

        if (reload) Warn($"config.json {(existed ? "Reloaded" : "Created")}");
    }

    public static void Save()
    {
        try { File.WriteAllText(_file, "{\n" + Render(Instance, 2) + "\n}\n"); }
        catch (Exception e) { Helper.Debug($"Cant Save config.json ({e.Message})", true); }
    }

    static void Warn(string message) => Helper.Debug(message, true);

    static IEnumerable<PropertyInfo> Props(object obj) => obj.GetType()
        .GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && p.CanWrite);

    static JsonObject? ReadFile()
    {
        if (!File.Exists(_file)) return null;

        string text;
        try { text = File.ReadAllText(_file); } catch { return null; }

        var noComments = string.Join("\n", text.Split('\n').Where(l => !l.TrimStart().StartsWith("//")));
        var commas = Regex.Replace(noComments, @"([}\]""\d]|true|false|null)(\s*\r?\n\s*)([""{\[])", "$1,$2$3");

        foreach (var attempt in new[] { text, noComments, commas })
        {
            try
            {
                if (JsonNode.Parse(attempt, documentOptions: new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true }) is not JsonObject obj) continue;

                if (attempt == commas && commas != noComments) Warn("config.json Had A Wrong Format (Missing \",\"), Auto Fixed, Your Values Are Kept");
                return obj;
            }
            catch { }
        }

        Warn("config.json Had A Wrong Format, Auto Fixed, Every Readable Setting Is Kept");
        return Salvage(noComments);
    }

    static JsonObject Salvage(string text)
    {
        var known = new HashSet<string>(typeof(Configs).GetProperties().Select(p => p.Name), StringComparer.OrdinalIgnoreCase);
        var obj = new JsonObject();
        var i = 0;

        while (i < text.Length)
        {
            if (text[i] != '"') { i++; continue; }

            var keyEnd = StringEnd(text, i);
            var key = text[(i + 1)..(keyEnd - 1)];
            var j = keyEnd;

            while (j < text.Length && char.IsWhiteSpace(text[j])) j++;

            if (!known.Contains(key) || j >= text.Length || text[j] != ':') { i = keyEnd; continue; }

            j++;
            while (j < text.Length && char.IsWhiteSpace(text[j])) j++;

            JsonNode? node = null;

            try { node = JsonNode.Parse(text[j..ValueEnd(text, j)].Trim()); } catch { }

            if (node != null) obj.TryAdd(key, node);
            else Warn($"\"{key}\" Is Not Readable, Default Value Will Be Used");

            i = j;
        }

        return obj;
    }

    static int StringEnd(string t, int i)
    {
        for (var j = i + 1; j < t.Length; j++)
            if (t[j] == '\\') j++;
            else if (t[j] == '"') return j + 1;

        return t.Length;
    }

    static int ValueEnd(string t, int i)
    {
        if (t[i] == '"') return StringEnd(t, i);

        if (t[i] is not ('{' or '['))
        {
            var k = i;
            while (k < t.Length && t[k] is not (',' or '\n' or '}' or ']')) k++;
            return k;
        }

        for (int j = i, depth = 0; j < t.Length; j++)
        {
            if (t[j] == '"') j = StringEnd(t, j) - 1;
            else if (t[j] is '{' or '[') depth++;
            else if (t[j] is '}' or ']' && --depth == 0) return j + 1;
        }

        return t.Length;
    }

    static void Walk(object? target, JsonObject? json, string path)
    {
        if (target == null || target is string) return;
        if (target is System.Collections.IDictionary map) { foreach (var v in map.Values) Walk(v, null, path); return; }
        if (target is System.Collections.IEnumerable list) { foreach (var v in list) Walk(v, null, path); return; }
        if (!target.GetType().IsClass) return;

        object? defaults = null;

        foreach (var p in Props(target))
        {
            var name = path + p.Name;
            var type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
            var node = json?.FirstOrDefault(kv => string.Equals(kv.Key, p.Name, StringComparison.OrdinalIgnoreCase)).Value;

            if (node is JsonObject child && p.GetValue(target) is { } nested && type.IsClass && type != typeof(string) && nested is not System.Collections.IEnumerable)
            {
                Walk(nested, child, name + " -> ");
            }
            else if (node != null)
            {
                try { if (node.Deserialize(p.PropertyType, Opts) is { } value) p.SetValue(target, value); }
                catch { Warn($"\"{name}\": {node.ToJsonString()} Is Not Valid, Kept Default ({p.GetValue(target)})"); }
            }

            var val = p.GetValue(target);

            if (p.GetCustomAttribute<StringAttribute>() is { } str && type == typeof(string))
            {
                defaults ??= Activator.CreateInstance(target.GetType());
                p.SetValue(target, FixKeys(val as string, str.Keys, p.Name, defaults == null ? null : p.GetValue(defaults) as string));
            }
            else if (p.GetCustomAttribute<RangeAttribute>() is { } range && val is IConvertible num)
            {
                var d = num.ToDouble(null);
                if (d < range.Min || d > range.Max)
                {
                    defaults ??= Activator.CreateInstance(target.GetType());

                    var back = defaults == null ? Convert.ChangeType(Math.Clamp(d, range.Min, range.Max), type) : p.GetValue(defaults);
                    p.SetValue(target, back);

                    Warn($"\"{name}\": {d} Is Not Valid, Changed To Default ({back}) (Allowed {range.Min} To {range.Max})");

                    foreach (var c in p.GetCustomAttributes<CommentAttribute>().Where(c => Regex.IsMatch(c.Text.Trim(), @"^-?\d+\s*=")))
                        Warn("   " + c.Text.Trim());
                }
            }
            else if (node == null || val is System.Collections.IEnumerable) Walk(val, null, name + " -> ");
        }
    }

    static string Render(object obj, int indent)
    {
        var pad = new string(' ', indent);
        var blocks = new List<(string Text, bool IsProp)>();

        foreach (var p in Props(obj))
        {
            var infos = p.GetCustomAttributes<InfoAttribute>().ToList();
            var lines = new List<string>();

            if (p.GetCustomAttribute<BreakLineAttribute>() is { } br) lines.AddRange(Comments(br.Text, pad));
            foreach (var i in infos) lines.AddRange(Comments(i.Key switch { "Version" => Version, "Github" => Github, _ => i.Key }, pad));
            foreach (var c in p.GetCustomAttributes<CommentAttribute>()) lines.AddRange(Comments(c.Text, pad));

            var val = infos.Count > 0 ? null : p.GetValue(obj);
            if (val != null) lines.Add($"{pad}\"{p.Name}\": {Value(val, indent)}");

            if (lines.Count > 0) blocks.Add((string.Join("\n", lines), val != null));
        }

        var last = blocks.FindLastIndex(b => b.IsProp);
        return string.Join("\n\n", blocks.Select((b, i) => b.IsProp && i < last ? b.Text + "," : b.Text));
    }

    static string Value(object val, int indent)
    {
        var pad = new string(' ', indent);

        if (val is not string && val is not System.Collections.IEnumerable && val.GetType().IsClass)
            return $"\n{pad}{{\n{Render(val, indent + 2)}\n{pad}}}";

        var json = JsonSerializer.Serialize(val, Pretty).Split('\n');
        return string.Join("\n", json.Select((l, i) => (i == 0 ? "" : pad) + l.TrimEnd()));
    }

    static IEnumerable<string> Comments(string? text, string pad)
    {
        if (string.IsNullOrWhiteSpace(text)) yield break;

        foreach (var raw in text.Replace("\r", "").Split('\n'))
        {
            var t = raw.Trim();
            var before = t.StartsWith("{nextline}");
            var after = t.EndsWith("{nextline}");

            t = t.Replace("{nextline}", "").Trim();

            if (t.Length == 0) { yield return before || after ? "" : pad + "//"; continue; }

            if (before) yield return "";
            yield return pad + "// " + t;
            if (after) yield return "";
        }
    }

    public static string GetStringValue(string? input, string key) =>
        Split(input, new[] { key }).FirstOrDefault(s => string.Equals(s.Key, key, StringComparison.OrdinalIgnoreCase)).Val?.Trim() ?? "";

    public static string FixString(string? value, Format format, string name = "Value") => FixKeys(value, Formats.Get(format), name);

    public static string FixKeys(string? current, string[] keys, string name = "Value", string? fallback = null)
    {
        var segs = Split(current, keys);
        var used = segs.Any(s => s.Has);

        if (!used)
        {
            var empty = fallback ?? string.Join(" | ", keys.Select(k => $"{k}: "));

            if (empty != (current ?? ""))
            {
                Warn($"\"{name}\": \"{current}\" Is Empty, Changed To Default:");
                Warn($"   \"{empty}\"");
            }

            return empty;
        }

        var parts = keys.Select((key, i) =>
        {
            var hit = segs.FindIndex(s => string.Equals(s.Key, key, StringComparison.OrdinalIgnoreCase));
            if (hit >= 0) return $"{key}: {segs[hit].Val}";

            var renamed = i < segs.Count && segs[i].Has && !keys.Any(k => string.Equals(segs[i].Key, k, StringComparison.OrdinalIgnoreCase));
            return $"{key}: {(renamed ? segs[i].Val : "")}";
        });

        var result = string.Join(" | ", parts);
        if (used && result != (current ?? ""))
        {
            Warn($"\"{name}\": \"{current}\" Is Not Valid, Changed To:");
            Warn($"   \"{result}\"");
        }
        return result;
    }

    static List<(string Key, string Val, bool Has)> Split(string? input, string[] keys) =>
        Regex.Split(input ?? "", @"\s*\|\s*|\s*(?=\b(?:" + string.Join("|", keys.Select(Regex.Escape)) + @")\s*:)", RegexOptions.IgnoreCase)
             .Where(s => s.Length > 0)
             .Select(s =>
             {
                 var i = s.IndexOf(':');
                 if (i < 0) return ("", s.Trim(), false);

                 var val = s[(i + 1)..].TrimEnd();
                 return (s[..i].Trim(), val.StartsWith(" ") ? val[1..] : val, true);
             }).ToList();
}
