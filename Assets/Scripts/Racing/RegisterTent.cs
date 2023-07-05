using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RegisterTent : MonoBehaviour
{
    [SerializeField]
    private GameObject ticketPrefab;

    private RegisterTicket ticket;


    public void OnNewTicketGrabbed(XRSimpleInteractable pile)
    {
        if (ticket) Destroy(ticket.gameObject);
        var newObj = Instantiate(ticketPrefab, transform);
        ticket = newObj.GetComponent<RegisterTicket>();
        var ticketGrab = ticket.GetComponent<XRGrabInteractable>();
        pile.interactionManager.SelectEnter(pile.firstInteractorSelecting, ticketGrab);
        //ticketGrab.interactorsSelecting.Add(pile.firstInteractorSelecting);
        //ticket.transform.SetPositionAndRotation(fakeTicket.position, fakeTicket.rotation);
    }
}
