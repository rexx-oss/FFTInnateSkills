using System.ComponentModel;

namespace FFTInnateSkills.Configuration;

/// <summary>
/// Configuration for innate skill grants. Features Poach and Tame out-of-the-box,
/// with support for adding any additional ability IDs.
/// </summary>
public class Config : Configurable<Config>
{
    [DisplayName("Enable Innate Poach")]
    [Description("Grants the Poach (Secret Hunt, 0x01D7) support ability innately to standard player jobs and unique story characters. Default: on.")]
    [DefaultValue(true)]
    public bool EnablePoach { get; set; } = true;

    [DisplayName("Enable Innate Tame")]
    [Description("Grants the Tame (Train, 0x01D6) support ability innately, recruiting monsters when knocked to Critical HP with a normal attack. Default: on.")]
    [DefaultValue(true)]
    public bool EnableTame { get; set; } = true;

    [DisplayName("Additional Ability IDs")]
    [Description("Add any extra abilities to grant innately to free slots. Enter comma-separated hex or decimal IDs (e.g., '0x01DE, 0x01DD' or '478, 477'). Default: empty.")]
    public string CustomAbilityIds { get; set; } = "";
}
