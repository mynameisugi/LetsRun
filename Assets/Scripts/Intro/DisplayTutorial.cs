using UnityEngine;

public class DisplayTutorial : MonoBehaviour
{
    [SerializeField]
    private string tutorialText = "";

    private GUIController gui = null;

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<PlayerManager>();
        if (!player) return;
        gui = player.GUI;
        gui.SetMessageBoxText(tutorialText);
    }
    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponentInParent<PlayerManager>();
        if (!player) return;
        if (gui) gui.SetMessageBoxText(string.Empty);
    }
}
