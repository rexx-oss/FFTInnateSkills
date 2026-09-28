using System.ComponentModel;

namespace FFTInnateSkills.Configuration;

/// <summary>
/// Configuration for innate skill grants.
/// All options default OFF for full manual control.
/// </summary>
public class Config : Configurable<Config>
{
    [DisplayName("Apply to Generic Jobs (Warning: Enemies Share)")]
    [Description("When ON, innates apply to all 20 generic jobs (Squires, Knights, Chemists, Thieves, etc.). " +
                 "Enemy human soldiers of those jobs will also receive the abilities. " +
                 "When OFF, innates are strictly applied to unique story characters (Ramza, Agrias, Cid, Mustadio, etc.) " +
                 "with 0% enemy spillover. Default: off.")]
    [DefaultValue(false)]
    public bool IncludeGenericJobs { get; set; } = false;

    [DisplayName("Enable Innate Treasure Hunter (509)")]
    [Description("Grants Treasure Hunter (Move-Find Item, 509 / 0x01FD) innately. Pick up hidden treasures by walking on tiles. Default: off.")]
    [DefaultValue(false)]
    public bool EnableTreasureHunter { get; set; } = false;

    [DisplayName("Enable Innate Concentration (469)")]
    [Description("Grants Concentration (Concentrate, 469 / 0x01D5) innately. Physical attacks ignore target physical evasion. Default: off.")]
    [DefaultValue(false)]
    public bool EnableConcentration { get; set; } = false;

    [DisplayName("Enable Innate Safeguard (475)")]
    [Description("Grants Safeguard (Maintenance, 475 / 0x01DB) innately. Protects equipment from theft and destruction. Default: off.")]
    [DefaultValue(false)]
    public bool EnableSafeguard { get; set; } = false;

    [DisplayName("Enable Innate Attack Boost (465)")]
    [Description("Grants Attack Boost (Attack UP, 465 / 0x01D1) innately. Increases physical damage by 33%. Default: off.")]
    [DefaultValue(false)]
    public bool EnableAttackBoost { get; set; } = false;

    [DisplayName("Enable Innate Defense Boost (466)")]
    [Description("Grants Defense Boost (Defense UP, 466 / 0x01D2) innately. Reduces incoming physical damage by 33%. Default: off.")]
    [DefaultValue(false)]
    public bool EnableDefenseBoost { get; set; } = false;

    [DisplayName("Enable Innate Magick Boost (467)")]
    [Description("Grants Magick Boost (Magic Attack UP, 467 / 0x01D3) innately. Increases magic damage and spell power by 33%. Default: off.")]
    [DefaultValue(false)]
    public bool EnableMagickBoost { get; set; } = false;

    [DisplayName("Enable Innate Magick Defense Boost (468)")]
    [Description("Grants Magick Defense Boost (Magic Defend UP, 468 / 0x01D4) innately. Reduces incoming magic damage by 33%. Default: off.")]
    [DefaultValue(false)]
    public bool EnableMagickDefenseBoost { get; set; } = false;

    [DisplayName("Enable Innate Brawler (472)")]
    [Description("Grants Brawler (Martial Arts, 472 / 0x01D8) innately. Significantly increases unarmed bare-handed attack damage. Default: off.")]
    [DefaultValue(false)]
    public bool EnableBrawler { get; set; } = false;

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
