namespace Substrate_v2.Entity;
/// <summary> A base entity type for all entities except <see cref="Player"/> entities. </summary>
/// <remarks>Generally, this class should be subtyped into new concrete Entity types, as this generic type is unable to
/// capture any of the custom data fields.  It is however still possible to create instances of <see cref="Entity"/> objects, 
/// which may allow for graceful handling of unknown Entity types.</remarks>
public class TypedEntity : Entity, INbtObject<TypedEntity>, ICopyable<TypedEntity>
{
    /// <summary> Gets the id (type) of the entity. </summary>
    public string ID { get; private set; }
    /// <summary> Creates a new generic <see cref="TypedEntity"/> with the given id. </summary>
    /// <param name="id">The id (name) of the Entity.</param>
    public TypedEntity(string id) : base()
    {
        ID = id;
    }
    /// <summary> Constructs a new <see cref="TypedEntity"/> by copying an existing one. </summary>
    /// <param name="e">The <see cref="TypedEntity"/> to copy.</param>
    protected TypedEntity(TypedEntity e) : base(e)
    {
        ID = e.ID;
    }
    #region INBTObject<Entity> Members
    /// <summary> Gets a <see cref="SchemaNode"/> representing the basic schema of an Entity. </summary>
    public static new SchemaNodeCompound Schema { get; } = Entity.Schema.MergeInto(new SchemaNodeCompound("")
    {
        new SchemaNodeScaler("id", TagType.TAG_STRING),
    });
    /// <summary> Attempt to load an Entity subtree into the <see cref="TypedEntity"/> without validation. </summary>
    /// <param name="tree">The root node of an Entity subtree.</param>
    /// <returns>The <see cref="TypedEntity"/> returns itself on success, or null if the tree was unparsable.</returns>
    public virtual new TypedEntity LoadTree(TagNode tree)
    {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if (ctree == null || base.LoadTree(tree) == null)
            return null;
        ID = ctree["id"].ToTagString();
        return this;
    }
    /// <summary> Attempt to load an Entity subtree into the <see cref="TypedEntity"/> with validation. </summary>
    /// <param name="tree">The root node of an Entity subtree.</param>
    /// <returns>The <see cref="TypedEntity"/> returns itself on success, or null if the tree failed validation.</returns>
    public virtual new TypedEntity LoadTreeSafe(TagNode tree) => ValidateTree(tree) ? LoadTree(tree) : null;
    /// <summary> Builds an Entity subtree from the current data. </summary>
    /// <returns>The root node of an Entity subtree representing the current data.</returns>
    public virtual new TagNode BuildTree()
    {
        TagNodeCompound tree = base.BuildTree() as TagNodeCompound;
        tree["id"] = new TagNodeString(ID);
        return tree;
    }
    /// <summary> Validate an Entity subtree against a basic schema. </summary>
    /// <param name="tree">The root node of an Entity subtree.</param>
    /// <returns>Status indicating whether the tree was valid against the internal schema.</returns>
    public virtual new bool ValidateTree(TagNode tree) => new NbtVerifier(tree, _schema).Verify();
    #endregion
    #region ICopyable<Entity> Members
    /// <summary> Creates a deep-copy of the <see cref="TypedEntity"/>. </summary>
    /// <returns>A deep-copy of the <see cref="TypedEntity"/>.</returns>
    public virtual new TypedEntity Copy() => new TypedEntity(this);
    #endregion
}