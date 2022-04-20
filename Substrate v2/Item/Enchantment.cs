namespace Substrate_v2;
/// <summary> Represents an enchantment that can be applied to some <see cref="Item"/>s. </summary>
public class Enchantment : INbtObject<Enchantment>, ICopyable<Enchantment>
{
    private static readonly SchemaNodeCompound _schema = new SchemaNodeCompound("")
    {
        new SchemaNodeScaler("id", TagType.TAG_SHORT),
        new SchemaNodeScaler("lvl", TagType.TAG_SHORT),
    };
    /// <summary> Constructs a blank <see cref="Enchantment"/>. </summary>
    public Enchantment () { }
    private TagNodeCompound source;
    /// <summary> Constructs an <see cref="Enchantment"/> from a given id and level. </summary>
    /// <param name="id">The id (type) of the enchantment.</param>
    /// <param name="level">The level of the enchantment.</param>
    public Enchantment (int id, int level)
    {
        ID = (short)id;
        Level = (short)level;
    }
    /// <summary> Gets an <see cref="EnchantmentInfo"/> entry for this enchantment's type. </summary>
    public EnchantmentInfo Info
    {
        get => EnchantmentInfo.EnchantmentTable[ID];
    }
    /// <summary> Gets or sets the current type (id) of the enchantment. </summary>
    public int ID { get; set; }
    /// <summary> Gets or sets the level of the enchantment. </summary>
    public int Level { get; set; }
    /// <summary> Gets a <see cref="SchemaNode"/> representing the schema of an enchantment. </summary>
    public static SchemaNodeCompound Schema { get; private set; }
    public Enchantment LoadTree (TagNode tree)
    {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if (ctree == null)
            return null;
        ID = ctree["id"].ToTagShort();
        Level = ctree["lvl"].ToTagShort();
        source = ctree.Copy() as TagNodeCompound;
        return this;
    }
    public Enchantment LoadTreeSafe(TagNode tree) => ValidateTree(tree) ? LoadTree(tree) : null;
    public TagNode BuildTree ()
    {
        TagNodeCompound tree = new TagNodeCompound();
        tree["id"] = new TagNodeShort(ID);
        tree["lvl"] = new TagNodeShort(Level);
        if (source != null)
            tree.MergeFrom(source);
        return tree;
    }
    public bool ValidateTree (TagNode tree) => new NbtVerifier(tree, _schema).Verify();
    public Enchantment Copy ()
    {
        Enchantment ench = new Enchantment(ID, Level);
        if (source != null)
            ench.source = source.Copy() as TagNodeCompound;
        return ench;
    }
}
