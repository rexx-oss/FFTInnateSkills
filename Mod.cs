using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using FFTInnateSkills.Configuration;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;

namespace FFTInnateSkills;

public class Mod : IMod
{
    private const string ModId = "rexx.fft.innateskills";
    private int _grantArmed;

    static Mod()
    {
        AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
        {
            var requested = new AssemblyName(args.Name);
            if (string.Equals(requested.Name, "fftivc.utility.modloader.Interfaces", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (string.Equals(asm.GetName().Name, "fftivc.utility.modloader.Interfaces", StringComparison.OrdinalIgnoreCase))
                        return asm;
                }

                try
                {
                    string modDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
                    string modsRoot = Directory.GetParent(modDir)?.FullName ?? "";
                    string fallbackPath = Path.Combine(modsRoot, "fftivc.utility.modloader", "fftivc.utility.modloader.Interfaces.dll");
                    if (File.Exists(fallbackPath)) return Assembly.LoadFrom(fallbackPath);
                }
                catch { }
            }
            return null;
        };
    }

    public void Start(IModLoaderV1 modLoader) => Init(modLoader);
    public void StartEx(IModLoaderV1 modLoader, IModConfigV1 modConfig) => Init(modLoader);

    private void Init(IModLoaderV1 modLoader)
    {
        if (Interlocked.Exchange(ref _grantArmed, 1) != 0) return;

        string modDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;
        string configPath = ResolveConfigPath(modDir);
        var cfg = Configurable<Config>.FromFile(configPath, "FFT Innate Skills Configuration");

        modLoader.OnModLoaderInitialized += () =>
        {
            var t = new Thread(() => ApplyGrants(modLoader, cfg)) { IsBackground = true, Name = "FFTInnateSkills.Grant" };
            t.Start();
        };
    }

    private static void ApplyGrants(IModLoaderV1 modLoader, Config cfg)
    {
        FftivcJobTable? table = null;

        for (int i = 0; i < 100; i++)
        {
            table = FftivcJobTable.TryCreate(modLoader);
            if (table != null) break;
            Thread.Sleep(100);
        }

        if (table == null) return;

        var abilities = new List<ushort>();

        // Collect toggled abilities
        if (cfg.EnableTreasureHunter)     abilities.Add(InnateGrant.AbilityTreasureHunter);
        if (cfg.EnableConcentration)      abilities.Add(InnateGrant.AbilityConcentration);
        if (cfg.EnableSafeguard)          abilities.Add(InnateGrant.AbilitySafeguard);
        if (cfg.EnableAttackBoost)        abilities.Add(InnateGrant.AbilityAttackBoost);
        if (cfg.EnableDefenseBoost)       abilities.Add(InnateGrant.AbilityDefenseBoost);
        if (cfg.EnableMagickBoost)        abilities.Add(InnateGrant.AbilityMagickBoost);
        if (cfg.EnableMagickDefenseBoost) abilities.Add(InnateGrant.AbilityMagickDefenseBoost);
        if (cfg.EnableBrawler)            abilities.Add(InnateGrant.AbilityBrawler);
        if (cfg.EnableTame)               abilities.Add(InnateGrant.AbilityTame);
        if (cfg.EnablePoach)              abilities.Add(InnateGrant.AbilityPoach);

        // Parse any additional custom abilities from text box
        if (!string.IsNullOrWhiteSpace(cfg.CustomAbilityIds))
        {
            var parts = cfg.CustomAbilityIds.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                string clean = part.Trim();
                if (clean.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    if (ushort.TryParse(clean[2..], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort hexVal))
                        abilities.Add(hexVal);
                }
                else if (ushort.TryParse(clean, out ushort decVal))
                {
                    abilities.Add(decVal);
                }
            }
        }

        new InnateGrant(table, abilities, cfg.IncludeGenericJobs).Run();
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

    public void Suspend() { }
    public void Resume() { }
    public void Unload() { }
    public bool CanUnload() => false;
    public bool CanSuspend() => false;
    public Action Disposing { get; } = () => { };
}
