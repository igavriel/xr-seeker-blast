using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class SpawnerWall : MonoBehaviour
{
    [SerializeField] public GameObject housePrefab;

    [Tooltip("Small offset so the house front face sits flush on the wall (negative = into room).")]
    [SerializeField] public float wallInset = 0f;

    [Tooltip("Depth of the house beyond the wall in world units (how far it extends outside the room).")]
    [SerializeField] public float houseDepthBeyondWall = 1.5f;
    [Tooltip("Scale factor for house height. House starts from room floor and height = prefab height × scaleHouseY.")]
    [SerializeField] public float scaleHouseY = 1f;

    private bool _spawned;
    private Vector3 _houseSize;
    private bool _houseSizeCached;

    void Start()
    {
        UnityEngine.Assertions.Assert.IsNotNull(housePrefab, "SpawnerWall: housePrefab is not assigned");
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

        foreach (var wall in wallAnchors)
            SpawnHouseOnWall(wall);
    }

    void SpawnHouseOnWall(MRUKAnchor wallAnchor)
    {
        Transform wallTransform = wallAnchor.transform;

        // Wall size and bottom edge from MRUK (floor level = wall bottom in local Y)
        float wallWidth = 2f;
        float wallMinY;

        if (wallAnchor.PlaneRect.HasValue)
        {
            Rect r = wallAnchor.PlaneRect.Value;
            wallWidth = r.width;
            wallMinY = r.y; // Rect.y = bottom edge
        }
        else
        {
            Vector3 anchorSize = wallAnchor.GetAnchorSize();
            wallWidth = anchorSize.x;
            wallMinY = -anchorSize.y * 0.5f;
        }

        // Room floor level in world Y (wall bottom)
        Vector3 wallBottomWorld = wallTransform.TransformPoint(0f, wallMinY, 0f);
        float floorWorldY = wallBottomWorld.y;

        // House prefab size
        if (!_houseSizeCached)
        {
            GetPrefabSize(housePrefab, out _houseSize);
            _houseSizeCached = true;
        }

        float houseW = Mathf.Max(0.01f, _houseSize.x);
        float houseH = Mathf.Max(0.01f, _houseSize.y);
        float houseD = Mathf.Max(0.01f, _houseSize.z);

        // Scale: width = wall; height = scaleHouseY; depth = houseDepthBeyondWall
        float scaleX = wallWidth / houseW;
        float scaleY = scaleHouseY;
        float scaleZ = houseDepthBeyondWall / houseD;
        Vector3 scale = new Vector3(scaleX, scaleY, scaleZ);

        // House height in world after scaling
        float houseHeightWorld = houseH * scaleHouseY;

        // Position: house bottom on room floor; center = floor + half height
        Vector3 wallCenter = wallTransform.position;
        Vector3 outward = -wallTransform.forward;
        float depthWorld = houseDepthBeyondWall;

        Vector3 houseCenter = wallCenter
            + outward * (depthWorld * 0.5f)
            + wallTransform.forward * wallInset;
        houseCenter.y = floorWorldY + houseHeightWorld * 0.5f;

        Quaternion houseRot = wallTransform.rotation;
        GameObject house = Instantiate(housePrefab, houseCenter, houseRot);
        house.name = "house_wall_" + wallAnchor.name;
        house.transform.localScale = scale;
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
