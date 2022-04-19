namespace Substrate_v2;
public enum PlayerGameMode
{
    Survival = 0,
    Creative = 1,
    Adventure = 2,
}
/// <summary> Represents a Player from either single- or multi-player Minecraft. </summary>
/// <remarks>Unlike <see cref="TypedEntity"/> objects, <see cref="Player"/> objects do not need to be added to chunks.  They
/// are stored individually or within level data.</remarks>
public class Player : Entity, INbtObject<Player>, ICopyable<Player>, IItemContainer
{
    private static readonly SchemaNodeCompound schema = Entity.Schema.MergeInto(new SchemaNodeCompound("")
    {
        new SchemaNodeScaler("AttackTime", TagType.TAG_SHORT, SchemaOptions.CREATE_ON_MISSING),
        new SchemaNodeScaler("DeathTime", TagType.TAG_SHORT),
        new SchemaNodeScaler("Health", TagType.TAG_FLOAT),
        new SchemaNodeScaler("HurtTime", TagType.TAG_SHORT),
        new SchemaNodeScaler("Dimension", TagType.TAG_INT),
        new SchemaNodeList("Inventory", TagType.TAG_COMPOUND, ItemCollection.Schema),
        //new SchemaNodeList("EnderItems", TagType.TAG_COMPOUND, ItemCollection.Schema, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("World", TagType.TAG_STRING, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("Sleeping", TagType.TAG_BYTE, SchemaOptions.CREATE_ON_MISSING),
        new SchemaNodeScaler("SleepTimer", TagType.TAG_SHORT, SchemaOptions.CREATE_ON_MISSING),
        new SchemaNodeScaler("SpawnX", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("SpawnY", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("SpawnZ", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("foodLevel", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("foodTickTimer", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("foodExhaustionLevel", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("foodSaturationLevel", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("XpP", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("XpLevel", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("XpTotal", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("Score", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeScaler("playerGameType", TagType.TAG_INT, SchemaOptions.OPTIONAL),
        new SchemaNodeCompound("abilities", new SchemaNodeCompound("") {
            new SchemaNodeScaler("flying", TagType.TAG_BYTE),
            new SchemaNodeScaler("instabuild", TagType.TAG_BYTE),
            new SchemaNodeScaler("mayfly", TagType.TAG_BYTE),
            new SchemaNodeScaler("invulnerable", TagType.TAG_BYTE),
            new SchemaNodeScaler("mayBuild", TagType.TAG_BYTE, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("walkSpeed", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("flySpeed", TagType.TAG_FLOAT, SchemaOptions.OPTIONAL),
        }, SchemaOptions.OPTIONAL),
    });

    private const int CAPACITY = 105;
    private const int ENDER_CAPACITY = 27;

    private short attackTime;
    private short deathTime;
    private float health;
    private short hurtTime;

    private int dimension;
    private byte sleeping;
    private short sleepTimer;
    private int? spawnX;
    private int? spawnY;
    private int? spawnZ;

    private int? foodLevel;
    private int? foodTickTimer;
    private float? foodExhaustion;
    private float? foodSaturation;
    private float? xpP;
    private int? xpLevel;
    private int? xpTotal;
    private int? score;

    private string world;
    private string name;

    private PlayerAbilities abilities;
    private PlayerGameMode? gameMode;

    private ItemCollection inventory;
    private ItemCollection enderItems;
    /// <summary> Gets or sets the number of ticks left in the player's "invincibility shield" after last struck. </summary>
    public int AttackTime
    {
        get => attackTime; 
        set => attackTime = (short)value;
    }
    /// <summary> Gets or sets the number of ticks that the player has been dead for. </summary>
    public int DeathTime
    {
        get => deathTime; 
        set => deathTime = (short)value; 
    }
    /// <summary> Gets or sets the amount of the player's health. </summary>
    public float Health { get; set; }
    /// <summary> Gets or sets the player's Hurt Time value. </summary>
    public int HurtTime
    {
        get => hurtTime; 
        set => hurtTime = (short)value; 
    }
    /// <summary> Gets or sets the dimension that the player is currently in. </summary>
    public int Dimension { get; set; }
    public PlayerGameMode GameType
    {
        get => gameMode ?? PlayerGameMode.Survival; 
        set => gameMode = value; 
    }
    /// <summary> Gets or sets a value indicating whether the player is sleeping in a bed. </summary>
    public bool IsSleeping
    {
        get => sleeping == 1; 
        set => sleeping = (byte)(value ? 1 : 0); 
    }
    /// <summary> Gets or sets the player's Sleep Timer value. </summary>
    public int SleepTimer
    {
        get => sleepTimer; 
        set => sleepTimer = (short)value; 
    }
    /// <summary> Gets or sets the player's personal spawn point, set by sleeping in beds. </summary>
    public SpawnPoint Spawn
    {
        get => new SpawnPoint(spawnX ?? 0, spawnY ?? 0, spawnZ ?? 0); 
        set
        {
            spawnX = value.X;
            spawnY = value.Y;
            spawnZ = value.Z;
        }
    }
    /// <summary> Tests if the player currently has a personal spawn point. </summary>
    public bool HasSpawn
    {
        get => spawnX != null && spawnY != null && spawnZ != null; 
    }
    /// <summary> Gets or sets the name of the world that the player is currently within. </summary>
    public string World { get; set; }
    /// <summary> Gets or sets the name that is used when the player is read or written from a <see cref="PlayerManager"/>. </summary>
    public string Name { get; set; }
    /// <summary> Gets or sets the player's score. </summary>
    public int Score
    {
        get => score ?? 0; 
        set => score = value; 
    }
    /// <summary> Gets or sets the player's XP Level. </summary>
    public int XPLevel
    {
        get => xpLevel ?? 0; 
        set => xpLevel = value; 
    }
    /// <summary> Gets or sets the amount of the player's XP points. </summary>
    public int XPTotal
    {
        get => xpTotal ?? 0; 
        set => xpTotal = value; 
    }
    /// <summary> Gets or sets the hunger level of the player.  Valid values range 0 - 20. </summary>
    public int HungerLevel
    {
        get => foodLevel ?? 0; 
        set => foodLevel = value; 
    }
    /// <summary> Gets or sets the player's hunger saturation level, which is reserve food capacity above <see cref="HungerLevel"/>. </summary>
    public float HungerSaturationLevel
    {
        get => foodSaturation ?? 0; 
        set => foodSaturation = value; 
    }
    /// <summary> Gets or sets the counter towards the next hunger point decrement.  Valid values range 0.0 - 4.0. </summary>
    public float HungerExhaustionLevel
    {
        get => foodExhaustion ?? 0; 
        set => foodExhaustion = value; 
    }
    /// <summary> Gets or sets the timer used to periodically heal or damage the player based on <see cref="HungerLevel"/>.  Valid values range 0 - 80. </summary>
    public int HungerTimer
    {
        get => foodTickTimer ?? 0; 
        set => foodTickTimer = value; 
    }
    /// <summary> Gets the state of the player's abilities. </summary>
    public PlayerAbilities Abilities
    {
        get => abilities; 
    }
    /// <summary> Creates a new <see cref="Player"/> object with reasonable default values. </summary>
    public Player () : base()
    {
        inventory = new ItemCollection(_CAPACITY);
        enderItems = new ItemCollection(_ENDER_CAPACITY);
        abilities = new PlayerAbilities();

        // Sane defaults
        dimension = 0;
        sleeping = 0;
        sleepTimer = 0;

        Air = 300;
        Health = 20.0f;
        Fire = -20;
    }

    /// <summary> Creates a copy of a <see cref="Player"/> object. </summary>
    /// <param name="p">The <see cref="Player"/> to copy fields from.</param>
    protected Player (Player p) : base(p)
    {
        attackTime = p.attackTime;
        deathTime = p.deathTime;
        health = p.health;
        hurtTime = p.hurtTime;

        dimension = p.dimension;
        gameMode = p.gameMode;
        sleeping = p.sleeping;
        sleepTimer = p.sleepTimer;
        spawnX = p.spawnX;
        spawnY = p.spawnY;
        spawnZ = p.spawnZ;
        world = p.world;
        inventory = p.inventory.Copy();
        enderItems = p.inventory.Copy();

        foodLevel = p.foodLevel;
        foodTickTimer = p.foodTickTimer;
        foodSaturation = p.foodSaturation;
        foodExhaustion = p.foodExhaustion;
        xpP = p.xpP;
        xpLevel = p.xpLevel;
        xpTotal = p.xpTotal;
        abilities = p.abilities.Copy();
    }

    /// <summary> Clears the player's personal spawn point. </summary>
    public void ClearSpawn ()
    {
        spawnX = null;
        spawnY = null;
        spawnZ = null;
    }

    private bool AbilitiesSet () => abilities.Flying || abilities.InstantBuild || abilities.MayFly || abilities.Invulnerable;
    /// <summary> Gets a <see cref="SchemaNode"/> representing the schema of a Player. </summary>
    public static new SchemaNodeCompound Schema { get; private set; }
    /// <summary> Attempt to load a Player subtree into the <see cref="Player"/> without validation. </summary>
    /// <param name="tree">The root node of a Player subtree.</param>
    /// <returns>The <see cref="Player"/> returns itself on success, or null if the tree was unparsable.</returns>
    public virtual new Player LoadTree (TagNode tree)
    {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if (ctree == null || base.LoadTree(tree) == null)
            return null;

        attackTime = ctree["AttackTime"].ToTagShort();
        deathTime = ctree["DeathTime"].ToTagShort();
        health = ctree["Health"].ToTagFloat();
        hurtTime = ctree["HurtTime"].ToTagShort();

        dimension = ctree["Dimension"].ToTagInt();
        sleeping = ctree["Sleeping"].ToTagByte();
        sleepTimer = ctree["SleepTimer"].ToTagShort();

        if (ctree.ContainsKey("SpawnX"))
            spawnX = ctree["SpawnX"].ToTagInt();
        if (ctree.ContainsKey("SpawnY"))
            spawnY = ctree["SpawnY"].ToTagInt();
        if (ctree.ContainsKey("SpawnZ"))
            spawnZ = ctree["SpawnZ"].ToTagInt();
        if (ctree.ContainsKey("World")) 
            world = ctree["World"].ToTagString();
        if (ctree.ContainsKey("foodLevel"))
            foodLevel = ctree["foodLevel"].ToTagInt();
        if (ctree.ContainsKey("foodTickTimer"))
            foodTickTimer = ctree["foodTickTimer"].ToTagInt();
        if (ctree.ContainsKey("foodExhaustionLevel"))
            foodExhaustion = ctree["foodExhaustionLevel"].ToTagFloat();
        if (ctree.ContainsKey("foodSaturationLevel"))
            foodSaturation = ctree["foodSaturationLevel"].ToTagFloat();
        if (ctree.ContainsKey("XpP")) 
            xpP = ctree["XpP"].ToTagFloat();
        if (ctree.ContainsKey("XpLevel")) 
            xpLevel = ctree["XpLevel"].ToTagInt();
        if (ctree.ContainsKey("XpTotal")) 
            xpTotal = ctree["XpTotal"].ToTagInt();
        if (ctree.ContainsKey("Score")) 
            score = ctree["Score"].ToTagInt();

        if (ctree.ContainsKey("abilities")) {
            TagNodeCompound pb = ctree["abilities"].ToTagCompound();

            abilities = new PlayerAbilities();
            abilities.Flying = pb["flying"].ToTagByte().Data == 1;
            abilities.InstantBuild = pb["instabuild"].ToTagByte().Data == 1;
            abilities.MayFly = pb["mayfly"].ToTagByte().Data == 1;
            abilities.Invulnerable = pb["invulnerable"].ToTagByte().Data == 1;

            if (pb.ContainsKey("mayBuild"))
                abilities.MayBuild = pb["mayBuild"].ToTagByte().Data == 1;
            if (pb.ContainsKey("walkSpeed"))
                abilities.WalkSpeed = pb["walkSpeed"].ToTagFloat();
            if (pb.ContainsKey("flySpeed"))
                abilities.FlySpeed = pb["flySpeed"].ToTagFloat();
        }

        if (ctree.ContainsKey("PlayerGameType"))    
            gameMode = (PlayerGameMode)ctree["PlayerGameType"].ToTagInt().Data;

        inventory.LoadTree(ctree["Inventory"].ToTagList());

        if (ctree.ContainsKey("EnderItems") && ctree["EnderItems"].ToTagList().Count > 0)
            enderItems.LoadTree(ctree["EnderItems"].ToTagList());

        return this;
    }
    /// <summary> Attempt to load a Player subtree into the <see cref="Player"/> with validation. </summary>
    /// <param name="tree">The root node of a Player subtree.</param>
    /// <returns>The <see cref="Player"/> returns itself on success, or null if the tree failed validation.</returns>
    public virtual new Player LoadTreeSafe(TagNode tree) => ValidateTree(tree) ? LoadTree(tree) : null;
    /// <summary> Builds a Player subtree from the current data. </summary>
    /// <returns>The root node of a Player subtree representing the current data.</returns>
    public virtual new TagNode BuildTree ()
    {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["AttackTime"] = new TagNodeShort(attackTime);
        tree["DeathTime"] = new TagNodeShort(deathTime);
        tree["Health"] = new TagNodeFloat(health);
        tree["HurtTime"] = new TagNodeShort(hurtTime);

        tree["Dimension"] = new TagNodeInt(dimension);
        tree["Sleeping"] = new TagNodeByte(sleeping);
        tree["SleepTimer"] = new TagNodeShort(sleepTimer);

        if (spawnX != null && spawnY != null && spawnZ != null) {
            tree["SpawnX"] = new TagNodeInt(spawnX ?? 0);
            tree["SpawnY"] = new TagNodeInt(spawnY ?? 0);
            tree["SpawnZ"] = new TagNodeInt(spawnZ ?? 0);
        }
        else {
            tree.Remove("SpawnX");
            tree.Remove("SpawnY");
            tree.Remove("SpawnZ");
        }

        if (world != null)
            tree["World"] = new TagNodeString(world);

        if (foodLevel != null)
            tree["foodLevel"] = new TagNodeInt(foodLevel ?? 0);
        if (foodTickTimer != null)
            tree["foodTickTimer"] = new TagNodeInt(foodTickTimer ?? 0);
        if (foodExhaustion != null)
            tree["foodExhaustionLevel"] = new TagNodeFloat(foodExhaustion ?? 0);
        if (foodSaturation != null)
            tree["foodSaturation"] = new TagNodeFloat(foodSaturation ?? 0);
        if (xpP != null)
            tree["XpP"] = new TagNodeFloat(xpP ?? 0);
        if (xpLevel != null)
            tree["XpLevel"] = new TagNodeInt(xpLevel ?? 0);
        if (xpTotal != null)
            tree["XpTotal"] = new TagNodeInt(xpTotal ?? 0);
        if (score != null)
            tree["Score"] = new TagNodeInt(score ?? 0);

        if (gameMode != null)
            tree["playerGameType"] = new TagNodeInt((int)(gameMode ?? PlayerGameMode.Survival));

        if (AbilitiesSet()) {
            TagNodeCompound pb = new TagNodeCompound();
            pb["flying"] = new TagNodeByte(abilities.Flying ? (byte)1 : (byte)0);
            pb["instabuild"] = new TagNodeByte(abilities.InstantBuild ? (byte)1 : (byte)0);
            pb["mayfly"] = new TagNodeByte(abilities.MayFly ? (byte)1 : (byte)0);
            pb["invulnerable"] = new TagNodeByte(abilities.Invulnerable ? (byte)1 : (byte)0);
            pb["mayBuild"] = new TagNodeByte(abilities.MayBuild ? (byte)1 : (byte)0);
            pb["walkSpeed"] = new TagNodeFloat(abilities.WalkSpeed);
            pb["flySpeed"] = new TagNodeFloat(abilities.FlySpeed);

            tree["abilities"] = pb;
        }

        tree["Inventory"] = inventory.BuildTree();
        tree["EnderItems"] = enderItems.BuildTree();

        return tree;
    }
    /// <summary> Validate a Player subtree against a schema defintion. </summary>
    /// <param name="tree">The root node of a Player subtree.</param>
    /// <returns>Status indicating whether the tree was valid against the internal schema.</returns>
    public virtual new bool ValidateTree (TagNode tree) => new NbtVerifier(tree, schema).Verify();
    /// <summary> Creates a deep-copy of the <see cref="Player"/>. </summary>
    /// <returns>A deep-copy of the <see cref="Player"/>.</returns>
    public virtual new Player Copy() => new Player(this);
    /// <summary> Gets access to an <see cref="ItemCollection"/> representing the player's equipment and inventory. </summary>
    public ItemCollection Items { get; private set; }    public ItemCollection EnderItems { get; private set; }
}