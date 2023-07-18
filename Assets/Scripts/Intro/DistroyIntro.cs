using UnityEngine;

public class DistroyIntro : MonoBehaviour
{
    [SerializeField]
    private IntroController owner;

    private void OnCollisionEnter(Collision collision)
    {
        var sphere = collision.gameObject.GetComponent<HorseSphere>();
        if (!sphere) return;
        if (!sphere.horse.isPlayerRiding) return;

        owner.EndTutorial();
    }
}
