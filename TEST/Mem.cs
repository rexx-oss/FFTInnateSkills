using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FFTInnateSkills;

internal static class Mem
{
    private static readonly nint Self = GetCurrentProcess();
    [ThreadStatic] private static byte[]? _scratch;

    public static readonly long AslrOffset;

    static Mem()
    {
        try
        {
            long baseAddr = Process.GetCurrentProcess().MainModule?.BaseAddress.ToInt64() ?? 0x140000000L;
            AslrOffset = baseAddr - 0x140000000L;
        }
        catch
        {
            AslrOffset = 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Rebase(long a)
    {
        if (AslrOffset != 0 && a >= 0x140000000L && a < 0x200000000L)
            return a + AslrOffset;
        return a;
    }

    public static byte U8(long a)
    {
        a = Rebase(a);
        var s = _scratch ??= new byte[8];
        return ReadProcessMemory(Self, (nint)a, s, 1, out _) ? s[0] : (byte)0;
    }

    public static ushort U16(long a)
    {
        a = Rebase(a);
        var s = _scratch ??= new byte[8];
        return ReadProcessMemory(Self, (nint)a, s, 2, out _) ? (ushort)(s[0] | (s[1] << 8)) : (ushort)0;
    }

    public static uint U32(long a)
    {
        a = Rebase(a);
        var s = _scratch ??= new byte[8];
        return ReadProcessMemory(Self, (nint)a, s, 4, out _) 
            ? (uint)(s[0] | (s[1] << 8) | (s[2] << 16) | (s[3] << 24)) : 0u;
    }

    public static byte[] ReadBytes(long a, int length)
    {
        a = Rebase(a);
        byte[] buf = new byte[length];
        ReadProcessMemory(Self, (nint)a, buf, (nuint)length, out _);
        return buf;
    }

    public static void W16(long a, ushort v)
    {
        a = Rebase(a);
        var s = _scratch ??= new byte[8];
        s[0] = (byte)v;
        s[1] = (byte)(v >> 8);
        WriteProcessMemory(Self, (nint)a, s, 2, out _);
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool ReadProcessMemory(nint h, nint addr, [Out] byte[] buf, nuint size, out nuint read);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool WriteProcessMemory(nint h, nint addr, byte[] buf, nuint size, out nuint written);

    [DllImport("kernel32.dll")]
    private static extern nint GetCurrentProcess();
}
