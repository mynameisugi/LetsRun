using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static RaceManager;

public class RegisterTicket : MonoBehaviour
{
    [SerializeField]
    private Toggle[] toggleLength;
    [SerializeField]
    private Toggle toggleObstacle;

    public RaceType ParseTicket()
    {
        RaceType type;
        if (toggleLength[0].isOn) type = RaceType.Easy;
        else if (toggleLength[1].isOn) type = RaceType.Normal;
        else type = RaceType.Hard;

        return type;
    }

    public bool ObstacleTicket()
        => toggleObstacle.isOn;

}
