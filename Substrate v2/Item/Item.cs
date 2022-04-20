namespace Substrate_v2;
/// <summary> Represents an item (or item stack) within an item slot. </summary>
public class Item : INbtObject<Item>, ICopyable<Item>
{
    private List<Enchantment> enchantments;
    /// <summary> Constructs an empty <see cref="Item"/> instance. </summary>
    public Item ()
    {
        enchantments = new List<Enchantment>();
        Source = new TagNodeCompound();
    }
    /// <summary> Constructs an <see cref="Item"/> instance representing the given item id. </summary>
    /// <param name="id">An item id.</param>
    public Item (int id) : this() => ID = (short)id;
    /// <summary> Gets an <see cref="ItemInfo"/> entry for this item's type. </summary>
    public ItemInfo Info
    {
        get => ItemInfo.ItemTable[ID];
    }
    /// <summary> Gets or sets the current type (id) of the item. </summary>
    public int ID { get; set; }
    /// <summary> Gets or sets the damage value of the item. </summary>
    /// <remarks>The damage value may represent a generic data value for some items.</remarks>
    public int Damage { get; set; }
    /// <summary>
    /// Gets or sets the number of this item stacked together in an item slot.
    /// </summary>
    public int Count { get; set; }
    /// <summary>
    /// Gets the list of <see cref="Enchantment"/>s applied to this item.
    /// </summary>
    public IList<Enchantment> Enchantments
    {
        get => enchantments;
    }
    /// <summary>
    /// Gets the source <see cref="TagNodeCompound"/> used to create this <see cref="Item"/> if it exists.
    /// </summary>
    public TagNodeCompound Source { get; private set; }
    /// <summary>
    /// Gets a <see cref="SchemaNode"/> representing the schema of an item.
    /// </summary>
    public static SchemaNodeCompound Schema { get; private set; } = new SchemaNodeCompound("")
    {
        new SchemaNodeScaler("id", TagType.TAG_SHORT),
        new SchemaNodeScaler("Damage", TagType.TAG_SHORT),
        new SchemaNodeScaler("Count", TagType.TAG_BYTE),
        new SchemaNodeCompound("tag", new SchemaNodeCompound("") {
            new SchemaNodeList("ench", TagType.TAG_COMPOUND, Enchantment.Schema, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("title", TagType.TAG_STRING, SchemaOptions.OPTIONAL),
            new SchemaNodeScaler("author", TagType.TAG_STRING, SchemaOptions.OPTIONAL),
            new SchemaNodeList("pages", TagType.TAG_STRING, SchemaOptions.OPTIONAL),
        }, SchemaOptions.OPTIONAL),
    };
    public Item Copy ()
    {
        Item item = new Item()
        {
            ID = ID,
            Count = Count,
            Damage = Damage,
        };
        foreach (Enchantment e in enchantments)
            item.enchantments.Add(e.Copy());
        if (Source != null)
            item.Source = Source.Copy() as TagNodeCompound;
        return item;
    }
    public Item LoadTree (TagNode tree)
    {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if (ctree == null)
            return null;
        enchantments.Clear();
        ID = ctree["id"].ToTagShort();
        Count = ctree["Count"].ToTagByte();
        Damage = ctree["Damage"].ToTagShort();
        if (ctree.ContainsKey("tag")) {
            TagNodeCompound tagtree = ctree["tag"].ToTagCompound();
            if (tagtree.ContainsKey("ench")) {
                TagNodeList enchList = tagtree["ench"].ToTagList();

                foreach (TagNode tag in enchList)
                    enchantments.Add(new Enchantment().LoadTree(tag));
            }
        }
        Source = ctree.Copy() as TagNodeCompound;
        return this;
    }
    public Item LoadTreeSafe(TagNode tree) => ValidateTree(tree) ? LoadTree(tree) : null;
    public TagNode BuildTree ()
    {
        TagNodeCompound tree = new TagNodeCompound();
        tree["id"] = new TagNodeShort(ID);
        tree["Count"] = new TagNodeByte(Count);
        tree["Damage"] = new TagNodeShort(Damage);
        if (_enchantments.Count > 0) {
            TagNodeList enchList = new TagNodeList(TagType.TAG_COMPOUND);
            foreach (Enchantment e in _enchantments)
                enchList.Add(e.BuildTree());            
            TagNodeCompound tagtree = new TagNodeCompound();
            tagtree["ench"] = enchList;
            if (Source != null && Source.ContainsKey("tag")) 
                tagtree.MergeFrom(Source["tag"].ToTagCompound());
            tree["tag"] = tagtree;
        }
        if (Source != null)
            tree.MergeFrom(Source);

        return tree;
    }
    public bool ValidateTree (TagNode tree) => new NbtVerifier(tree, Schema).Verify();
}