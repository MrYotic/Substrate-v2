namespace Substrate_v2.Item;
/// <summary> Represents a collection of items, such as a chest or an inventory. </summary>
/// <remarks>ItemCollections have a limited number of slots that depends on where they are used.</remarks>
public class ItemCollection : INbtObject<ItemCollection>, ICopyable<ItemCollection>
{
    private static readonly SchemaNodeList _listSchema = new SchemaNodeList("", TagType.TAG_COMPOUND, _schema);
    private Dictionary<int, Item> items;
    /// <summary>
    /// Constructs an <see cref="ItemCollection"/> with at most <paramref name="capacity"/> item slots.
    /// </summary>
    /// <param name="capacity">The upper bound on item slots available.</param>
    /// <remarks>The <paramref name="capacity"/> parameter does not necessarily indicate the true capacity of an item collection.
    /// The player object, for example, contains a conventional inventory, a range of invalid slots, and then equipment.  Capacity in
    /// this case would refer to the highest equipment slot.</remarks>
    public ItemCollection (int capacity)
    {
        Capacity = capacity;
        items = new Dictionary<int, Item>();
    }
    /// <summary> Gets the capacity of the item collection. </summary>
    public int Capacity { get; private set; }
    /// <summary> Gets the current number of item slots actually used in the collection. </summary>
    public int Count { get; private set; }
    /// <summary> Gets or sets an item in a given item slot. </summary>
    /// <param name="slot">The item slot to query or insert an item or item stack into.</param>
    public Item this[int slot]
    {
        get
        {
            Item item;
            items.TryGetValue(slot, out item);
            return item;
        }
        set
        {
            if (slot < 0 || slot >= Capacity)
                return;
            items[slot] = value;
        }
    }
    /// <summary> Gets a <see cref="SchemaNode"/> representing the schema of an item collection. </summary>
    public static SchemaNodeCompound Schema { get; private set; } = Item.Schema.MergeInto(new SchemaNodeCompound("")
    {
        new SchemaNodeScaler("Slot", TagType.TAG_BYTE),
    });
    /// <summary> Checks if an item exists in the given item slot. </summary>
    /// <param name="slot">The item slot to check.</param>
    /// <returns>True if an item or stack of items exists in the given slot.</returns>
    public bool ItemExists (int slot) => items.ContainsKey(slot);
    /// <summary> Removes an item from the given item slot, if it exists. </summary>
    /// <param name="slot">The item slot to clear.</param>
    /// <returns>True if an item was removed; false otherwise.</returns>
    public bool Clear (int slot) => items.Remove(slot);
    /// <summary> Removes all items from the item collection. </summary>
    public void ClearAllItems () => items.Clear();
    public ItemCollection Copy ()
    {
        ItemCollection ic = new ItemCollection(Capacity);
        foreach (KeyValuePair<int, Item> item in items)
            ic[item.Key] = item.Value.Copy();
        return ic;
    }
    public ItemCollection LoadTree (TagNode tree)
    {
        TagNodeList ltree = tree as TagNodeList;
        if (ltree == null)
            return null;
        foreach (TagNodeCompound item in ltree) {
            int slot = item["Slot"].ToTagByte();
            _items[slot] = new Item().LoadTree(item);
        }
        return this;
    }
    public ItemCollection LoadTreeSafe(TagNode tree) => ValidateTree(tree) ? LoadTree(tree) : null;
    public TagNode BuildTree ()
    {
        TagNodeList list = new TagNodeList(TagType.TAG_COMPOUND);
        foreach (KeyValuePair<int, Item> item in items) {
            TagNodeCompound itemtree = item.Value.BuildTree() as TagNodeCompound;
            itemtree["Slot"] = new TagNodeByte((byte)item.Key);
            list.Add(itemtree);
        }
        return list;
    }
    public bool ValidateTree (TagNode tree) => new NbtVerifier(tree, _listSchema).Verify();
}