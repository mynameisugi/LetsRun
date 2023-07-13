using Unity.XR.CoreUtils;
using UnityEngine;

public class JockeyController : MonoBehaviour
{
    private Animator myAnim;

    [SerializeField]
    private SkinnedMeshRenderer myRenderer;
    [SerializeField]
    private MeshRenderer hatRenderer;

    [SerializeField]
    internal Transform[] hands = new Transform[2];

    private HorseController horse;

    private void Start()
    {
        myAnim = GetComponent<Animator>();
        horse = GetComponentInParent<HorseController>();
    }

    private void Update()
    {
        myAnim.speed = Mathf.Clamp(Mathf.Pow(horse.CurMode * 0.5f, 1.5f), 0.1f, 2.0f);
    }

    public void SetNumber(int number)
    {
        Material mat = Resources.Load("Materials/Jockey/Jockey" + number.ToString("00")) as Material;
        if (!mat) return;
        hatRenderer.material = mat;
        var mats = myRenderer.materials;
        mats[1] = mat;
        myRenderer.materials = mats;
    }

    public void SetPlayer()
    {
        gameObject.SetLayerRecursively(8);
    }

    public void PlayWhip() => myAnim.SetTrigger("Whip");
    public void PlayHalt() => myAnim.SetTrigger("Halt");

}
