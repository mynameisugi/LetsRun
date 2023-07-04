using UnityEngine;

public class HWMap : MonoBehaviour
{


    private void OnEnable()
    {
        var camera = PlayerManager.Instance().mapCamera;
        if (camera) camera.SetActive(true);
    }

    private void OnDisable()
    {
        var camera = PlayerManager.Instance().mapCamera;
        if (camera) camera.SetActive(false);
    }
}
