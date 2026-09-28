using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;

namespace FFTInnateSkills;

public class Mod : IMod
{
    private const string ModId = "rexx.fft.innateskills";

    // In-battle sentinels verified from Treasure Master
    private const long Slot9Addr      = 0x140782A54L; // 0xFFFFFFFF in battle
    private const long BattleModeAddr = 0x1409069A0L; // != 0 on battlefield
    private const long Slot0Base      = 0x141851F00L; // Base of the 512-byte unit block
    private const long SlotStride     = 0x200L;       // 512 bytes per slot
    private const int  TotalSlots     = 65;           // 65 total combatant slots in FFT

    private CancellationTokenSource? _cts;
    private bool _dumpedThisBattle;

    public void Start(IModLoaderV1 modLoader) => StartInspector();
    public void StartEx(IModLoaderV1 modLoader, IModConfigV1 modConfig) => StartInspector();

    private void StartInspector()
    {
        string modDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;
        string outputPath = Path.Combine(modDir, "unit-struct-dump.txt");

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

                    if (inBattle && !_dumpedThisBattle)
                    {
                        // Wait 3 seconds to ensure all units are positioned on the field
                        await Task.Delay(3000, token);

                        DumpActiveCombatants(outputPath);
                        _dumpedThisBattle = true;
                    }
                    else if (!inBattle)
                    {
                        _dumpedThisBattle = false;
                    }
                }
                catch { }

                try { await Task.Delay(500, token); } catch { }
            }
        }, token);
    }

    private static void DumpActiveCombatants(string outputPath)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"=== FFT ACTIVE BATTLE UNIT OCCUPANCY DUMP ({DateTime.Now}) ===");
        sb.AppendLine($"Module ASLR Offset: 0x{Mem.AslrOffset:X}");
        sb.AppendLine();

        int activeFound = 0;

        for (int k = 0; k < TotalSlots; k++)
        {
            long slotAddr = Slot0Base + (k * SlotStride);
            byte[] data = Mem.ReadBytes(slotAddr, 256); // Read first 256 bytes

            // Check if slot has non-zero combatant data
            bool isNonZero = false;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != 0) { isNonZero = true; break; }
            }

            if (isNonZero)
            {
                activeFound++;
                byte posX = data[0x2F];
                byte posY = data[0x30];

                sb.AppendLine($"=== [ACTIVE UNIT #{activeFound}] Slot #{k} at 0x{slotAddr:X} (Field Position: X={posX}, Y={posY}) ===");
                for (int r = 0; r < data.Length; r += 16)
                {
                    sb.Append($"+0x{r:X2}: ");
                    for (int b = 0; b < 16 && (r + b) < data.Length; b++)
                    {
                        sb.Append($"{data[r + b]:X2} ");
                        if (b == 7) sb.Append(" ");
                    }
                    sb.AppendLine();
                }
                sb.AppendLine();
            }
        }

        sb.AppendLine($"Total Active Units Deployed on Map: {activeFound} (Scanned {TotalSlots} slots)");

        if (activeFound == 0)
        {
            sb.AppendLine("No non-zero slots found in current 65-slot block.");
        }

        File.WriteAllText(outputPath, sb.ToString());
    }

    public void Suspend() { }
    public void Resume() { }
    public void Unload() => _cts?.Cancel();
    public bool CanUnload() => false;
    public bool CanSuspend() => false;
    public Action Disposing { get; } = () => { };
}
