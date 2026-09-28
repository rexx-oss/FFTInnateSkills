using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FFTInnateSkills.Configuration;

namespace FFTInnateSkills;

internal sealed class BattleEngine
{
    private const int PollIntervalMs = 500;

    private const long Slot9Addr      = 0x140782A54L; // 0xFFFFFFFF when battle map is active
    private const long BattleModeAddr = 0x1409069A0L; // != 0 on battlefield

    private readonly Config _config;
    private CancellationTokenSource? _cts;
    private bool _appliedThisBattle;

    public BattleEngine(Config config)
    {
        _config = config;
    }

    public void Start()
    {
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    uint slot9 = Mem.U32(Slot9Addr);
                    byte mode  = Mem.U8(BattleModeAddr);
                    bool inBattle = (slot9 == 0xFFFFFFFFu && mode != 0);

                    if (inBattle && !_appliedThisBattle)
                    {
                        // Wait 2.5 seconds for units to be fully deployed on the field
                        await Task.Delay(2500, token);

                        var abilities = GetSelectedAbilities(_config);
                        LiveUnitPatcher.ApplyToPlayerUnits(abilities);

                        _appliedThisBattle = true;
                    }
                    else if (!inBattle)
                    {
                        _appliedThisBattle = false;
                    }
                }
                catch { }

                try { await Task.Delay(PollIntervalMs, token); } catch { }
            }
        }, token);
    }

    public void Stop()
    {
        try { _cts?.Cancel(); } catch { }
    }

    private static List<ushort> GetSelectedAbilities(Config cfg)
    {
        var list = new List<ushort>();

        if (cfg.EnableConcentration) list.Add(469); // Concentration (0x01D5)
        if (cfg.EnableSafeguard)     list.Add(475); // Safeguard (0x01DB)
        if (cfg.EnableAttackBoost)   list.Add(465); // Attack Boost (0x01D1)
        if (cfg.EnableTame)          list.Add(470); // Tame (0x01D6)
        if (cfg.EnablePoach)         list.Add(471); // Poach (0x01D7)

        if (!string.IsNullOrWhiteSpace(cfg.CustomAbilityIds))
        {
            var parts = cfg.CustomAbilityIds.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                string clean = part.Trim();
                if (clean.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    if (ushort.TryParse(clean[2..], System.Globalization.NumberStyles.HexNumber, null, out var hexVal))
                        list.Add(hexVal);
                }
                else if (ushort.TryParse(clean, out var decVal))
                {
                    list.Add(decVal);
                }
            }
        }

        return list;
    }
}
