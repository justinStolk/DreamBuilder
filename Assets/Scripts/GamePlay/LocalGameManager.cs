using ChatClientExample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGameManager : MonoBehaviour
{
    [SerializeField] private LayerMask floorMask;
    [SerializeField] private Client client;

    private LocalCityComponent activeComponent;
    private bool isMyTurn = true;
    // Update is called once per frame
    void Update()
    {
        if (activeComponent == null || !isMyTurn) return;

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Should rotate on server");
            //activeComponent.Rotate();
            RPCMessage msg = new RPCMessage();
            msg.target = activeComponent;
            msg.TargetID = activeComponent.NetworkID;
            msg.MethodName = "Rotate";
            client.SendRPCToServer(msg);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Should be placed on server");
            //    activeComponent.PlaceComponent();
            RPCMessage msg = new RPCMessage();
            msg.target = activeComponent;
            msg.TargetID = activeComponent.NetworkID;
            msg.MethodName = "PlaceComponent";
            client.SendRPCToServer(msg);
        }
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mouseRay, out RaycastHit hit, floorMask))
        {
            Debug.Log("Should move the object on the server");
            Vector3Int targetPosition = new(Mathf.RoundToInt(hit.transform.position.x), 0, Mathf.RoundToInt(hit.transform.position.z));
            //activeComponent.MoveToPosition(targetPosition);
            RPCMessage msg = new RPCMessage();
            msg.TargetID = activeComponent.NetworkID;
            msg.target = activeComponent;
            msg.MethodName = "MoveToPosition";
            msg.data = new object[1] { targetPosition };
            client.SendRPCToServer(msg);
        }
    }
    public void ReceiveCityComponent(NetworkCityComponent component)
    {
        activeComponent = component as LocalCityComponent;
        Debug.Log(activeComponent);
    }
    public void SetTurn(bool isMyTurn) => this.isMyTurn = isMyTurn;
}