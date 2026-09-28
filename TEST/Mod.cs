using System;
using System.Diagnostics;
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
    private const long Slot9Addr      = 0x140782A54L; // 0xFFFFFFFF in battle
    private const long BattleModeAddr = 0x1409069A0L; // != 0 in battle

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
                        // Wait 3 seconds after loading to make sure battle units are placed in memory
                        await Task.Delay(3000, token);

                        ScanAndDumpRamza(outputPath);
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

    private static void ScanAndDumpRamza(string outputPath)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"=== FFT BATTLE UNIT REAL-TIME SCAN ({DateTime.Now}) ===");
        sb.AppendLine($"Module ASLR Offset: 0x{Mem.AslrOffset:X}");
        sb.AppendLine();

        var process = Process.GetCurrentProcess();
        var mainModule = process.MainModule;
        if (mainModule == null) return;

        long baseAddr = mainModule.BaseAddress.ToInt64();
        int size = mainModule.ModuleMemorySize;

        byte[] asciiRamza = Encoding.ASCII.GetBytes("Ramza");
        byte[] unicodeRamza = Encoding.Unicode.GetBytes("Ramza");

        // Scan the module's data space in 1MB chunks
        int chunkSize = 0x100000;
        int hitCount = 0;

        for (int offset = 0; offset < size; offset += chunkSize)
        {
            int toRead = Math.Min(chunkSize + 64, size - offset);
            byte[] chunk = Mem.ReadBytes(baseAddr + offset, toRead);
            if (chunk.Length == 0) continue;

            for (int i = 0; i < chunk.Length - 16; i++)
            {
                bool isAscii = MatchPattern(chunk, i, asciiRamza);
                bool isUnicode = !isAscii && MatchPattern(chunk, i, unicodeRamza);

                if (isAscii || isUnicode)
                {
                    hitCount++;
                    long matchAddress = baseAddr + offset + i;

                    sb.AppendLine($"[MATCH #{hitCount}] {(isAscii ? "ASCII" : "Unicode")} 'Ramza' found at 0x{matchAddress:X}");
                    sb.AppendLine($"Offset from Module Base: +0x{(matchAddress - baseAddr):X}");

                    // Dump 64 bytes before the name and 128 bytes after the name
                    long startDump = Math.Max(baseAddr, matchAddress - 64);
                    byte[] unitSlice = Mem.ReadBytes(startDump, 192);

                    sb.AppendLine($"--- Memory Window around Match (0x{startDump:X}) ---");
                    for (int r = 0; r < unitSlice.Length; r += 16)
                    {
                        sb.Append($"+0x{r:X2}: ");
                        for (int b = 0; b < 16 && (r + b) < unitSlice.Length; b++)
                        {
                            sb.Append($"{unitSlice[r + b]:X2} ");
                            if (b == 7) sb.Append(" ");
                        }
                        sb.AppendLine();
                    }
                    sb.AppendLine();

                    if (hitCount >= 5) break; // First few hits are enough to identify the unit table
                }
            }

            if (hitCount >= 5) break;
        }

        if (hitCount == 0)
        {
            sb.AppendLine("No matches for 'Ramza' found in current module memory.");
        }

        File.WriteAllText(outputPath, sb.ToString());
    }

    private static bool MatchPattern(byte[] buffer, int index, byte[] pattern)
    {
        if (index + pattern.Length > buffer.Length) return false;
        for (int p = 0; p < pattern.Length; p++)
        {
            if (buffer[index + p] != pattern[p]) return false;
        }
        return true;
    }

    public void Suspend() { }
    public void Resume() { }
    public void Unload() => _cts?.Cancel();
    public bool CanUnload() => false;
    public bool CanSuspend() => false;
    public Action Disposing { get; } = () => { };
}
