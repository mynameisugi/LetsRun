using UnityEngine;

public class RegisterTicketReceiver : MonoBehaviour
{
    [SerializeField]
    private RegisterTent owner;

    private void Start()
    {
        var renderer = GetComponent<MeshRenderer>();
        if (renderer) renderer.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        var ticket = other.GetComponentInParent<RegisterTicket>();
        if (ticket && ticket == owner.ticket) owner.ReceiveTicket();
    }
}
