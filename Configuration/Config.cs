using System.ComponentModel;

namespace FFTInnateSkills.Configuration;

public class Config : Configurable<Config>
{
    [DisplayName("Apply to Generic Jobs (Warning: Enemies Share)")]
    [Description("When ON, innates apply to all generic jobs (enemy soldiers will share them). " +
                 "When OFF, strictly applied to unique story characters (Ramza, Agrias, Cid, etc.) with 0% enemy spillover. Default: off.")]
    [DefaultValue(false)]
    public bool IncludeGenericJobs { get; set; } = false;

    // --- Core Combat Passives ---

    [DisplayName("Dual Wield (477)")]
    [Description("Two Swords: Wield two one-handed weapons simultaneously. Default: off.")]
    [DefaultValue(false)]
    public bool EnableDualWield { get; set; } = false;

    [DisplayName("Doublehand (476)")]
    [Description("Two Hands: Hold a one-handed weapon with two hands to increase damage. Default: off.")]
    [DefaultValue(false)]
    public bool EnableDoublehand { get; set; } = false;

    [DisplayName("Swiftness (491)")]
    [Description("Short Charge: Halves spell casting charge time. Default: off.")]
    [DefaultValue(false)]
    public bool EnableSwiftness { get; set; } = false;

    [DisplayName("Halve MP (493)")]
    [Description("Cuts all MP costs in half. Default: off.")]
    [DefaultValue(false)]
    public bool EnableHalveMP { get; set; } = false;

    [DisplayName("Concentration (469)")]
    [Description("Concentrate: Physical attacks ignore target evasion. Default: off.")]
    [DefaultValue(false)]
    public bool EnableConcentration { get; set; } = false;

    [DisplayName("Attack Boost (465)")]
    [Description("Attack UP: Increases physical attack damage by 33%. Default: off.")]
    [DefaultValue(false)]
    public bool EnableAttackBoost { get; set; } = false;

    [DisplayName("Defense Boost (466)")]
    [Description("Defense UP: Reduces physical damage taken by 33%. Default: off.")]
    [DefaultValue(false)]
    public bool EnableDefenseBoost { get; set; } = false;

    [DisplayName("Magick Boost (467)")]
    [Description("Magic Attack UP: Increases magic damage and healing by 33%. Default: off.")]
    [DefaultValue(false)]
    public bool EnableMagickBoost { get; set; } = false;

    [DisplayName("Magick Defense Boost (468)")]
    [Description("Magic Defend UP: Reduces magic damage taken by 33%. Default: off.")]
    [DefaultValue(false)]
    public bool EnableMagickDefenseBoost { get; set; } = false;

    [DisplayName("Brawler (472)")]
    [Description("Martial Arts: Greatly increases unarmed barehanded damage. Default: off.")]
    [DefaultValue(false)]
    public bool EnableBrawler { get; set; } = false;

    // --- Utility & Loot Passives ---

    [DisplayName("Treasure Hunter (509)")]
    [Description("Move-Find Item: Discover hidden treasures on battlefield tiles. Default: off.")]
    [DefaultValue(false)]
    public bool EnableTreasureHunter { get; set; } = false;

    [DisplayName("Safeguard (475)")]
    [Description("Maintenance: Protects equipment from theft and destruction. Default: off.")]
    [DefaultValue(false)]
    public bool EnableSafeguard { get; set; } = false;

    [DisplayName("Poach (471)")]
    [Description("Secret Hunt: Poaches monsters upon delivering the killing blow. Default: off.")]
    [DefaultValue(false)]
    public bool EnablePoach { get; set; } = false;

    [DisplayName("Tame (470)")]
    [Description("Train: Recruits monsters reduced to Critical HP with a basic attack. Default: off.")]
    [DefaultValue(false)]
    public bool EnableTame { get; set; } = false;

    [DisplayName("Beastmaster (478)")]
    [Description("Monster Skill: Unlocks hidden special skills on adjacent allied monsters. Default: off.")]
    [DefaultValue(false)]
    public bool EnableBeastmaster { get; set; } = false;

    [DisplayName("Monster Talk (473)")]
    [Description("Beast Tongue: Allows human speech abilities (Talk Skill) to work on monsters. Default: off.")]
    [DefaultValue(false)]
    public bool EnableMonsterTalk { get; set; } = false;

    [DisplayName("Throw Item (474)")]
    [Description("Enables using Item commands at ranged distance. Default: off.")]
    [DefaultValue(false)]
    public bool EnableThrowItem { get; set; } = false;

    [DisplayName("Defend (479)")]
    [Description("Adds the Defend command to action menus. Default: off.")]
    [DefaultValue(false)]
    public bool EnableDefend { get; set; } = false;

    [DisplayName("Equip Change (480)")]
    [Description("Enables swapping equipment mid-battle. Default: off.")]
    [DefaultValue(false)]
    public bool EnableEquipChange { get; set; } = false;

    // --- Equip Gear Passives ---

    [DisplayName("Equip Shields (481)")]
    [DefaultValue(false)]
    public bool EnableEquipShields { get; set; } = false;

    [DisplayName("Equip Swords (482)")]
    [DefaultValue(false)]
    public bool EnableEquipSwords { get; set; } = false;

    [DisplayName("Equip Knives (483)")]
    [DefaultValue(false)]
    public bool EnableEquipKnives { get; set; } = false;

    [DisplayName("Equip Katana (484)")]
    [DefaultValue(false)]
    public bool EnableEquipKatana { get; set; } = false;

    [DisplayName("Equip Axes (485)")]
    [DefaultValue(false)]
    public bool EnableEquipAxes { get; set; } = false;

    [DisplayName("Equip Crossbows (486)")]
    [DefaultValue(false)]
    public bool EnableEquipCrossbows { get; set; } = false;

    [DisplayName("Equip Guns (487)")]
    [DefaultValue(false)]
    public bool EnableEquipGuns { get; set; } = false;

    [DisplayName("Equip Heavy Armor (488)")]
    [DefaultValue(false)]
    public bool EnableEquipHeavyArmor { get; set; } = false;

    [DisplayName("Equip Clothing (489)")]
    [DefaultValue(false)]
    public bool EnableEquipClothing { get; set; } = false;

    [DisplayName("Equip Robes (490)")]
    [DefaultValue(false)]
    public bool EnableEquipRobes { get; set; } = false;

    // --- Custom IDs ---

    [DisplayName("Additional Custom Ability IDs")]
    [Description("Add any extra abilities to grant innately to free slots. Enter comma-separated hex or decimal IDs.")]
    public string CustomAbilityIds { get; set; } = "";
}
