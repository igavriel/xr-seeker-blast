using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class SpawnerWall : MonoBehaviour
{
    [SerializeField] public GameObject[] housePrefabs;

    [Tooltip("Offset along wall normal: negative = house slightly behind wall plane (avoids z-fight).")]
    [SerializeField] public float wallInset = -0.01f;

    [Tooltip("Set to -1 if the house appears in front of the wall instead of replacing it (flips depth direction).")]
    [SerializeField] public float wallNormalDirection = 1f;

    [Tooltip("Depth of the house beyond the wall in world units (how far it extends outside the room).")]
    [SerializeField] public float houseDepthBeyondWall = 1.5f;
    [Tooltip("Scale factor for house height. House starts from room floor and height = prefab height × scaleHouseY.")]
    [SerializeField] public float scaleHouseY = 1f;

    [Tooltip("If true, the wall mesh is hidden so the house fully replaces it.")]
    [SerializeField] public bool hideWall = true;

    private bool _spawned;
    private Vector3 _houseSize;

    void Start()
    {
        UnityEngine.Assertions.Assert.IsTrue(housePrefabs.Length > 0, "SpawnerWall: housePrefabs is empty");
    }

    void Update()
    {
        if (!_spawned)
        {
            if (!MRUK.Instance || !MRUK.Instance.IsInitialized)
                return;

            SpawnObject();
            _spawned = true;
        }
    }

    public void SpawnObject()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        if (room == null)
        {
            Debug.LogWarning("SpawnerWall: No current room.");
            return;
        }

        List<MRUKAnchor> wallAnchors = new List<MRUKAnchor>();
        if (room.WallAnchors != null && room.WallAnchors.Count > 0)
            wallAnchors.AddRange(room.WallAnchors);
        else
        {
            foreach (var anchor in room.Anchors)
            {
                if (anchor.Label == MRUKAnchor.SceneLabels.WALL_FACE)
                    wallAnchors.Add(anchor);
            }
        }

        // Room floor level from floor anchor (so house sits on actual floor)
        float floorWorldY = GetRoomFloorY(room);

        foreach (var wall in wallAnchors)
            SpawnHouseOnWall(wall, floorWorldY);
    }

    static float GetRoomFloorY(MRUKRoom room)
    {
        if (room.FloorAnchors != null && room.FloorAnchors.Count > 0)
            return room.FloorAnchors[0].GetAnchorCenter().y;
        Bounds bounds = room.GetRoomBounds();
        if (bounds.size.y > 0.001f)
            return bounds.min.y;
        return 0f;
    }

    void SpawnHouseOnWall(MRUKAnchor wallAnchor, float floorWorldY)
    {
        Transform wallTransform = wallAnchor.transform;

        if (hideWall)
            SetWallVisible(wallAnchor, false);

        // Wall size from MRUK
        float wallWidth = 2f;

        if (wallAnchor.PlaneRect.HasValue)
        {
            Rect r = wallAnchor.PlaneRect.Value;
            wallWidth = r.width;
        }
        else
        {
            Vector3 anchorSize = wallAnchor.GetAnchorSize();
            wallWidth = anchorSize.x;
        }

        GameObject housePrefab = housePrefabs[Random.Range(0, housePrefabs.Length)];
        Vector3 houseSize = Vector3.zero;
        GetPrefabSize(housePrefab, out houseSize);

        float houseW = Mathf.Max(0.01f, houseSize.x);
        float houseH = Mathf.Max(0.01f, houseSize.y);
        float houseD = Mathf.Max(0.01f, houseSize.z);

        // Scale: width = wall; height = scaleHouseY; depth = houseDepthBeyondWall
        float scaleX = wallWidth / houseW;
        float scaleY = scaleHouseY;
        float scaleZ = houseDepthBeyondWall / houseD;
        Vector3 scale = new Vector3(scaleX, scaleY, scaleZ);

        // House height in world after scaling
        float houseHeightWorld = houseH * scaleHouseY;

        // Position: house front face on the wall plane (so house replaces wall), then apply inset.
        // Use wallNormalDirection: 1 = house extends backward from wall; -1 = house extends forward (into room).
        Vector3 wallCenter = wallTransform.position;
        float depthWorld = houseDepthBeyondWall;
        Vector3 depthOffset = wallNormalDirection * wallTransform.forward * (depthWorld * 0.5f);
        Vector3 houseCenter = wallCenter - depthOffset + wallTransform.forward * wallInset;
        houseCenter.y = floorWorldY;
        // Nudge house slightly in front of wall plane so it draws on top (avoids z-fight, ensures it hides wall)
        houseCenter += wallTransform.forward * 0.02f;

        Quaternion houseRot = wallTransform.rotation;
        GameObject house = Instantiate(housePrefab, houseCenter, houseRot);
        house.name = "house_wall";
        house.transform.localScale = scale;
    }

    static void SetWallVisible(MRUKAnchor wallAnchor, bool visible)
    {
        foreach (var r in wallAnchor.GetComponentsInChildren<Renderer>(true))
            r.enabled = visible;
    }

    static void GetPrefabSize(GameObject prefab, out Vector3 size)
    {
        size = Vector3.one;

        GameObject temp = Object.Instantiate(prefab);
        Renderer r = temp.GetComponentInChildren<Renderer>();
        if (r != null)
        {
            size = r.bounds.size;
        }
        else
        {
            Collider c = temp.GetComponentInChildren<Collider>();
            if (c != null)
                size = c.bounds.size;
        }
        Object.Destroy(temp);
    }
}
