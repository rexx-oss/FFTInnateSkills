using System.ComponentModel;

namespace FFTInnateSkills.Configuration;

/// <summary>
/// Configuration for innate skill grants. All abilities are default OFF (toggle manually).
/// </summary>
public class Config : Configurable<Config>
{
    [DisplayName("Enable Innate Concentration (469)")]
    [Description("Grants Concentration (Concentrate, 469 / 0x01D5) innately. Physical attacks ignore target physical evasion. Default: off.")]
    [DefaultValue(false)]
    public bool EnableConcentration { get; set; } = false;

    [DisplayName("Enable Innate Safeguard (475)")]
    [Description("Grants Safeguard (Maintenance, 475 / 0x01DB) innately. Protects equipped items from theft and destruction. Default: off.")]
    [DefaultValue(false)]
    public bool EnableSafeguard { get; set; } = false;

    [DisplayName("Enable Innate Attack Boost (465)")]
    [Description("Grants Attack Boost (Attack UP, 465 / 0x01D1) innately. Increases physical damage by 33%. Default: off.")]
    [DefaultValue(false)]
    public bool EnableAttackBoost { get; set; } = false;

    [DisplayName("Enable Innate Tame (470)")]
    [Description("Grants Tame (Train, 470 / 0x01D6) innately. Recruits monsters reduced to Critical HP with a basic attack. Default: off.")]
    [DefaultValue(false)]
    public bool EnableTame { get; set; } = false;

    [DisplayName("Enable Innate Poach (471)")]
    [Description("Grants Poach (Secret Hunt, 471 / 0x01D7) innately. Poaches monsters upon delivering the killing blow. Default: off.")]
    [DefaultValue(false)]
    public bool EnablePoach { get; set; } = false;

    [DisplayName("Additional Custom Ability IDs")]
    [Description("Add any extra abilities to grant innately to free slots. Enter comma-separated hex or decimal IDs (e.g., '0x01DE, 478'). Default: empty.")]
    public string CustomAbilityIds { get; set; } = "";
}
