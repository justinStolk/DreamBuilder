using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;

namespace ChatClientExample
{
    public class Client : MonoBehaviour
    {
        public ChatCanvas chat;
        public string ClientName = "Justin";

        private NetworkDriver networkDriver;
        private NetworkConnection connection;
        private bool Done;

        static Dictionary<NetworkMessageType, ClientMessageHandler> clientHeaderHandlers = new() {
            { NetworkMessageType.RPC, HandleRPC },
            { NetworkMessageType.HANDSHAKE_RESPONSE, HandleHandshakeResponse },
            { NetworkMessageType.CHAT_MESSAGE, HandleChatMessage }
        };
        // Start is called before the first frame update
        void Start()
        {
            networkDriver = NetworkDriver.Create();
            connection = default(NetworkConnection);
            var endpoint = NetworkEndPoint.LoopbackIpv4;
            endpoint.Port = 1511;
            connection = networkDriver.Connect(endpoint);
        }

        // Update is called once per frame
        void Update()
        {
            networkDriver.ScheduleUpdate().Complete();
            if (!connection.IsCreated)
            {
                if (!Done)
                {
                    Debug.Log("Something went wrong while trying to connect!");
                    return;
                }
            }

            DataStreamReader stream;
            NetworkEvent.Type cmd;

            while((cmd = connection.PopEvent(networkDriver, out stream)) != NetworkEvent.Type.Empty)
            {

                if (cmd == NetworkEvent.Type.Connect)
                {
                    Debug.Log("We are now connected to the server");

                    networkDriver.BeginSend(connection, out var writer);

                    HandshakeMessage message = new HandshakeMessage();
                    message.ClientName = ClientName;
                    message.SerializeObject(ref writer);

                    networkDriver.EndSend(writer);
                }
                else if (cmd == NetworkEvent.Type.Data)
                {
                    //uint value = stream.ReadUInt();
                    //Debug.Log("Got the value = " + value + " back from the server");
                    //Done = true;
                    //connection.Disconnect(networkDriver);
                    //connection = default(NetworkConnection);

                    NetworkMessageType msgType = (NetworkMessageType)stream.ReadUShort();
                    MessageHeader header = (MessageHeader)System.Activator.CreateInstance(NetworkMessageInfo.TypeMap[msgType]);
                    header.DeserializeObject(ref stream);

                    if (clientHeaderHandlers.ContainsKey(msgType))
                    {
                        try
                        {
                            clientHeaderHandlers[msgType].Invoke(this, header);
                        }
                        catch
                        {
                            Debug.LogError("Badly formatted message received...");
                        }
                    }

                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client got disconnected from server");
                    connection = default(NetworkConnection);
                }
            }


        }
        public void SendMessageOnServer(string message)
        {
            networkDriver.BeginSend(connection, out var writer);
            ChatMessage chatMessage = new ChatMessage();
            chatMessage.Message = message;
            chatMessage.SerializeObject(ref writer);
            networkDriver.EndSend(writer);
        }
        static void HandleHandshakeResponse(object handler, MessageHeader header)
        {
            Client client = handler as Client;
            HandshakeResponseMessage response = header as HandshakeResponseMessage;

            client.chat.NewMessage(response.Response, ChatCanvas.joinColor);

        }
        static void HandleChatMessage(object handler, MessageHeader header)
        {
            Client client = handler as Client;
            ChatMessage chatMessage = header as ChatMessage;

            client.chat.NewMessage(chatMessage.Message, ChatCanvas.chatColor);
        }
        static void HandleRPC(object handler, MessageHeader header)
        {
            Client client = handler as Client;
            RPCMessage msg = header as RPCMessage;
            // try to call the function
            try
            {
                msg.mInfo.Invoke(msg.target, msg.data);
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e.StackTrace);
            }
        }
        private void OnDestroy()
        {
            networkDriver.Dispose();
        }


    }
}
