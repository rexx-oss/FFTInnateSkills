using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FFTInnateSkills;

internal sealed class InnateGrant
{
    // Default Ability IDs in FFT
    public const ushort AbilityPoach = 0x01D7; // Secret Hunt (471)
    public const ushort AbilityTame  = 0x01D6; // Train (470)

    // Standard player generics and unique story characters
    public static readonly int[] TargetJobIds =
    {
        1, 2, 3,                                 // Ramza (Chapters 1, 2/3, 4)
        13, 22, 25, 26, 30, 50, 72,              // Orlandeau, Mustadio, Rapha, Marach, Agrias, Cloud, Reis Dragon
        74, 75, 76, 77, 78, 79, 80, 81, 82, 83,  // Standard Generics (Male & Female)
        84, 85, 86, 87, 88, 89, 90, 91, 92, 93
    };

    private readonly FftivcJobTable _table;
    private readonly List<ushort> _abilitiesToGrant;

    public InnateGrant(FftivcJobTable table, List<ushort> abilitiesToGrant)
    {
        _table = table;
        _abilitiesToGrant = abilitiesToGrant;
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

        foreach (var job in TargetJobIds)
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
