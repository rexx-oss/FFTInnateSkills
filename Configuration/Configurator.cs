using System.IO;
using Reloaded.Mod.Interfaces;

namespace FFTInnateSkills.Configuration;

public class Configurator : IConfiguratorV3
{
    private static readonly ConfiguratorMixin _mixin = new();

    public string? ModFolder { get; private set; }
    public string? ConfigFolder { get; private set; }
    public ConfiguratorContext Context { get; private set; }

    public IUpdatableConfigurable[] Configurations => _configurations ?? MakeConfigurations();
    private IUpdatableConfigurable[]? _configurations;

    private IUpdatableConfigurable[] MakeConfigurations()
    {
        _configurations = _mixin.MakeConfigurations(ConfigFolder!);
        for (int i = 0; i < _configurations.Length; i++)
        {
            var idx = i;
            _configurations[i].ConfigurationUpdated += c => _configurations[idx] = c;
        }
        return _configurations;
    }

    public Configurator() { }
    public Configurator(string configDirectory) : this() => ConfigFolder = configDirectory;

    public IConfiguratorV3 SetModDirectory(string modDirectory) { ModFolder = modDirectory; return this; }
    public IConfiguratorV3 SetConfigDirectory(string configDirectory) { ConfigFolder = configDirectory; return this; }
    public IConfiguratorV3 SetConfigDirectory(string configDirectory, string oldDirectory)
    {
        ConfigFolder = configDirectory;
        return this;
    }

    void IConfiguratorV3.SetContext(in ConfiguratorContext context) => Context = context;
    void IConfiguratorV2.SetConfigDirectory(string configDirectory) => ConfigFolder = configDirectory;
    public void Migrate(string oldDirectory, string newDirectory) { }
    public IConfigurable[] GetConfigurations() => Configurations;
    void IConfiguratorV1.SetModDirectory(string d) => ModFolder = d;
    public bool TryRunCustomConfiguration() => false;
    public TType GetConfiguration<TType>(int index) where TType : class => (TType)(object)Configurations[index];
    public void Save()
    {
        foreach (var c in Configurations) (c as IConfigurable)?.Save?.Invoke();
    }
}
