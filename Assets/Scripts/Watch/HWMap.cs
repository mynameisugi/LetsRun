using UnityEngine;

public class HWMap : MonoBehaviour
{
    [SerializeField]
    private GameObject camera;


    private void OnEnable()
    {
        if (camera) camera.SetActive(true);
    }

    private void OnDisable()
    {
        if (camera) camera.SetActive(false);
    }
}
