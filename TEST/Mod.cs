using System;
using System.IO;
using System.Reflection;
using FFTInnateSkills.Configuration;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;

namespace FFTInnateSkills;

public class Mod : IMod
{
    private const string ModId = "rexx.fft.innateskills";
    private BattleEngine? _engine;
    private bool _started;

    public void Start(IModLoaderV1 modLoader) => StartMod();
    public void StartEx(IModLoaderV1 modLoader, IModConfigV1 modConfig) => StartMod();

    private void StartMod()
    {
        if (_started) return;
        _started = true;

        string modDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;
        string configPath = ResolveConfigPath(modDir);
        var cfg = Configurable<Config>.FromFile(configPath, "FFT Innate Skills Configuration");

        _engine = new BattleEngine(cfg);
        _engine.Start();
    }

    private static string ResolveConfigPath(string modDir)
    {
        try
        {
            var reloadedRoot = Directory.GetParent(modDir)?.Parent?.FullName;
            if (reloadedRoot != null)
            {
                var userConfig = Path.Combine(reloadedRoot, "User", "Mods", ModId, "Config.json");
                if (File.Exists(userConfig)) return userConfig;
            }
        }
        catch { }
        return Path.Combine(modDir, "Config.json");
    }

    public void Suspend() => _engine?.Stop();
    public void Resume() => _engine?.Start();
    public void Unload() => _engine?.Stop();
    public bool CanUnload() => false;
    public bool CanSuspend() => false;
    public Action Disposing { get; } = () => { };
}
