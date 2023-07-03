using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class HandWatchController : MonoBehaviour
{
    //[Header("Canvases")]

    [SerializeField]
    private GameObject[] canvases = new GameObject[5];

    private void Start()
    {
        RequestModeSwitch(Mode.Main);
    }

    #region Mode

    public enum Mode : int
    {
        Main = -1,
        Clock = 0,
        Horse = 1,
        Inventory = 2,
        Setting = 3,
        Map = 4,
        Tutorial = 5
    }

    public Mode CurMode { get; private set; } = Mode.Main;

    public void RequestModeSwitch(Mode mode)
    {
        if (mode != Mode.Main)
        {
            //if (CurMode != Mode.Main) return; // Switching to other mode directly
            CurMode = mode;
        }
        else CurMode = mode;
        ToggleCanvases(mode);

        void ToggleCanvases(Mode newMode)
        {
            foreach (var c in canvases) if (c) c.SetActive(false);
            if (newMode == Mode.Main) return;
            canvases[(int)newMode].SetActive(true);
        }
    }

    #endregion Mode

    private void Update()
    {
        

    }

}
