using System.IO;
using Reloaded.Mod.Interfaces;

namespace FFTInnateSkills.Configuration;

public class ConfiguratorMixinBase
{
    public virtual IUpdatableConfigurable[] MakeConfigurations(string configFolder)
    {
        var path = Path.Combine(configFolder, "Config.json");
        var config = Configurable<Config>.FromFile(path, "FFT Innate Skills Configuration");
        return new IUpdatableConfigurable[] { config };
    }
}

public class ConfiguratorMixin : ConfiguratorMixinBase { }
