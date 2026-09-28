using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FFTInnateSkills;

internal sealed class InnateGrant
{
    // Ability IDs (Decimal / Hex)
    public const ushort AbilityAttackBoost        = 465; // 0x01D1 (Attack UP)
    public const ushort AbilityDefenseBoost       = 466; // 0x01D2 (Defense UP)
    public const ushort AbilityMagickBoost        = 467; // 0x01D3 (Magic Attack UP)
    public const ushort AbilityMagickDefenseBoost = 468; // 0x01D4 (Magic Defend UP)
    public const ushort AbilityConcentration      = 469; // 0x01D5 (Concentrate)
    public const ushort AbilityTame               = 470; // 0x01D6 (Train)
    public const ushort AbilityPoach              = 471; // 0x01D7 (Secret Hunt)
    public const ushort AbilityBrawler            = 472; // 0x01D8 (Martial Arts)
    public const ushort AbilitySafeguard          = 475; // 0x01DB (Maintenance)
    public const ushort AbilityTreasureHunter     = 509; // 0x01FD (Move-Find Item)

    // Player-only Unique Story Jobs (Enemies NEVER use these)
    public static readonly int[] StoryJobIds =
    {
        1, 2, 3,                    // Ramza (Chapters 1, 2/3, 4)
        13, 22, 25, 26, 30, 50, 72  // Orlandeau, Mustadio, Rapha, Marach, Agrias, Cloud, Reis Dragon
    };

    // Generic Jobs (Shared with enemy spawns)
    public static readonly int[] GenericJobIds =
    {
        74, 75, 76, 77, 78, 79, 80, 81, 82, 83, // Standard Male & Female Generic Jobs (Squire to Mime)
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

        // Wait up to 30 seconds for the mod loader's async signature scan to finish
        for (int i = 0; i < 120; i++)
        {
            if (_table.IsReady()) break;
            Thread.Sleep(250);
        }

        if (!_table.IsReady()) return;

        // Determine targets: Story units only OR all jobs
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
                // Skip if the job already has this ability in any of its 4 slots
                if (innates.Contains(abilityId)) continue;

                // Find the first free slot (0 = empty)
                int freeSlot = Array.IndexOf(innates, (ushort)0);
                if (freeSlot != -1)
                {
                    if (_table.TryApplyInnate(job, freeSlot, abilityId))
                    {
                        innates[freeSlot] = abilityId; // Track locally for subsequent grants
                    }
                }
            }
        }
    }
}
