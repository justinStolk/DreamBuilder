using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChatClientExample
{

    public class NetworkPlayer : NetworkedBehaviour
    {
        private bool isLocal;
    
        public void PlaceObject(NetworkSpawnObject networkSpawn, Vector2Int position, float rotation)
        {
            if (isLocal)
            {
                RPCMessage rpc = new RPCMessage();
                rpc.MethodName = "PlaceObject";
                rpc.data = new object[] { (int)networkSpawn, position, rotation };
                rpc.target = this;
            }
        }

    }
}
