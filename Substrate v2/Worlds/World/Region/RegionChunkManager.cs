using System.Collections;

namespace Substrate_v2;
/// <summary> Represents a Beta-compatible interface for globally managing chunks. </summary>
public class RegionChunkManager : IChunkManager, IEnumerable<ChunkRef>
{
    private const int REGION_XLEN = 32;
    private const int REGION_ZLEN = 32;
    private const int REGION_XLOG = 5;
    private const int REGION_ZLOG = 5;
    private const int REGION_XMASK = 0x1F;
    private const int REGION_ZMASK = 0x1F;
    private IRegionManager _regionMan;
    private ChunkCache _cache;
    /// <summary> Creates a new <see cref="RegionChunkManager"/> instance given a backing <see cref="RegionManager"/> and <see cref="ChunkCache"/>. /summary>
    /// <param name="rm">A <see cref="RegionManager"/> exposing access to regions.</param>
    /// <param name="cache">A shared cache for storing chunks read in.</param>
    public RegionChunkManager (IRegionManager rm, ChunkCache cache)
    {
        _regionMan = rm;
        _cache = cache;
    }
    /// <summary> Creates a new <see cref="RegionChunkManager"/> instance from another. </summary>
    /// <param name="cm">A <see cref="RegionChunkManager"/> to get a <see cref="RegionManager"/> and <see cref="ChunkCache"/> from.</param>
    public RegionChunkManager (RegionChunkManager cm)
    {
        _regionMan = cm._regionMan;
        _cache = cm._cache;
    }
    /// <summary> Gets the <see cref="RegionManager"/> backing this manager. </summary>
    public IRegionManager RegionManager { get => _regionMan; }
    public int ChunkGlobalX(int cx) => cx;
    public int ChunkGlobalZ(int cz) => cz;
    public int ChunkLocalX (int cx) => cx & REGION_XMASK;
    public int ChunkLocalZ(int cz) => cz & REGION_ZMASK;
    public IChunk GetChunk (int cx, int cz)
    {
        IRegion r = GetRegion(cx, cz);
        if (r == null)
            return null;
        return r.GetChunk(cx & REGION_XMASK, cz & REGION_ZMASK);
    }
    public ChunkRef GetChunkRef (int cx, int cz)
    {
        IRegion r = GetRegion(cx, cz);
        if (r == null)
            return null;
        return r.GetChunkRef(cx & REGION_XMASK, cz & REGION_ZMASK);
    }
    public bool ChunkExists (int cx, int cz)
    {
        IRegion r = GetRegion(cx, cz);
        if (r == null) 
            return false;
        return r.ChunkExists(cx & REGION_XMASK, cz & REGION_ZMASK);
    }
    public ChunkRef CreateChunk (int cx, int cz)
    {
        IRegion r = GetRegion(cx, cz);
        if (r == null) {
            int rx = cx >> REGION_XLOG;
            int rz = cz >> REGION_ZLOG;
            r = _regionMan.CreateRegion(rx, rz);
        }
        return r.CreateChunk(cx & REGION_XMASK, cz & REGION_ZMASK);
    }
    public ChunkRef SetChunk (int cx, int cz, IChunk chunk)
    {
        IRegion r = GetRegion(cx, cz);
        if (r == null) {
            int rx = cx >> REGION_XLOG;
            int rz = cz >> REGION_ZLOG;
            r = _regionMan.CreateRegion(rx, rz);
        }
        chunk.SetLocation( cx, cz );
        r.SaveChunk(chunk);
        return r.GetChunkRef(cx & REGION_XMASK, cz & REGION_ZMASK);
    }
    public int Save ()
    {
        _cache.SyncDirty();
        int saved = 0;
        IEnumerator<ChunkRef> en = _cache.GetDirtyEnumerator();
        while (en.MoveNext()) {
            ChunkRef chunk = en.Current;
            IRegion r = GetRegion(chunk.X, chunk.Z);
            if (r == null)
                continue;
            using (Stream chunkOutStream = r.GetChunkOutStream(chunk.LocalX, chunk.LocalZ))
            {
                chunk.Save(chunkOutStream);
                saved++;
            }
        }
        _cache.ClearDirty();
        return saved;
    }
    public bool SaveChunk (IChunk chunk)
    {
        IRegion r = GetRegion(chunk.X, chunk.Z);
        if (r == null)
            return false;
        return r.SaveChunk(chunk);
    }
    public bool DeleteChunk (int cx, int cz)
    {
        IRegion r = GetRegion(cx, cz);
        if (r == null) 
            return false;
        if (!r.DeleteChunk(cx & REGION_XMASK, cz & REGION_ZMASK))
            return false;
        if (r.ChunkCount() == 0) 
            _regionMan.DeleteRegion(r.X, r.Z);
        return true;
    }
    public bool CanDelegateCoordinates { get => true; }
    /// <summary> Copies a chunk from one location to another. </summary>
    /// <param name="src_cx">The global X-coordinate of the source chunk.</param>
    /// <param name="src_cz">The global Z-coordinate of the source chunk.</param>
    /// <param name="dst_cx">The global X-coordinate of the destination chunk.</param>
    /// <param name="dst_cz">The global Z-coordinate of the destination chunk.</param>
    /// <returns>A <see cref="ChunkRef"/> for the destination chunk.</returns>
    public ChunkRef CopyChunk (int src_cx, int src_cz, int dst_cx, int dst_cz)
    {
        IRegion src_r = GetRegion(src_cx, src_cz);
        if (src_r == null) {
            return null;
        }
        IRegion dst_r = GetRegion(dst_cx, dst_cz);
        if (dst_r == null) {
            int rx = dst_cx >> REGION_XLOG;
            int rz = dst_cz >> REGION_ZLOG;
            dst_r = _regionMan.CreateRegion(rx, rz);
        }
        IChunk c = src_r.GetChunk(src_cx & REGION_XMASK, src_cz & REGION_ZMASK);
        c.SetLocation(dst_cx, dst_cz);
        dst_r.SaveChunk(c);
        return dst_r.GetChunkRef(dst_cx & REGION_XMASK, dst_cz & REGION_ZMASK);
    }
    /// <summary> Performs a full chunk relight sequence on all modified chunks. </summary>
    public void RelightDirtyChunks ()
    {
        Dictionary<ChunkKey, ChunkRef> dirty = new Dictionary<ChunkKey, ChunkRef>();
        _cache.SyncDirty();
        IEnumerator<ChunkRef> en = _cache.GetDirtyEnumerator();
        while (en.MoveNext()) {
            ChunkKey key = new ChunkKey(en.Current.X, en.Current.Z);
            dirty[key] = en.Current;
        }

        foreach (ChunkRef chunk in dirty.Values) {
            chunk.Blocks.ResetBlockLight();
            chunk.Blocks.ResetSkyLight();
        }

        foreach (ChunkRef chunk in dirty.Values) {
            chunk.Blocks.RebuildBlockLight();
            chunk.Blocks.RebuildSkyLight();
        }

        foreach (ChunkRef chunk in dirty.Values) {  
            if (!dirty.ContainsKey(new ChunkKey(chunk.X, chunk.Z - 1))) {
                ChunkRef east = chunk.GetEastNeighbor();
                chunk.Blocks.StitchBlockLight(east.Blocks, BlockCollectionEdge.EAST);
                chunk.Blocks.StitchSkyLight(east.Blocks, BlockCollectionEdge.EAST);
            }

            if (!dirty.ContainsKey(new ChunkKey(chunk.X, chunk.Z + 1))) {
                ChunkRef west = chunk.GetWestNeighbor();
                chunk.Blocks.StitchBlockLight(west.Blocks, BlockCollectionEdge.WEST);
                chunk.Blocks.StitchSkyLight(west.Blocks, BlockCollectionEdge.WEST);
            }

            if (!dirty.ContainsKey(new ChunkKey(chunk.X - 1, chunk.Z))) {
                ChunkRef north = chunk.GetNorthNeighbor();
                chunk.Blocks.StitchBlockLight(north.Blocks, BlockCollectionEdge.NORTH);
                chunk.Blocks.StitchSkyLight(north.Blocks, BlockCollectionEdge.NORTH);
            }

            if (!dirty.ContainsKey(new ChunkKey(chunk.X + 1, chunk.Z))) {
                ChunkRef south = chunk.GetSouthNeighbor();
                chunk.Blocks.StitchBlockLight(south.Blocks, BlockCollectionEdge.SOUTH);
                chunk.Blocks.StitchSkyLight(south.Blocks, BlockCollectionEdge.SOUTH);
            }
        }
    }
    /// <summary> Gets the timestamp of the chunk from its underlying region file. </summary>
    /// <param name="cx">The global X-coordinate of a chunk.</param>
    /// <param name="cz">The global Z-coordinate of a chunk.</param>
    /// <returns>The timestamp of the chunk from its underlying region file.</returns>
    /// <remarks>The value returned may differ from any timestamp stored in the chunk data itself.</remarks>
    public int GetChunkTimestamp (int cx, int cz)
    {
        IRegion r = GetRegion(cx, cz);
        if (r == null)
            return 0;            
        return r.GetChunkTimestamp(cx & REGION_XMASK, cz & REGION_ZMASK);
    }
    /// <summary> Sets the timestamp of the chunk in its underlying region file. </summary>
    /// <param name="cx">The global X-coordinate of a chunk.</param>
    /// <param name="cz">The global Z-coordinate of a chunk.</param>
    /// <param name="timestamp">The new timestamp value.</param>
    /// <remarks>This function will only update the timestamp of the chunk slot in the underlying region file.  It will not update
    /// any timestamp information in the chunk data itself.</remarks>
    public void SetChunkTimestamp (int cx, int cz, int timestamp)
    {
        IRegion r = GetRegion(cx, cz);
        if (r == null) {
            return;
        }

        r.SetChunkTimestamp(cx & REGION_XMASK, cz & REGION_ZMASK, timestamp);
    }
    private ChunkRef GetChunkRefInRegion (IRegion r, int lcx, int lcz)
    {
        int cx = r.X * REGION_XLEN + lcx;
        int cz = r.Z * REGION_ZLEN + lcz;
        return GetChunkRef(cx, cz);
    }
    private IRegion GetRegion (int cx, int cz)
    {
        cx >>= REGION_XLOG;
        cz >>= REGION_ZLOG;
        return _regionMan.GetRegion(cx, cz);
    }
    /// <summary> Returns an enumerator that iterates through all chunks in all regions of the world. </summary>
    /// <returns>An enumerator for this manager.</returns>
    public IEnumerator<ChunkRef> GetEnumerator () => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator () => new Enumerator(this);
    private class Enumerator : IEnumerator<ChunkRef>
    {
        private RegionChunkManager _cm;
        private IEnumerator<IRegion> _enum;
        private IRegion _region;
        private ChunkRef _chunk;
        private int _x = 0;
        private int _z = -1;
        public Enumerator (RegionChunkManager cm)
        {
            _cm = cm;
            _enum = _cm.RegionManager.GetEnumerator();
            if (_enum.MoveNext())
                _region = _enum.Current;
        }
        public virtual bool MoveNext ()
        {
            if (_region == null)
                return false;

            if (_enum == null)
                return MoveNextInRegion();
            else {
                while (true) {
                    if (_x >= RegionChunkManager.REGION_XLEN) {
                        if (!_enum.MoveNext()) {
                            return false;
                        }
                        _x = 0;
                        _z = -1;
                        _region = _enum.Current;
                    }
                    if (MoveNextInRegion()) {
                        _chunk = _region.GetChunkRef(_x, _z);
                        return true;
                    }
                }
            }
        }
        protected bool MoveNextInRegion ()
        {
            for (; _x < REGION_XLEN; _x++) {
                for (_z++; _z < REGION_ZLEN; _z++) {
                    if (_region.ChunkExists(_x, _z)) {
                        goto Нахуй;
                    }
                }
                _z = -1;
            }
            Нахуй:
                return (_x < REGION_XLEN);
        }
        public void Reset ()
        {
            if (_enum != null) {
                _enum.Reset();
                _enum.MoveNext();
                _region = _enum.Current;
            }
            _x = 0;
            _z = -1;
        }
        void IDisposable.Dispose () { }
        object IEnumerator.Current { get => Current; }
        ChunkRef IEnumerator<ChunkRef>.Current { get => Current; }
        public ChunkRef Current
        {
            get
            {
                if (_x >= RegionChunkManager.REGION_XLEN)
                    throw new InvalidOperationException();
                return _chunk;
            }
        }
    }
}