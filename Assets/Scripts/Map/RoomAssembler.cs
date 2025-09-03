using UnityEngine;
using System.Collections.Generic;

public class RoomAssembler : MonoBehaviour
{
    [Header("Chunk libraries (assign prefabs)")]
    public List<RoomChunk> startChunks = new();
    public List<RoomChunk> middleChunks = new();
    public List<RoomChunk> endChunks = new();
    public GameObject beaconPrefab; 
    bool _isGenerating;

    [Header("Length & seed")]
    public int baseChunkCount = 5;          // total chunks in room 1
    public int chunksPerRoomIncrement = 1;  // rooms get longer
    public int currentRoomIndex = 1;        // start at room 1
    public int seed = 12345;

    [Header("Parents & refs")]
    public Transform roomRoot;              // where chunks are spawned
    public Transform player;                //reposition player at start
    public Vector3 playerSpawnOffset = new(0, 0.6f, 0);

    [Header("Safe pad at spawn")]
    public GameObject safePadPrefab;       
    public float safePadRadius = 2f;       
    public float safePadFollowSeconds = 0.25f; 
    public LayerMask groundMask;   
    

    System.Random rng;

    void Reset()
    {
        if (!roomRoot) roomRoot = this.transform;
    }

    void Start()
    {
        GenerateRoom();
    }

    [ContextMenu("Generate Room")]
    public void GenerateRoom()
    {
        if (!roomRoot) roomRoot = this.transform;
        ClearRoom();
        rng = new System.Random(seed);

        int total = Mathf.Max(3, baseChunkCount + (currentRoomIndex - 1) * chunksPerRoomIncrement);

        // 1) Start
        RoomChunk start = Instantiate(WeightedPick(startChunks), roomRoot);
        AlignFirst(start);

        Transform lastExit = start.exitAnchor;


        if (player)
        {
            player.position = start.entryAnchor.position + playerSpawnOffset;
        }

        // 2) Middles
        for (int i = 0; i < total - 2; i++)
        {
            RoomChunk mid = Instantiate(WeightedPick(middleChunks), roomRoot);
            AlignToPrevious(mid, lastExit);
            lastExit = mid.exitAnchor;
        }

        // 3) End
        RoomChunk end = Instantiate(WeightedPick(endChunks), roomRoot);
        AlignToPrevious(end, lastExit);

        if (beaconPrefab)
        {
            Vector3 pos = end.beaconPoint ? end.beaconPoint.position : end.exitAnchor.position;
            Instantiate(beaconPrefab, pos, Quaternion.identity, end.transform);
        }

        var pop = GetComponent<RoomPopulator>();
        if (pop) pop.Populate();


        var grid = GroundPaintGrid.Instance;
        if (grid)
        {
            grid.roomRoot = roomRoot;
            grid.RebuildBounds();
            // Pre-mark a safe area at spawn
            Vector3 spawnPos = player.position;
            grid.MarkCircle(spawnPos, 2.0f, float.PositiveInfinity); // permanent safe start
        }
        
        var cam = Camera.main?.GetComponent<IsoCamera>();
        LayerMask groundMask = default;
        if (cam) cam.SetClampFromRoot(roomRoot, groundMask, 6f);
    }

    void SpawnSafePadAtPlayer()
{
    if (!safePadPrefab || !player) return;

    // Base position = player
    Vector3 pos = player.position;

    // Snap to ground so the decal sits flush
    if (groundMask.value != 0 &&
        Physics.Raycast(pos + Vector3.up * 3f, Vector3.down, out var hit, 10f, groundMask))
    {
        pos.y = hit.point.y + 0.02f;
    }

    var padGO = Instantiate(safePadPrefab, pos, Quaternion.identity, roomRoot); // parented to room
    var pad = padGO.GetComponent<SafePad>();
    if (pad)
    {
        pad.radius = safePadRadius;
        pad.followSeconds = safePadFollowSeconds; // 0 = don’t follow
        pad.followTarget = player;               
        pad.groundMask = groundMask;
    }
}

    void ClearRoom()
    {
        var trash = new List<GameObject>();
        foreach (Transform child in roomRoot) trash.Add(child.gameObject);
        foreach (var go in trash)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(go);
            else Destroy(go);
#else
            Destroy(go);
#endif
        }
    }

    RoomChunk WeightedPick(List<RoomChunk> list)
    {
        if (list == null || list.Count == 0) return null;
        int sum = 0; foreach (var c in list) sum += Mathf.Max(1, c.weight);
        int pick = rng.Next(sum);
        int run = 0;
        foreach (var c in list)
        {
            run += Mathf.Max(1, c.weight);
            if (pick < run) return c;
        }
        return list[list.Count - 1];
    }

    void AlignFirst(RoomChunk chunk)
    {
        Vector3 delta = roomRoot.position - chunk.entryAnchor.position;
        chunk.transform.position += delta;
    }

    void AlignToPrevious(RoomChunk chunk, Transform prevExit)
    {
        Vector3 delta = prevExit.position - chunk.entryAnchor.position;
        chunk.transform.position += delta;
    }

    public void NextRoom()
    {
        if (_isGenerating) return;
        StartCoroutine(NextRoomCo());
    }

    System.Collections.IEnumerator NextRoomCo()
    {
        _isGenerating = true;

        currentRoomIndex++;
        seed += 9973;

        // Let current frame finishs
        yield return null;

        GenerateRoom();

        _isGenerating = false;
    }
}
