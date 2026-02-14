using Meta.XR.MRUtilityKit;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] public MRUKAnchor.SceneLabels spawnLabels = MRUKAnchor.SceneLabels.WALL_FACE;
    [SerializeField] public GameObject[] objectToSpawn;
    [SerializeField] public float minSpawnInterval = 2f;
    [SerializeField] public float maxSpawnInterval = 5f;
    [SerializeField] public float distanceToEdge = 0.3f;
    [SerializeField] public float normalOffset = 0.1f;
    [SerializeField] public int maxSpawnedObjects = 10;
    [SerializeField] public int spawnTryCount = 1000;

    private float spawnTimer = 0f;
    private int spawnedObjects = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UnityEngine.Assertions.Assert.IsNotNull(objectToSpawn[0], "Spawner - Object to spawn is not assigned");
    }

    // Update is called once per frame
    void Update()
    {
        if ( !MRUK.Instance || !MRUK.Instance.IsInitialized)
        {
            return;
        }
        if (spawnedObjects >= maxSpawnedObjects)
        {
            return;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= Random.Range(minSpawnInterval, maxSpawnInterval))
        {
            spawnTimer = 0f;
            SpawnObject();
            spawnedObjects++;
        }
    }

    public void SpawnObject()
    {
        MRUKRoom room = MRUK.Instance.GetCurrentRoom();
        for (int i = 0; i < spawnTryCount; i++)
        {
            bool success = room.GenerateRandomPositionOnSurface(
                MRUK.SurfaceType.VERTICAL,
                distanceToEdge,
                LabelFilter.Included(spawnLabels),
                out Vector3 position,
                out Vector3 normal);

            if (success)
            {
                Vector3 randomPosition = position + normal * normalOffset;
                randomPosition.y = 0f;  // force at the same y level

                Quaternion rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                Debug.Log("Spawner - Spawning object at " + randomPosition + " with rotation " + rotation);
                Instantiate(objectToSpawn[Random.Range(0, objectToSpawn.Length)], randomPosition, rotation);
                return;
            }
        }
        Debug.LogWarning("Spawner - Failed to spawn object");
    }
}
