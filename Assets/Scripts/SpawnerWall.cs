using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;

public class SpawnerWall : MonoBehaviour
{
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private float wallInset = 0.01f;

    private bool spawned = false;
    private float _brickWidth = 1f;
    private float _brickHeight = 1f;
    private bool _brickSizeCached;

    void Start()
    {
        UnityEngine.Assertions.Assert.IsNotNull(brickPrefab, "SpawnerWall: brickPrefab is not assigned");
    }

    void Update()
    {
        if (!MRUK.Instance || !MRUK.Instance.IsInitialized)
            return;

        if (!spawned)
        {
            SpawnObject();
            spawned = true;
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
            SpawnOneBrickBottomLeft(wall);
    }

    void SpawnOneBrickBottomLeft(MRUKAnchor wallAnchor)
    {
        Transform wallTransform = wallAnchor.transform;

        // Wall size from MRUK (PlaneRect = local 2D bounds; Unity Rect: x,y = min corner, width/height = size)
        float wallWidth = 2f;
        float wallHeight = 2.5f;
        float wallMinX = -1f;
        float wallMinY = -1.25f;

        if (wallAnchor.PlaneRect.HasValue)
        {
            Rect r = wallAnchor.PlaneRect.Value;
            wallWidth = r.width;
            wallHeight = r.height;
            wallMinX = r.x;
            wallMinY = r.y;
        }
        else
        {
            Vector3 anchorSize = wallAnchor.GetAnchorSize();
            wallWidth = anchorSize.x;
            wallHeight = anchorSize.y;
            wallMinX = -wallWidth * 0.5f;
            wallMinY = -wallHeight * 0.5f;
        }

        // Brick size from prefab (original prefab size – the smaller one)
        if (!_brickSizeCached)
        {
            GetBrickSize(out _brickWidth, out _brickHeight);
            _brickSizeCached = true;
        }
        float brickWidth = _brickWidth;
        float brickHeight = _brickHeight;

        // One brick at bottom-left: center so that bottom-left of brick aligns with bottom-left of wall
        float localX = wallMinX + brickWidth * 0.5f;
        float localY = wallMinY + brickHeight * 0.5f;

        Vector3 localPos =
            wallTransform.right * localX +
            wallTransform.up * localY +
            wallTransform.forward * wallInset;

        Vector3 worldPos = wallTransform.TransformPoint(localPos);
        Quaternion worldRot = wallTransform.rotation;

        GameObject brick = Instantiate(brickPrefab, worldPos, worldRot, wallTransform);
        brick.name = "brick_bottomLeft";
    }

    void GetBrickSize(out float width, out float height)
    {
        width = 1f;
        height = 1f;

        GameObject temp = Instantiate(brickPrefab);
        Renderer r = temp.GetComponentInChildren<Renderer>();
        if (r != null)
        {
            Bounds b = r.bounds;
            width = b.size.x;
            height = b.size.y;
        }
        else
        {
            Collider c = temp.GetComponentInChildren<Collider>();
            if (c != null)
            {
                width = c.bounds.size.x;
                height = c.bounds.size.y;
            }
        }
        Destroy(temp);
    }
}
