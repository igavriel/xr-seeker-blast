using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] public GameObject objectToSpawn;
    [SerializeField] public float minSpawnInterval = 1f;
    [SerializeField] public float maxSpawnInterval = 3f;
    [SerializeField] public float spawnTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Spawner - Start");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
