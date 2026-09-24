using System;
using System.Runtime.CompilerServices;
using fftivc.utility.modloader.Interfaces.Tables;
using fftivc.utility.modloader.Interfaces.Tables.Models;
using Reloaded.Mod.Interfaces.Internal;

namespace FFTInnateSkills;

internal sealed class FftivcJobTable
{
    private const string OwnerModId = "rexx.fft.innateskills";
    private readonly IFFTOJobDataManager _mgr;

    private FftivcJobTable(IFFTOJobDataManager mgr) => _mgr = mgr;

    public static FftivcJobTable? TryCreate(IModLoaderV1 loader)
    {
        try { return Create(loader); }
        catch { return null; }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static FftivcJobTable? Create(IModLoaderV1 loader)
    {
        var weak = loader.GetController<IFFTOJobDataManager>();
        return weak != null && weak.TryGetTarget(out var mgr) ? new FftivcJobTable(mgr) : null;
    }

    public bool IsReady()
    {
        try { _mgr.GetJob(0); return true; }
        catch { return false; }
    }

    public ushort[]? GetInnates(int jobId)
    {
        try
        {
            var job = _mgr.GetJob(jobId);
            if (job == null) return null;
            return new[]
            {
                job.InnateAbilityId1 ?? 0,
                job.InnateAbilityId2 ?? 0,
                job.InnateAbilityId3 ?? 0,
                job.InnateAbilityId4 ?? 0,
            };
        }
        catch { return null; }
    }

    public bool TryApplyInnate(int jobId, int slotIndex, ushort abilityId)
    {
        try
        {
            var patch = new Job { Id = jobId };
            switch (slotIndex)
            {
                case 0: patch.InnateAbilityId1 = abilityId; break;
                case 1: patch.InnateAbilityId2 = abilityId; break;
                case 2: patch.InnateAbilityId3 = abilityId; break;
                case 3: patch.InnateAbilityId4 = abilityId; break;
                default: return false;
            }
            _mgr.ApplyTablePatch(OwnerModId, patch);
            return true;
        }
        catch { return false; }
    }
}
