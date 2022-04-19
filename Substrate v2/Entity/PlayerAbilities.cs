namespace Substrate_v2.Entity;
/// <summary> Encompases data to specify player abilities, especially mode-dependent abilities. </summary>
/// <remarks>Whether or not any of these values are respected by the game client is dependent upon the active game mode.</remarks>
public class PlayerAbilities : ICopyable<PlayerAbilities>
{
    /// <summary> Gets or sets whether the player is currently flying. </summary>
    public bool Flying { get; set; } = false;
    /// <summary> Gets or sets whether the player can instantly build or mine. </summary>
    public bool InstantBuild { get; set; } = false;
    /// <summary> Gets or sets whether the player is allowed to fly. </summary>
    public bool MayFly { get; set; } = false;
    /// <summary> Gets or sets whether the player can take damage. </summary>
    public bool Invulnerable { get; set; } = false;
    /// <summary> Gets or sets whether the player can create or destroy blocks. </summary>
    public bool MayBuild { get; set; } = true;
    /// <summary> Gets or sets the player's walking speed.  Always 0.1. </summary>
    public float FlySpeed { get; set; } = 0.05f;
    /// <summary> Gets or sets the player's flying speed.  Always 0.05. </summary>
    public float WalkSpeed { get; set; } = 0.1f;
    #region ICopyable<PlayerAbilities> Members
    /// <inheritdoc />
    public PlayerAbilities Copy() => new PlayerAbilities()
    {
        Flying = Flying,
        InstantBuild = InstantBuild,
        MayFly = MayFly,
        Invulnerable = Invulnerable,
        MayBuild = MayBuild,
        WalkSpeed = WalkSpeed,
        FlySpeed = FlySpeed,
    };
    #endregion
}