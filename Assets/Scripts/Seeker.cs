using UnityEngine;
using UnityEngine.AI;

public class Seeker : MonoBehaviour
{
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] public float speed = 3.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = Camera.main.transform.position;

        agent.SetDestination(targetPosition);
        agent.speed = speed;
    }
}
