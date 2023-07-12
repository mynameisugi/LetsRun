using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RegisterTent : MonoBehaviour
{
    [SerializeField]
    private GameObject ticketPrefab;

    [SerializeField]
    private TextReader NPC;

    internal RegisterTicket ticket;


    public void OnNewTicketGrabbed(XRSimpleInteractable pile)
    {
        if (ticket) Destroy(ticket.gameObject);
        else NPC.PlayConversation("RegisterTentPickup");
        var newObj = Instantiate(ticketPrefab);
        newObj.transform.SetParent(transform);
        ticket = newObj.GetComponent<RegisterTicket>();
        var ticketGrab = ticket.GetComponent<XRGrabInteractable>();
        pile.interactionManager.SelectEnter(pile.firstInteractorSelecting, ticketGrab);
        //ticketGrab.interactorsSelecting.Add(pile.firstInteractorSelecting);
        //ticket.transform.SetPositionAndRotation(fakeTicket.position, fakeTicket.rotation);
    }

    public void ReceiveTicket()
    {
        var ticketType = ticket.ParseTicket();
        GameManager.Instance().Race.RegisterPlayer(ticketType, ticket.ObstacleTicket());
        Destroy(ticket.gameObject); ticket = null;

        string dialogue = ticketType switch
        {
            RaceManager.RaceType.Easy => "RegisterTentReceiveEasy",
            RaceManager.RaceType.Normal => "RegisterTentReceiveNormal",
            _ => "RegisterTentReceiveHard"
        };
        NPC.PlayConversation(dialogue);
    }
}
