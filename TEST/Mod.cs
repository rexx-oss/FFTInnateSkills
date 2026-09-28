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

    // In-battle sentinels verified in Treasure Master
    private const long Slot9Addr      = 0x140782A54L; // 0xFFFFFFFF when in battle
    private const long BattleModeAddr = 0x1409069A0L; // != 0 on battlefield
    private const long UnitArrayBase  = 0x141851F00L; // Base of the 512-byte unit structs
    private const long UnitStride     = 0x200L;       // 512 bytes per combatant

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
                        // Give the game 2 seconds to finish placing all units on the field
                        await Task.Delay(2000, token);

                        DumpBattleUnits(outputPath);
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

    private static void DumpBattleUnits(string outputPath)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"=== FFT BATTLE UNIT MEMORY DUMP ({DateTime.Now}) ===");
        sb.AppendLine($"Module ASLR Offset: 0x{Mem.AslrOffset:X}");
        sb.AppendLine();

        // Dump Unit 0 (Player 1 / Ramza), Unit 1 (Player Generic), Unit 4 (Player 5), and Unit 5 (Enemy 1)
        int[] unitsToDump = { 0, 1, 4, 5 };

        foreach (int idx in unitsToDump)
        {
            long addr = UnitArrayBase + (idx * UnitStride);
            byte[] data = Mem.ReadBytes(addr, 128); // Read first 128 bytes of the struct

            string label = idx switch
            {
                0 => "Unit 0 (Player 1 / Ramza)",
                1 => "Unit 1 (Player 2 / Generic)",
                4 => "Unit 4 (Player 5)",
                5 => "Unit 5 (Enemy 1)",
                _ => $"Unit {idx}"
            };

            sb.AppendLine($"--- {label} at 0x{addr:X} ---");
            for (int r = 0; r < data.Length; r += 16)
            {
                sb.Append($"+0x{r:X2}: ");
                for (int b = 0; b < 16; b++)
                {
                    sb.Append($"{data[r + b]:X2} ");
                    if (b == 7) sb.Append(" ");
                }
                sb.AppendLine();
            }
            sb.AppendLine();
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
