using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RaceManager;

public class RegisterTicket : MonoBehaviour
{
    public RaceType ParseTicket()
    {
        // TODO: 티켓의 설정에 따라 타입 결정
        return RaceType.Easy;
    }

}
