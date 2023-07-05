using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterTicketReceiver : MonoBehaviour
{
    [SerializeField]
    private RegisterTent owner;

    private void OnTriggerEnter(Collider other)
    {
        var ticket = other.GetComponentInParent<RegisterTicket>();
        if (ticket && ticket == owner.ticket) owner.ReceiveTicket();
    }
}
