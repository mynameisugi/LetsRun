using UnityEngine;

public class DisplayTutorial : MonoBehaviour
{
    [SerializeField]
    private string tutorialText = "";

    private GUIController gui = null;

    private void OnTriggerEnter(Collider other)
    {
        var sphere = other.GetComponent<HorseSphere>();
        if (!sphere || !sphere.horse.isPlayerRiding) return;
        gui = PlayerManager.Instance().GUI;
        gui.SetMessageBoxText(tutorialText);
    }

    private void OnTriggerExit(Collider other)
    {
        var sphere = other.GetComponent<HorseSphere>();
        if (!sphere || !sphere.horse.isPlayerRiding) return;
        if (gui) gui.SetMessageBoxText(string.Empty);
    }
}
