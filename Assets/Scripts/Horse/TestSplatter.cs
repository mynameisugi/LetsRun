using UnityEngine;

public class TestSplatter : MonoBehaviour
{
    [SerializeField]
    private int count = 50;

    [SerializeField]
    private float radius = 50f;

    private void Start()
    {
        for (int i = 0; i < count; ++i)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localScale = new Vector3(Random.Range(1f, 5f), Random.Range(1f, 4f), Random.Range(1f, 5f));
            cube.transform.SetLocalPositionAndRotation(new Vector3(Random.Range(-radius, radius), 0f, Random.Range(-radius, radius)) + transform.position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
            cube.transform.SetParent(transform);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(radius * 2f, 10f, radius * 2f));
    }
}
