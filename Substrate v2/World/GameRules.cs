namespace Substrate_v2.World;
/// <summary> Encompases data to specify game rules. </summary>
public class GameRules : ICopyable<GameRules>
{
    /// <summary> Gets or sets whether or not actions performed by command blocks are displayed in the chat. </summary>
    public bool CommandBlockOutput { get; set; } = true;
    /// <summary> Gets or sets whether to spread or remove fire. </summary>
    public bool DoFireTick { get; set; } = true;
    /// <summary> Gets or sets whether mobs should drop loot when killed. </summary>
    public bool DoMobLoot { get; set; } = true;
    /// <summary> Gets or sets whether mobs should spawn naturally. </summary>
    public bool DoMobSpawning { get; set; } = true;
    /// <summary> Gets or sets whether breaking blocks should drop the block's item drop. </summary>
    public bool DoTileDrops { get; set; } = true;
    /// <summary> Gets or sets whether players keep their inventory after they die. </summary>
    public bool KeepInventory { get; set; } = false;

    /// <summary> Gets or sets whether mobs can destroy blocks (creeper explosions, zombies breaking doors, etc.). </summary>
    public bool MobGriefing { get; set; } = true;
    public GameRules Copy() => new GameRules() {
        CommandBlockOutput = CommandBlockOutput,
        DoFireTick = DoFireTick,
        DoMobLoot = DoMobLoot,
        DoMobSpawning = DoMobSpawning,
        DoTileDrops = DoTileDrops,
        KeepInventory = KeepInventory,
        MobGriefing = MobGriefing,
    };
}
/// <summary> Specifies the type of gameplay associated with a world. </summary>
public enum GameType
{
    /// <summary> The world will be played in Survival mode. </summary>
    SURVIVAL = 0,
    /// <summary> The world will be played in Creative mode. </summary>
    CREATIVE = 1,
}
