namespace Substrate_v2;
/// <summary> The base Entity type for Minecraft Entities, providing access to data common to all Minecraft Entities. </summary>
public class Entity : INbtObject<Entity>, ICopyable<Entity>
{
    private static readonly SchemaNodeCompound _schema = new SchemaNodeCompound("")
    {
        new SchemaNodeList("Pos", TagType.TAG_DOUBLE, 3),
        new SchemaNodeList("Motion", TagType.TAG_DOUBLE, 3),
        new SchemaNodeList("Rotation", TagType.TAG_FLOAT, 2),
        new SchemaNodeScaler("FallDistance", TagType.TAG_FLOAT),
        new SchemaNodeScaler("Fire", TagType.TAG_SHORT),
        new SchemaNodeScaler("Air", TagType.TAG_SHORT),
        new SchemaNodeScaler("OnGround", TagType.TAG_BYTE),
    };

    private byte onGround;
    /// <summary>  Gets or sets the global position of the entity in fractional block coordinates. </summary>
    public Vector3 Position { get; set; }
    /// <summary> Gets or sets the velocity of the entity. </summary>
    public Vector3 Motion { get; set; }
    /// <summary> Gets or sets the orientation of the entity. </summary>
    public Orientation Rotation { get; set; }
    /// <summary> Gets or sets the distance that the entity has fallen, if it is falling. </summary>
    public double FallDistance { get; set; }
    /// <summary> Gets or sets the fire counter of the entity. </summary>
    public int Fire { get; set; }
    /// <summary> Gets or sets the remaining air availale to the entity. </summary>
    public int Air { get; set; }
    /// <summary> Gets or sets a value indicating whether the entity is currently touch the ground. </summary>
    public bool IsOnGround
    {
        get => onGround == 1;
        set => onGround = (byte)(value ? 1 : 0);
    }
    /// <summary> Gets the source <see cref="TagNodeCompound"/> used to create this <see cref="Entity"/> if it exists. </summary>
    public TagNodeCompound Source { get; }
    /// <summary> Constructs a new generic <see cref="Entity"/> with default values. </summary>
    public Entity ()
    {
        Position = new Vector3();
        Motion = new Vector3();
        Rotation = new Orientation();
        Source = new TagNodeCompound();
    }
    /// <summary> Constructs a new generic <see cref="Entity"/> by copying fields from another <see cref="Entity"/> object. </summary>
    /// <param name="e">An <see cref="Entity"/> to copy fields from.</param>
    protected Entity (Entity e)
    {
        Position = new Vector3();
        Position.X = e._pos.X;
        Position.Y = e._pos.Y;
        Position.Z = e._pos.Z;

        Motion = new Vector3();
        Motion.X = e._motion.X;
        Motion.Y = e._motion.Y;
        Motion.Z = e._motion.Z;

        Rotation = new Orientation();
        Rotation.Pitch = e._rotation.Pitch;
        Rotation.Yaw = e._rotation.Yaw;

        FallDistance = e._fallDistance;
        Fire = e._fire;
        Air = e._air;
        onGround = e._onGround;

        if (e._source != null)
            Source = e._source.Copy() as TagNodeCompound;
    }
    /// <summary> Moves the <see cref="Entity"/> by given block offsets. </summary>
    /// <param name="diffX">The X-offset to move by, in blocks.</param>
    /// <param name="diffY">The Y-offset to move by, in blocks.</param>
    /// <param name="diffZ">The Z-offset to move by, in blocks.</param>
    public virtual void MoveBy (int diffX, int diffY, int diffZ)
    {
        Position.X += diffX;
        Position.Y += diffY;
        Position.Z += diffZ;
    }
    #region INBTObject<Entity> Members
    /// <summary> Gets a <see cref="SchemaNode"/> representing the basic schema of an Entity. </summary>
    public static SchemaNodeCompound Schema { get; }
    /// <summary> Attempt to load an Entity subtree into the <see cref="Entity"/> without validation. </summary>
    /// <param name="tree">The root node of an Entity subtree.</param>
    /// <returns>The <see cref="Entity"/> returns itself on success, or null if the tree was unparsable.</returns>
    public Entity LoadTree (TagNode tree)
    {
        TagNodeCompound ctree = tree as TagNodeCompound;
        if (ctree == null)
            return null;

        TagNodeList pos = ctree["Pos"].ToTagList();
        Position = new Vector3();
        Position.X = pos[0].ToTagDouble();
        Position.Y = pos[1].ToTagDouble();
        Position.Z = pos[2].ToTagDouble();

        TagNodeList motion = ctree["Motion"].ToTagList();
        Motion = new Vector3();
        Motion.X = motion[0].ToTagDouble();
        Motion.Y = motion[1].ToTagDouble();
        Motion.Z = motion[2].ToTagDouble();

        TagNodeList rotation = ctree["Rotation"].ToTagList();
        Rotation = new Orientation();
        Rotation.Yaw = rotation[0].ToTagFloat();
        Rotation.Pitch = rotation[1].ToTagFloat();

        Fire = ctree["Fire"].ToTagShort();
        Air = ctree["Air"].ToTagShort();
        onGround = ctree["OnGround"].ToTagByte();

        Source = ctree.Copy() as TagNodeCompound;

        return this;
    }

    /// <summary> Attempt to load an Entity subtree into the <see cref="Entity"/> with validation. </summary>
    /// <param name="tree">The root node of an Entity subtree.</param>
    /// <returns>The <see cref="Entity"/> returns itself on success, or null if the tree failed validation.</returns>
    public Entity LoadTreeSafe(TagNode tree) => ValidateTree(tree) ? LoadTree(tree) : null;
    /// <summary> Builds an Entity subtree from the current data. </summary>
    /// <returns>The root node of an Entity subtree representing the current data.</returns>
    public TagNode BuildTree ()
    {
        TagNodeCompound tree = new TagNodeCompound();

        TagNodeList pos = new TagNodeList(TagType.TAG_DOUBLE);
        pos.Add(new TagNodeDouble(_pos.X));
        pos.Add(new TagNodeDouble(_pos.Y));
        pos.Add(new TagNodeDouble(_pos.Z));
        tree["Pos"] = pos;

        TagNodeList motion = new TagNodeList(TagType.TAG_DOUBLE);
        motion.Add(new TagNodeDouble(_motion.X));
        motion.Add(new TagNodeDouble(_motion.Y));
        motion.Add(new TagNodeDouble(_motion.Z));
        tree["Motion"] = motion;

        TagNodeList rotation = new TagNodeList(TagType.TAG_FLOAT);
        rotation.Add(new TagNodeFloat((float)_rotation.Yaw));
        rotation.Add(new TagNodeFloat((float)_rotation.Pitch));
        tree["Rotation"] = rotation;

        tree["FallDistance"] = new TagNodeFloat(_fallDistance);
        tree["Fire"] = new TagNodeShort(_fire);
        tree["Air"] = new TagNodeShort(_air);
        tree["OnGround"] = new TagNodeByte(_onGround);

        if (_source != null) 
            tree.MergeFrom(_source);

        return tree;
    }

    /// <summary> Validate an Entity subtree against a basic schema. </summary>
    /// <param name="tree">The root node of an Entity subtree.</param>
    /// <returns>Status indicating whether the tree was valid against the internal schema.</returns>
    public bool ValidateTree (TagNode tree) => new NbtVerifier(tree, Schema).Verify();
    #endregion

    #region ICopyable<Entity> Members
    /// <summary> Creates a deep-copy of the <see cref="Entity"/>. </summary>
    /// <returns>A deep-copy of the <see cref="Entity"/>.</returns>
    public Entity Copy() => new Entity(this);
    #endregion
}