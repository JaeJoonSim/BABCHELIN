using NavMeshPlus.Components;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshRuntimeBake : MonoBehaviour
{
    public NavMeshSurface Surface2D;

    private void Start()
    {
        Surface2D = GetComponent<NavMeshSurface>();
    }

    // Update is called once per frame
    void Update()
    {
        Surface2D.UpdateNavMesh(Surface2D.navMeshData);
    }
}
