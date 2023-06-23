using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using Unity.Collections;


namespace ChatClientExample
{
    public delegate void NetworkMessageHandler(object handler, NetworkConnection con, DataStreamReader stream);
    public delegate void ServerMessageHandler(object handler, NetworkConnection con, MessageHeader header);
    public delegate void ClientMessageHandler(object handler, MessageHeader header);
    public enum NetworkMessageType
    {
        HANDSHAKE,
        HANDSHAKE_RESPONSE,
        CHAT_MESSAGE,
        CHAT_QUIT,
        CREATE_ELEMENT,
        MOVE_ELEMENT,
        ROTATE_ELEMENT,
        REMOVE_ELEMENT,
        PING,
        PONG,
        RPC
    }

    public static class NetworkMessageInfo
    {
        public static Dictionary<NetworkMessageType, System.Type> TypeMap = new Dictionary<NetworkMessageType, System.Type> {
            { NetworkMessageType.HANDSHAKE,                 typeof(HandshakeMessage) },
            { NetworkMessageType.HANDSHAKE_RESPONSE,        typeof(HandshakeResponseMessage) },
            { NetworkMessageType.CHAT_MESSAGE,              typeof(ChatMessage) },
            //{ NetworkMessageType.CHAT_QUIT,                 typeof(ChatQuitMessage) },
            //{ NetworkMessageType.NETWORK_SPAWN,             typeof(SpawnMessage) },
            //{ NetworkMessageType.NETWORK_DESTROY,           typeof(DestroyMessage) },
            //{ NetworkMessageType.NETWORK_UPDATE_POSITION,   typeof(UpdatePositionMessage) },
            //{ NetworkMessageType.INPUT_UPDATE,              typeof(InputUpdateMessage) },
            //{ NetworkMessageType.PING,                      typeof(PingMessage) },
            //{ NetworkMessageType.PONG,                      typeof(PongMessage) },
            { NetworkMessageType.RPC,                       typeof(RPCMessage) } 
        };


    } 


    public class Server : MonoBehaviour
    {
        //static Dictionary<NetworkMessageType, NetworkMessageHandler> networkMessageHandlers = new Dictionary<NetworkMessageType, NetworkMessageHandler>
        //{
        //    { NetworkMessageType.HANDSHAKE, HandleClientHandshake },
        //    { NetworkMessageType.CHAT_MESSAGE, HandleClientMessage },
        //    { NetworkMessageType.CHAT_QUIT, HandleClientExit },
        //    { NetworkMessageType.RPC, HandleRPC }
        //    { NetworkMessageType.CREATE_ELEMENT, HandleObjectSpawn },
        //    { NetworkMessageType.MOVE_ELEMENT, HandleObjectPlacement },
        //    { NetworkMessageType.ROTATE_ELEMENT, HandleObjectRotation },
        //    { NetworkMessageType.REMOVE_ELEMENT, HandleObjectRemoval }
        //};

        static Dictionary<NetworkMessageType, ServerMessageHandler> networkHeaderHandlers = new Dictionary<NetworkMessageType, ServerMessageHandler>{ 
            { NetworkMessageType.RPC, HandleRPC },
            { NetworkMessageType.HANDSHAKE, HandleClientHandshake },
            { NetworkMessageType.CHAT_MESSAGE, HandleClientMessage }
        };

        public NetworkDriver m_Driver;
        public NetworkPipeline m_Pipeline;
        private NativeList<NetworkConnection> m_Connections;
        private Dictionary<NetworkConnection, string> nameList = new Dictionary<NetworkConnection, string>();
        public ChatCanvas chat;

        void Start()
        {
            // Create Driver
            m_Driver = NetworkDriver.Create(new ReliableUtility.Parameters { WindowSize = 32 });
            m_Pipeline = m_Driver.CreatePipeline(typeof(ReliableSequencedPipelineStage));

            // Open listener on server port
            NetworkEndPoint endpoint = NetworkEndPoint.AnyIpv4;
            endpoint.Port = 1511;
            if (m_Driver.Bind(endpoint) != 0)
                Debug.Log("Failed to bind to port 1511");
            else
                m_Driver.Listen();
            m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);
        }
        // Write this immediately after creating the above Start calls, so you don't forget
        //  Or else you well get lingering thread sockets, and will have trouble starting new ones!
        void OnDestroy()
        {
            m_Driver.Dispose();
            m_Connections.Dispose();
        }
        void Update()
        {
            // This is a jobified system, so we need to tell it to handle all its outstanding tasks first
            m_Driver.ScheduleUpdate().Complete();
            // Clean up connections, remove stale ones
            for (int i = 0; i < m_Connections.Length; i++)
            {
                if (!m_Connections[i].IsCreated)
                {
                    if (nameList.ContainsKey(m_Connections[i]))
                    {
                        chat.NewMessage($"{ nameList[m_Connections[i]]} has disconnected.", ChatCanvas.leaveColor);
                        nameList.Remove(m_Connections[i]);
                    }
                    m_Connections.RemoveAtSwapBack(i);
                    // This little trick means we can alter the contents of the list without breaking / skipping instances
                    --i;
                }
            }
            // Accept new connections
            NetworkConnection c;
            while ((c = m_Driver.Accept()) != default(NetworkConnection))
            {
                m_Connections.Add(c);
                Debug.Log("Accepted a connection");
            }
            DataStreamReader stream;
            for (int i = 0; i < m_Connections.Length; i++)
            {
                if (!m_Connections[i].IsCreated)
                    continue;
                // Loop through available events
                NetworkEvent.Type cmd;
                while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
                {
                    if (cmd == NetworkEvent.Type.Data)
                    {
                        NetworkMessageType msgType = (NetworkMessageType)stream.ReadUShort();
                        MessageHeader header = (MessageHeader)System.Activator.CreateInstance(NetworkMessageInfo.TypeMap[msgType]);
                        header.DeserializeObject(ref stream);

                        // First UInt is always message type (this is our own first design choice)
                        if (networkHeaderHandlers.ContainsKey(msgType))
                        {
                            try
                            {
                                networkHeaderHandlers[msgType].Invoke(this, m_Connections[i], header);
                            }
                            catch
                            {
                                Debug.LogError("Badly formatted message received...");
                            }
                        }
                        //if (networkMessageHandlers.ContainsKey(msgType))
                        //{
                        //    try
                        //    {
                        //        networkMessageHandlers[msgType].Invoke(this, m_Connections[i], stream);
                        //    }
                        //    catch
                        //    {
                        //        Debug.LogError("Badly formatted message received...");
                        //    }
                        //}
                        else
                        {
                            Debug.LogWarning($"Unsupported message type received: { msgType} ", this);
                        }
                    }
                }
            }
        }
        // Static handler functions
        //  - Client handshake                  (DONE)
        //  - Client chat message               (DONE)
        //  - Client chat exit                  (DONE)
        static void HandleClientHandshake(object handler, NetworkConnection connection, MessageHeader header)
        {
            Server server = handler as Server;
            HandshakeMessage handshake = header as HandshakeMessage;

            HandshakeResponseMessage response = new HandshakeResponseMessage();
            response.Response = $"Welcome, {handshake.ClientName}!";
            server.chat.NewMessage(response.Response, ChatCanvas.joinColor);

            server.m_Driver.BeginSend(connection, out var writer);
            response.SerializeObject(ref writer);
            server.m_Driver.EndSend(writer);

        }
        static void HandleClientMessage(object handler, NetworkConnection connection, MessageHeader header)
        {
            Server server = handler as Server;
            ChatMessage chatMessage = header as ChatMessage;
            server.chat.NewMessage(chatMessage.Message, ChatCanvas.chatColor);

            foreach (NetworkConnection c in server.m_Connections)
            {
                server.m_Driver.BeginSend(c, out var writer);
                chatMessage.SerializeObject(ref writer);
                server.m_Driver.EndSend(writer);
            }
        }
        static void HandleClientHandshake(object handler, NetworkConnection connection, DataStreamReader stream)
        {
            // Pop name
            FixedString128Bytes str = stream.ReadFixedString128();

            Server serv = handler as Server;

            // Add to list
            serv.nameList.Add(connection, str.ToString());
            serv.chat.NewMessage($"{str.ToString()} has joined the chat.", ChatCanvas.joinColor);

            // Send message back
            DataStreamWriter writer;
            int result = serv.m_Driver.BeginSend(NetworkPipeline.Null, connection, out writer);

            // non-0 is an error code
            if (result == 0)
            {
                writer.WriteUInt((uint)NetworkMessageType.HANDSHAKE_RESPONSE);
                writer.WriteFixedString128(new FixedString128Bytes($"Welcome { str.ToString() }!"));

                serv.m_Driver.EndSend(writer);
            }
            else
            {
                Debug.LogError($"Could not write message to driver: {result}", serv);
            }
        }

        static void HandleClientMessage(object handler, NetworkConnection connection, DataStreamReader stream)
        {
            // Pop message
            FixedString128Bytes str = stream.ReadFixedString128();

            Server serv = handler as Server;
            if (serv.nameList.ContainsKey(connection))
            {
                serv.chat.NewMessage($"{serv.nameList[connection]}: { str.ToString()} ", ChatCanvas.chatColor);
            }
            else
            {
                Debug.LogError($"Received message from unlisted connection: { str } ");
            }
        }
        static void HandleClientExit(object handler, NetworkConnection connection, DataStreamReader stream)
        {
            Server serv = handler as Server;
            if (serv.nameList.ContainsKey(connection))
            {
                serv.chat.NewMessage($"{serv.nameList[connection]} has left the chat.", ChatCanvas.leaveColor);

                connection.Disconnect(serv.m_Driver);
            }
            else
            {
                Debug.LogError("Received exit from unlisted connection");
            }
        }

        static void HandleRPC(object handler, NetworkConnection connection, MessageHeader header)
        {
            Server serv = handler as Server;
            RPCMessage msg = header as RPCMessage;
            
            foreach(NetworkConnection c in serv.m_Connections)
            {
                serv.m_Driver.BeginSend(c, out var writer);
                msg.SerializeObject(ref writer);
                serv.m_Driver.EndSend(writer);
                //// try to call the function
                //try
                //{
                //    msg.mInfo.Invoke(msg.target, msg.data);
                //}
                //catch (System.Exception e)
                //{
                //    Debug.Log(e.Message);
                //    Debug.Log(e.StackTrace);
                //}

            }
        }


       }

}