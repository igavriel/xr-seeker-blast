using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;
using Meta.XR.MRUtilityKit;
using System.Collections;

public class RuntimeNavmeshBuilder : MonoBehaviour
{
    private NavMeshSurface navMeshSurface;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
        MRUK.Instance.RegisterSceneLoadedCallback(OnSceneLoaded);
    }

    void OnSceneLoaded()
    {
        StartCoroutine(BuildNavmeshRoutine());
    }

    public IEnumerator BuildNavmeshRoutine()
    {
        yield return new WaitForEndOfFrame();   
        navMeshSurface.BuildNavMesh();
    }
}
