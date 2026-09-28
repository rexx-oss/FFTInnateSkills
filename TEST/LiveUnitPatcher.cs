using System;
using System.Collections.Generic;

namespace FFTInnateSkills;

internal static class LiveUnitPatcher
{
    public const long Slot0Base  = 0x141851F00L;
    public const long SlotStride = 0x200L; // 512 bytes per combatant

    // Strictly player party slots (verified from live dump: Player = 15..30, Enemies = 31+)
    public const int PlayerSlotStart = 15;
    public const int PlayerSlotEnd   = 30;

    // Ability slots array inside each unit struct (starts at +0xD0, 2 bytes per ability)
    public const int AbilityArrayOffset = 0xD0;
    public const int AbilityArrayLength = 24; // 24 ability slots (48 bytes: 0xD0 to 0xFE)

    public static void ApplyToPlayerUnits(List<ushort> abilities)
    {
        if (abilities.Count == 0) return;

        for (int slot = PlayerSlotStart; slot <= PlayerSlotEnd; slot++)
        {
            long unitBase = Slot0Base + (slot * SlotStride);

            // Read the unit's active ability array (+0xD0 to +0xFE)
            byte[] abilityBytes = Mem.ReadBytes(unitBase + AbilityArrayOffset, AbilityArrayLength * 2);

            // If the slot is completely uninitialized, skip it
            bool hasData = false;
            for (int i = 0; i < abilityBytes.Length; i++)
            {
                if (abilityBytes[i] != 0) { hasData = true; break; }
            }
            if (!hasData) continue;

            // Parse live ability IDs into a list
            var liveAbilities = new List<ushort>();
            int firstEmptyIndex = -1;

            for (int i = 0; i < AbilityArrayLength; i++)
            {
                ushort id = (ushort)(abilityBytes[i * 2] | (abilityBytes[i * 2 + 1] << 8));
                liveAbilities.Add(id);

                if (id == 0 && firstEmptyIndex == -1)
                {
                    firstEmptyIndex = i;
                }
            }

            // Inject the selected abilities into empty slots
            foreach (var abilityId in abilities)
            {
                // Skip if player unit already has this ability
                if (liveAbilities.Contains(abilityId)) continue;

                if (firstEmptyIndex != -1 && firstEmptyIndex < AbilityArrayLength)
                {
                    long targetAddr = unitBase + AbilityArrayOffset + (firstEmptyIndex * 2);
                    Mem.W16(targetAddr, abilityId);

                    liveAbilities[firstEmptyIndex] = abilityId;

                    // Find next empty slot
                    firstEmptyIndex = liveAbilities.IndexOf((ushort)0);
                }
            }
        }
    }
}
