using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace Map_Configs_GoldKingZ;

public class CustomGameData
{
    private const string GithubUrl = "https://raw.githubusercontent.com/oqyh/cs2-Private-Plugins/main/Resources/gamedata.json";

    private static CustomGameData? _instance;
    public static CustomGameData? Instance => _instance;
    public static bool IsReady => _instance?._isLoaded == true;
    public static bool RestartAfterHook { get; set; } = true;
    private sealed record Reg(string Key, Func<BaseMemoryFunction?> Factory, Func<DynamicHook, HookResult> Cb, HookMode Mode, bool NeedsGameData);
    private static readonly List<Reg> _registered = new();
    private static readonly List<(Reg Reg, BaseMemoryFunction Fn)> _active = new();

    private readonly Dictionary<string, string> _signatures = new();
    private readonly Dictionary<string, string> _libraries  = new();
    private readonly Dictionary<string, int>    _offsets    = new();
    private readonly Dictionary<string, string> _patches    = new();
    private bool _isLoaded;

    private const string UserAgent = "CS2-Map-Configs";

    private static readonly TimeSpan   _timeout_Github    = TimeSpan.FromSeconds(50);
    private static readonly HttpClient _httpClient_Github = new() { Timeout = _timeout_Github };
    private static CancellationTokenSource? _cts;
    private static int  _generation;
    private static bool _applied;
    private static bool _busy;

    private static string GamedataPath => Path.Combine(MainPlugin.Instance.ModuleDirectory, "gamedata", "gamedata.json");

    private static string? GetModulePath(string library) => library.ToLowerInvariant() switch
    {
        "host"                   => Path.Join(Server.GameDirectory, Constants.GameBinaryPath, $"{Constants.ModulePrefix}host{Constants.ModuleSuffix}"),
        "matchmaking"            => Path.Join(Server.GameDirectory, Constants.GameBinaryPath, $"{Constants.ModulePrefix}matchmaking{Constants.ModuleSuffix}"),
        "server"                 => Path.Join(Server.GameDirectory, Constants.GameBinaryPath, $"{Constants.ModulePrefix}server{Constants.ModuleSuffix}"),

        "animationsystem"        => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}animationsystem{Constants.ModuleSuffix}"),
        "avcodec"                => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}avcodec{Constants.ModuleSuffix}"),
        "avformat"               => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}avformat{Constants.ModuleSuffix}"),
        "avresample"             => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}avresample{Constants.ModuleSuffix}"),
        "avutil"                 => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}avutil{Constants.ModuleSuffix}"),
        "cairo"                  => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}cairo{Constants.ModuleSuffix}"),
        "engine2"                => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}engine2{Constants.ModuleSuffix}"),
        "filesystem_stdio"       => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}filesystem_stdio{Constants.ModuleSuffix}"),
        "fontconfig"             => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}fontconfig{Constants.ModuleSuffix}"),
        "freetype"               => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}freetype{Constants.ModuleSuffix}"),
        "inputsystem"            => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}inputsystem{Constants.ModuleSuffix}"),
        "localize"               => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}localize{Constants.ModuleSuffix}"),
        "materialsystem2"        => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}materialsystem2{Constants.ModuleSuffix}"),
        "meshsystem"             => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}meshsystem{Constants.ModuleSuffix}"),
        "mpg123"                 => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}mpg123{Constants.ModuleSuffix}"),
        "networksystem"          => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}networksystem{Constants.ModuleSuffix}"),
        "ogg"                    => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}ogg{Constants.ModuleSuffix}"),
        "pango-1.0"              => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}pango-1.0{Constants.ModuleSuffix}"),
        "pangoft2-1.0"           => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}pangoft2-1.0{Constants.ModuleSuffix}"),
        "panorama"               => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}panorama{Constants.ModuleSuffix}"),
        "panorama_text_pango"    => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}panorama_text_pango{Constants.ModuleSuffix}"),
        "panoramauiclient"       => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}panoramauiclient{Constants.ModuleSuffix}"),
        "particles"              => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}particles{Constants.ModuleSuffix}"),
        "phonon"                 => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}phonon{Constants.ModuleSuffix}"),
        "pulse_system"           => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}pulse_system{Constants.ModuleSuffix}"),
        "rendersystemempty"      => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}rendersystemempty{Constants.ModuleSuffix}"),
        "rendersystemvulkan"     => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}rendersystemvulkan{Constants.ModuleSuffix}"),
        "resourcesystem"         => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}resourcesystem{Constants.ModuleSuffix}"),
        "scenefilecache"         => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}scenefilecache{Constants.ModuleSuffix}"),
        "scenesystem"            => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}scenesystem{Constants.ModuleSuffix}"),
        "schemasystem"           => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}schemasystem{Constants.ModuleSuffix}"),
        "sdl3"                   => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}SDL3{Constants.ModuleSuffix}"),
        "soundsystem"            => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}soundsystem{Constants.ModuleSuffix}"),
        "steam_api"              => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}steam_api{Constants.ModuleSuffix}"),
        "steamaudio"             => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}steamaudio{Constants.ModuleSuffix}"),
        "steamnetworkingsockets" => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}steamnetworkingsockets{Constants.ModuleSuffix}"),
        "swscale"                => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}swscale{Constants.ModuleSuffix}"),
        "tier0"                  => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}tier0{Constants.ModuleSuffix}"),
        "v8"                     => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}v8{Constants.ModuleSuffix}"),
        "v8_icui18n"             => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}v8_icui18n{Constants.ModuleSuffix}"),
        "v8_icuuc"               => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}v8_icuuc{Constants.ModuleSuffix}"),
        "v8_libbase"             => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}v8_libbase{Constants.ModuleSuffix}"),
        "v8_libcpp"              => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}v8_libcpp{Constants.ModuleSuffix}"),
        "v8_libplatform"         => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}v8_libplatform{Constants.ModuleSuffix}"),
        "v8_zlib"                => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}v8_zlib{Constants.ModuleSuffix}"),
        "v8system"               => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}v8system{Constants.ModuleSuffix}"),
        "vconcomm"               => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}vconcomm{Constants.ModuleSuffix}"),
        "video"                  => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}video{Constants.ModuleSuffix}"),
        "vorbis"                 => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}vorbis{Constants.ModuleSuffix}"),
        "vorbisenc"              => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}vorbisenc{Constants.ModuleSuffix}"),
        "vorbisfile"             => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}vorbisfile{Constants.ModuleSuffix}"),
        "vphysics2"              => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}vphysics2{Constants.ModuleSuffix}"),
        "vpx"                    => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}vpx{Constants.ModuleSuffix}"),
        "vscript"                => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}vscript{Constants.ModuleSuffix}"),
        "worldrenderer"          => Path.Join(Server.GameDirectory, Constants.RootBinaryPath, $"{Constants.ModulePrefix}worldrenderer{Constants.ModuleSuffix}"),
        _                        => null
    };

    public static void HookSignature<T>(string key, Func<DynamicHook, HookResult> cb, HookMode mode)
        where T : BaseMemoryFunction
        => Register(key, () => _instance?.CreateFunction<T>(key), cb, mode, true);

    public static void HookOffset<T>(string key, IntPtr objectPtr, Func<DynamicHook, HookResult> cb, HookMode mode)
        where T : BaseMemoryFunction
        => Register(key, () => _instance?.CreateOffsetFunction<T>(key, objectPtr), cb, mode, true);

    public static void HookSignatureInline<T>(string label, string signature, Func<DynamicHook, HookResult> cb, HookMode mode, string library = "server")
        where T : BaseMemoryFunction
        => Register(label, () => CreateFrom<T>(signature, library), cb, mode, false);

    public static void HookOffsetInline<T>(string label, IntPtr objectPtr, int offset, Func<DynamicHook, HookResult> cb, HookMode mode)
        where T : BaseMemoryFunction
        => Register(label, () => objectPtr == IntPtr.Zero ? null : (T)Activator.CreateInstance(typeof(T), objectPtr, offset)!, cb, mode, false);

    public static void UnhookAll()
    {
        Interlocked.Increment(ref _generation);

        try { _cts?.Cancel(); _cts?.Dispose(); } catch { }
        _cts = null;

        Detach();
        _registered.Clear();

        _instance = null;
        _applied  = false;
        _busy     = false;
    }

    private static void Register(string key, Func<BaseMemoryFunction?> factory, Func<DynamicHook, HookResult> cb, HookMode mode, bool needsGameData)
    {
        _registered.RemoveAll(r => r.Key == key);

        var reg = new Reg(key, factory, cb, mode, needsGameData);
        _registered.Add(reg);

        if (_applied || !needsGameData)
        {
            Attach(reg);
            return;
        }

        Helper.Debug($"{key} Registered, Waiting For GameData");
        EnsureLoaded();
    }

    private static void EnsureLoaded()
    {
        if (_busy || _applied) return;

        string path = GamedataPath;

        if (File.Exists(path) && !Configs.Instance.AutoUpdateSignatures)
        {
            Helper.Debug("AutoUpdateSignatures Off, Using Local gamedata.json");
            Apply(false);
            return;
        }

        _busy = true;

        var cts = new CancellationTokenSource(_timeout_Github);
        _cts = cts;

        int gen = Volatile.Read(ref _generation);

        _ = Task.Run(async () =>
        {
            bool updated = false;

            try
            {
                updated = await Start_DownloadMissingFiles(cts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                DebugSafe("gamedata.json Download Cancelled", true);
            }
            catch (Exception ex)
            {
                DebugSafe($"gamedata.json Download Failed: {ex.Message}", true);
            }

            await Server.NextWorldUpdateAsync(() =>
            {
                if (Volatile.Read(ref _generation) != gen) return;

                _busy = false;
                Apply(updated);
            }).ConfigureAwait(false);
        });
    }

    private static void Apply(bool restart)
    {
        var data = new CustomGameData();
        data.ParseJson();

        if (!data._isLoaded)
        {
            Helper.Debug($"GameData Not Available, {_registered.Count} Hook(s) Not Applied", true);
            return;
        }

        _instance = data;
        _applied  = true;
        Helper.Debug("GameData Loaded");

        Detach();
        foreach (var reg in _registered) Attach(reg);

        if (restart)               QueueRestart("GameData Updated, Applying New Signatures");
        else if (RestartAfterHook) QueueRestart("Hooks Applied, StartupServer Needs A Fresh Map To Take Effect");
    }

    public static void QueueRestart(string reason)
    {
        if (string.IsNullOrWhiteSpace(Server.MapName))
        {
            Helper.Debug($"{reason}, No Map Loaded Yet, Hooks Will Take Effect On First Map");
            return;
        }
        Helper.Debug($"{reason}, Restarting Map Now");
        Helper.RestartMap();
    }

    private static async Task<bool> Start_DownloadMissingFiles(CancellationToken token)
    {
        string localPath_gamedata = "gamedata/gamedata.json";
        string githubUrl_gamedata = GithubUrl;

        return await DownloadFromGitHub(localPath_gamedata, githubUrl_gamedata, Configs.Instance.AutoUpdateSignatures, token).ConfigureAwait(false);
    }

    public static async Task<bool> DownloadFromGitHub(string filePath, string githubUrl, bool AutoUpdate = false, CancellationToken token = default)
    {
        try
        {
            string fullPath = Path.Combine(MainPlugin.Instance.ModuleDirectory, filePath);
            string name     = Path.GetFileName(fullPath);

            string? dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            _httpClient_Github.DefaultRequestHeaders.Remove("User-Agent");
            _httpClient_Github.DefaultRequestHeaders.Add("User-Agent", UserAgent);

            string actualDownloadUrl = githubUrl;

            if (githubUrl.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                using var ctsTxt = Linked(token);

                using var txtResponse = await _httpClient_Github.GetAsync(githubUrl, ctsTxt.Token).ConfigureAwait(false);
                txtResponse.EnsureSuccessStatusCode();
                actualDownloadUrl = (await txtResponse.Content.ReadAsStringAsync(ctsTxt.Token).ConfigureAwait(false)).Trim();
            }

            using var ctsBytes = Linked(token);

            using var bytesResponse = await _httpClient_Github.GetAsync(actualDownloadUrl, ctsBytes.Token).ConfigureAwait(false);
            bytesResponse.EnsureSuccessStatusCode();
            byte[] remoteBytes = await bytesResponse.Content.ReadAsByteArrayAsync(ctsBytes.Token).ConfigureAwait(false);

            if (remoteBytes.Length == 0)
            {
                DebugSafe($"{name} Download Empty, Ignored", true);
                return false;
            }

            bool needDownload = !File.Exists(fullPath);

            if (!needDownload && AutoUpdate)
            {
                byte[] localBytes = await File.ReadAllBytesAsync(fullPath, ctsBytes.Token).ConfigureAwait(false);
                needDownload = !SHA256.HashData(localBytes).AsSpan().SequenceEqual(SHA256.HashData(remoteBytes));
            }

            if (!needDownload)
            {
                DebugSafe($"{name} Up To Date");
                return false;
            }

            await File.WriteAllBytesAsync(fullPath, remoteBytes, ctsBytes.Token).ConfigureAwait(false);
            DebugSafe($"{name} Updated ({remoteBytes.Length} Bytes)");
            return true;
        }
        catch (Exception ex)
        {
            DebugSafe($"DownloadFromGitHub Error: {ex.Message}", true);
            return false;
        }
    }

    private static CancellationTokenSource Linked(CancellationToken token)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        cts.CancelAfter(_timeout_Github);
        return cts;
    }

    private static void DebugSafe(string message, bool error = false)
    {
        int gen = Volatile.Read(ref _generation);

        Server.NextWorldUpdate(() =>
        {
            if (Volatile.Read(ref _generation) == gen) Helper.Debug(message, error);
        });
    }

    private static void Attach(Reg reg)
    {
        BaseMemoryFunction? fn;

        try
        {
            fn = reg.Factory();
        }
        catch (Exception ex)
        {
            Helper.Debug($"{reg.Key} Create Error: {ex.Message}", true);
            return;
        }

        if (fn == null || fn.Handle == IntPtr.Zero)
        {
            Helper.Debug($"{reg.Key} Function Not Created", true);
            return;
        }

        try
        {
            fn.Hook(reg.Cb, reg.Mode);
            _active.Add((reg, fn));
            Helper.Debug($"{reg.Key} Hooked ({reg.Mode})");
        }
        catch (Exception ex)
        {
            Helper.Debug($"{reg.Key} Hook Error: {ex.Message}", true);
        }
    }

    private static void Detach()
    {
        foreach (var (reg, fn) in _active)
        {
            try
            {
                fn.Unhook(reg.Cb, reg.Mode);
                Helper.Debug($"{reg.Key} Unhooked");
            }
            catch (Exception ex)
            {
                Helper.Debug($"{reg.Key} Unhook Error: {ex.Message}", true);
            }
        }

        _active.Clear();
    }

    public string GetSignature(string key)  => _signatures.GetValueOrDefault(key, string.Empty);
    public int    GetOffset(string key)     => _offsets.TryGetValue(key, out var v) ? v : -1;
    public string GetPatchBytes(string key) => _patches.GetValueOrDefault(key, string.Empty);
    public string GetLibrary(string key)    => _libraries.GetValueOrDefault(key, "server");

    private static T? CreateFrom<T>(string signature, string library) where T : class
    {
        if (string.IsNullOrWhiteSpace(signature)) return null;

        var module = GetModulePath(library);

        return module != null
            ? (T)Activator.CreateInstance(typeof(T), signature, module)!
            : (T)Activator.CreateInstance(typeof(T), signature)!;
    }

    public T? CreateFunction<T>(string key) where T : class
    {
        string sig = GetSignature(key);

        if (string.IsNullOrEmpty(sig))
        {
            Helper.Debug($"{key} Signature Missing", true);
            return null;
        }

        return CreateFrom<T>(sig, GetLibrary(key));
    }

    public T? CreateOffsetFunction<T>(string key, IntPtr objectPtr) where T : class
    {
        int offset = GetOffset(key);

        if (offset < 0)
        {
            Helper.Debug($"{key} Offset Missing", true);
            return null;
        }

        if (objectPtr == IntPtr.Zero)
        {
            Helper.Debug($"{key} Object Pointer Is Null", true);
            return null;
        }

        return (T)Activator.CreateInstance(typeof(T), objectPtr, offset)!;
    }

    private void ParseJson()
    {
        string path = GamedataPath;

        if (!File.Exists(path))
        {
            Helper.Debug("gamedata.json Not Found", true);
            return;
        }

        try
        {
            var json = JObject.Parse(File.ReadAllText(path));
            string platform = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" : "windows";

            foreach (var item in json.Properties())
            {
                string key  = item.Name;
                var    data = item.Value;

                if (data["signatures"]?[platform] is { } sig) _signatures[key] = sig.ToString();
                if (data["offsets"]?[platform]    is { } off) _offsets[key]    = off.Value<int>();
                if (data["patches"]?[platform]    is { } pat) _patches[key]    = pat.ToString();

                _libraries[key] = data["signatures"]?["library"]?.ToString() ?? "server";
            }

            _isLoaded = true;
        }
        catch (Exception ex)
        {
            Helper.Debug($"ParseJson Error: {ex.Message}", true);
        }
    }
}