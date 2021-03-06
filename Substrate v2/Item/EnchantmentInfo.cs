namespace Substrate_v2.Item;
/// <summary> Provides information on a specific type of enchantment. </summary>
/// <remarks>By default, all known MC enchantment types are already defined and registered, assuming Substrate
/// is up to date with the current MC version.
/// New enchantment types may be created and used at runtime, and will automatically populate various static lookup tables
/// in the <see cref="EnchantmentInfo"/> class.</remarks>
public class EnchantmentInfo
{
    private static Random rnd = new Random();
    private class CacheTableDict<T> : ICacheTable<T>
    {
        private Dictionary<int, T> cache;

        public T this[int index]
        {
            get
            {
                T val;
                if (cache.TryGetValue(index, out val))
                    return val;
                return default(T);
            }
        }

        public CacheTableDict (Dictionary<int, T> cache) => this.cache = cache;
        public IEnumerator<T> GetEnumerator ()
        {
            foreach (T val in cache.Values)
                yield return val;
        }
        System.Collections.IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    private static readonly Dictionary<int, EnchantmentInfo> enchantTable;
    private static readonly CacheTableDict<EnchantmentInfo> enchantTableCache;
    /// <summary> Gets the lookup table for id-to-info values. </summary>
    public static ICacheTable<EnchantmentInfo> EnchantmentTable
    {
        get => enchantTableCache;
    }
    /// <summary> Gets the id of the enchantment type. </summary>
    public int ID { get; private set; } = 0;
    /// <summary> Gets the name of the enchantment type. </summary>
    public string Name { get; private set; } = "";
    /// <summary> Gets the maximum level allowed for this enchantment type. </summary>
    public int MaxLevel { get; private set; } = 0;
    /// <summary> Constructs a new <see cref="EnchantmentInfo"/> record for the given enchantment id. </summary>
    /// <param name="id">The id of an item type.</param>
    public EnchantmentInfo (int id)
    {
        ID = id;
        enchantTable[ID] = this;
    }
    /// <summary> Constructs a new <see cref="EnchantmentInfo"/> record for the given enchantment id and name. </summary>
    /// <param name="id">The id of an item type.</param>
    /// <param name="name">The name of an item type.</param>
    public EnchantmentInfo (int id, string name)
    {
        ID = id;
        Name = name;
        enchantTable[ID] = this;
    }
    /// <summary> Sets the maximum level for this enchantment type. </summary>
    /// <param name="level">The maximum allowed level.</param>
    /// <returns>The object instance used to invoke this method.</returns>
    public EnchantmentInfo SetMaxLevel (int level)
    {
        MaxLevel = level;
        return this;
    }

    /// <summary> Chooses a registered enchantment type at random and returns it. </summary>
    /// <returns></returns>
    public static EnchantmentInfo GetRandomEnchantment ()
    {
        List<EnchantmentInfo> list = new List<EnchantmentInfo>(enchantTable.Values);
        return list[rnd.Next(list.Count)];
    }
    public static EnchantmentInfo Protection;
    public static EnchantmentInfo FireProtection;
    public static EnchantmentInfo FeatherFalling;
    public static EnchantmentInfo BlastProtection;
    public static EnchantmentInfo ProjectileProtection;
    public static EnchantmentInfo Respiration;
    public static EnchantmentInfo AquaAffinity;
    public static EnchantmentInfo Thorns;
    public static EnchantmentInfo Sharpness;
    public static EnchantmentInfo Smite;
    public static EnchantmentInfo BaneOfArthropods;
    public static EnchantmentInfo Knockback;
    public static EnchantmentInfo FireAspect;
    public static EnchantmentInfo Looting;
    public static EnchantmentInfo Efficiency;
    public static EnchantmentInfo SilkTouch;
    public static EnchantmentInfo Unbreaking;
    public static EnchantmentInfo Fortune;
    public static EnchantmentInfo Power;
    public static EnchantmentInfo Punch;
    public static EnchantmentInfo Flame;
    public static EnchantmentInfo Infinity;
    static EnchantmentInfo ()
    {
        enchantTable = new Dictionary<int, EnchantmentInfo>();
        enchantTableCache = new CacheTableDict<EnchantmentInfo>(enchantTable);

        Protection = new EnchantmentInfo(EnchantmentType.PROTECTION, "Protection").SetMaxLevel(4);
        FireProtection = new EnchantmentInfo(EnchantmentType.FIRE_PROTECTION, "Fire Protection").SetMaxLevel(4);
        FeatherFalling = new EnchantmentInfo(EnchantmentType.FEATHER_FALLING, "Feather Falling").SetMaxLevel(4);
        BlastProtection = new EnchantmentInfo(EnchantmentType.BLAST_PROTECTION, "Blast Protection").SetMaxLevel(4);
        ProjectileProtection = new EnchantmentInfo(EnchantmentType.PROJECTILE_PROTECTION, "Projectile Protection").SetMaxLevel(4);
        Respiration = new EnchantmentInfo(EnchantmentType.RESPIRATION, "Respiration").SetMaxLevel(3);
        AquaAffinity = new EnchantmentInfo(EnchantmentType.AQUA_AFFINITY, "Aqua Affinity").SetMaxLevel(1);
        Thorns = new EnchantmentInfo(EnchantmentType.THORNS, "Thorns").SetMaxLevel(3);
        Sharpness = new EnchantmentInfo(EnchantmentType.SHARPNESS, "Sharpness").SetMaxLevel(5);
        Smite = new EnchantmentInfo(EnchantmentType.SMITE, "Smite").SetMaxLevel(5);
        BaneOfArthropods = new EnchantmentInfo(EnchantmentType.BANE_OF_ARTHROPODS, "Bane of Arthropods").SetMaxLevel(5);
        Knockback = new EnchantmentInfo(EnchantmentType.KNOCKBACK, "Knockback").SetMaxLevel(2);
        FireAspect = new EnchantmentInfo(EnchantmentType.FIRE_ASPECT, "Fire Aspect").SetMaxLevel(2);
        Looting = new EnchantmentInfo(EnchantmentType.LOOTING, "Looting").SetMaxLevel(3);
        Efficiency = new EnchantmentInfo(EnchantmentType.EFFICIENCY, "Efficiency").SetMaxLevel(5);
        SilkTouch = new EnchantmentInfo(EnchantmentType.SILK_TOUCH, "Silk Touch").SetMaxLevel(1);
        Unbreaking = new EnchantmentInfo(EnchantmentType.UNBREAKING, "Unbreaking").SetMaxLevel(3);
        Fortune = new EnchantmentInfo(EnchantmentType.FORTUNE, "Fortune").SetMaxLevel(3);
        Power = new EnchantmentInfo(EnchantmentType.POWER, "Power").SetMaxLevel(5);
        Punch = new EnchantmentInfo(EnchantmentType.PUNCH, "Punch").SetMaxLevel(2);
        Flame = new EnchantmentInfo(EnchantmentType.FLAME, "Flame").SetMaxLevel(1);
        Infinity = new EnchantmentInfo(EnchantmentType.INFINITY, "Infinity").SetMaxLevel(1);
    }
}
