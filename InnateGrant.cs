using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FFTInnateSkills;

internal sealed class InnateGrant
{
    // Full Support & Utility Abilities Table
    public const ushort AbilityAttackBoost        = 465; // 0x01D1
    public const ushort AbilityDefenseBoost       = 466; // 0x01D2
    public const ushort AbilityMagickBoost        = 467; // 0x01D3
    public const ushort AbilityMagickDefenseBoost = 468; // 0x01D4
    public const ushort AbilityConcentration      = 469; // 0x01D5
    public const ushort AbilityTame               = 470; // 0x01D6
    public const ushort AbilityPoach              = 471; // 0x01D7
    public const ushort AbilityBrawler            = 472; // 0x01D8
    public const ushort AbilityMonsterTalk        = 473; // 0x01D9
    public const ushort AbilityThrowItem          = 474; // 0x01DA
    public const ushort AbilitySafeguard          = 475; // 0x01DB
    public const ushort AbilityDoublehand         = 476; // 0x01DC
    public const ushort AbilityDualWield          = 477; // 0x01DD
    public const ushort AbilityBeastmaster        = 478; // 0x01DE
    public const ushort AbilityDefend             = 479; // 0x01DF
    public const ushort AbilityEquipChange        = 480; // 0x01E0
    public const ushort AbilityEquipShields       = 481; // 0x01E1
    public const ushort AbilityEquipSwords        = 482; // 0x01E2
    public const ushort AbilityEquipKnives        = 483; // 0x01E3
    public const ushort AbilityEquipKatana        = 484; // 0x01E4
    public const ushort AbilityEquipAxes          = 485; // 0x01E5
    public const ushort AbilityEquipCrossbows     = 486; // 0x01E6
    public const ushort AbilityEquipGuns          = 487; // 0x01E7
    public const ushort AbilityEquipHeavyArmor    = 488; // 0x01E8
    public const ushort AbilityEquipClothing      = 489; // 0x01E9
    public const ushort AbilityEquipRobes         = 490; // 0x01EA
    public const ushort AbilitySwiftness          = 491; // 0x01EB
    public const ushort AbilityHalveMP            = 493; // 0x01ED
    public const ushort AbilityTreasureHunter     = 509; // 0x01FD

    public static readonly int[] StoryJobIds =
    {
        1, 2, 3,                    // Ramza
        13, 22, 25, 26, 30, 50, 72  // Orlandeau, Mustadio, Rapha, Marach, Agrias, Cloud, Reis Dragon
    };

    public static readonly int[] GenericJobIds =
    {
        74, 75, 76, 77, 78, 79, 80, 81, 82, 83,
        84, 85, 86, 87, 88, 89, 90, 91, 92, 93
    };

    private readonly FftivcJobTable _table;
    private readonly List<ushort> _abilitiesToGrant;
    private readonly bool _includeGenerics;

    public InnateGrant(FftivcJobTable table, List<ushort> abilitiesToGrant, bool includeGenerics)
    {
        _table = table;
        _abilitiesToGrant = abilitiesToGrant;
        _includeGenerics = includeGenerics;
    }

    public void Run()
    {
        if (_abilitiesToGrant.Count == 0) return;

        for (int i = 0; i < 120; i++)
        {
            if (_table.IsReady()) break;
            Thread.Sleep(250);
        }

        if (!_table.IsReady()) return;

        var targetJobs = new List<int>(StoryJobIds);
        if (_includeGenerics)
        {
            targetJobs.AddRange(GenericJobIds);
        }

        foreach (var job in targetJobs)
        {
            var innates = _table.GetInnates(job);
            if (innates == null) continue;

            foreach (var abilityId in _abilitiesToGrant)
            {
                if (innates.Contains(abilityId)) continue;

                int freeSlot = Array.IndexOf(innates, (ushort)0);
                if (freeSlot != -1)
                {
                    if (_table.TryApplyInnate(job, freeSlot, abilityId))
                    {
                        innates[freeSlot] = abilityId;
                    }
                }
            }
        }
    }
}
