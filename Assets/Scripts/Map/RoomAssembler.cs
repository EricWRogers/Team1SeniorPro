using UnityEngine;
using System.Collections.Generic;

public class RoomAssembler : MonoBehaviour
{
    [Header("Chunk libraries (assign prefabs)")]
    public List<RoomChunk> startChunks = new();
    public List<RoomChunk> middleChunks = new();
    public List<RoomChunk> endChunks = new();
    public GameObject beaconPrefab; 

    [Header("Length & seed")]
    public int baseChunkCount = 5;          // total chunks in room 1
    public int chunksPerRoomIncrement = 1;  // rooms get longer
    public int currentRoomIndex = 1;        // start at room 1
    public int seed = 12345;

    [Header("Parents & refs")]
    public Transform roomRoot;              // where chunks are spawned
    public Transform player;                //reposition player at start
    public Vector3 playerSpawnOffset = new(0, 0.6f, 0);
    

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
            var beacon = Instantiate(beaconPrefab, pos, Quaternion.identity, end.transform);
            var sb = beacon.GetComponent<SimpleBeacon>();
            if (sb) sb.assembler = this; // so it can advance to the next room
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
        currentRoomIndex++;
        seed += 9973; // change layout a bit each room
        GenerateRoom();
    }
}
