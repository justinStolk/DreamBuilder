using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.Networking.Transport;
using UnityEngine;


namespace ChatClientExample
{
    public class RPCMessage : MessageHeader
    {
        static NetworkManager networkManager;
        public override NetworkMessageType Type => NetworkMessageType.RPC;
        public uint TargetID;
        public NetworkedBehaviour target;
        public string MethodName;
        public object[] data;

        public MethodInfo mInfo;
        public ParameterInfo[] parameters;

        public override void SerializeObject(ref DataStreamWriter writer)
        {
            base.SerializeObject(ref writer);
            writer.WriteUInt(TargetID);
            writer.WriteFixedString128(MethodName);

            mInfo = target.GetType().GetMethod(MethodName);
            if (mInfo == null)
            {
                throw new System.ArgumentException($"Object of type: {target.GetType()} does not contain method called {MethodName}!");
            }

            parameters = mInfo.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType == typeof(string))
                {
                    writer.WriteFixedString128((string)data[i]);
                }
                else if (parameters[i].ParameterType == typeof(float))
                {
                    writer.WriteFloat((float)data[i]);
                }
                else if (parameters[i].ParameterType == typeof(int))
                {
                    writer.WriteInt((int)data[i]);
                }
                else if (parameters[i].ParameterType == typeof(Vector3))
                {
                    Vector3 vec = (Vector3)data[i];
                    writer.WriteFloat(vec.x);
                    writer.WriteFloat(vec.y);
                    writer.WriteFloat(vec.z);
                }
                else if (parameters[i].ParameterType == typeof(Vector2Int))
                {
                    Vector2Int vec = (Vector2Int)data[i];
                    writer.WriteInt(vec.x);
                    writer.WriteInt(vec.y);
                }
                else if (parameters[i].ParameterType == typeof(Vector3Int))
                {
                    Vector3Int vec = (Vector3Int)data[i];
                    writer.WriteInt(vec.x);
                    writer.WriteInt(vec.y);
                    writer.WriteInt(vec.z);
                }
                else
                {
                    throw new System.ArgumentException($"Unhandled RPCType: { parameters[i].ParameterType.ToString() }");
                }
            }
        }

        public override void DeserializeObject(ref DataStreamReader reader)
        {
            base.DeserializeObject(ref reader);
            TargetID = reader.ReadUInt();
            //NetworkId = reader.ReadUInt();
            MethodName = reader.ReadFixedString128().ToString();
            //TODO: Find a better way to do this...
            if (networkManager == null) networkManager = Object.FindObjectOfType<NetworkManager>();
            // Get Object reference, and extract the MethodInfo from it's NetworkedBehaviour
            // -> this works because the object itself knows its type (e.g.PlayerBehaviour), even if we treat it as its base(NetworkedBehaviour)
            
            //GameObject obj;
            if (networkManager.GetReference(TargetID, out GameObject obj))
            {
                target = obj.GetComponent<NetworkedBehaviour>();
                mInfo = target.GetType().GetMethod(MethodName);
                if (mInfo == null)
                {
                    throw new System.ArgumentException($"Object of type { target.GetType() } does not contain method called { MethodName}");
                }
            }
            else
            {
                Debug.LogError($"Could not find object with id { TargetID } ");
            }
            // Extract Parameter info
            parameters = mInfo.GetParameters();
            // Prepare Data to store parameters into
            data = new object[parameters.Length];
            // Read each parameter in order (must be the same as we wrote it)
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (parameters[i].ParameterType == typeof(string))
                {
                    data[i] = reader.ReadFixedString128().ToString();
                }
                else if (parameters[i].ParameterType == typeof(float))
                {
                    data[i] = reader.ReadFloat();
                }
                else if (parameters[i].ParameterType == typeof(int))
                {
                    data[i] = reader.ReadInt();
                }
                else if (parameters[i].ParameterType == typeof(Vector3))
                {
                    data[i] = new Vector3(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
                }
                else if (parameters[i].ParameterType == typeof(Vector2Int))
                {
                    data[i] = new Vector2Int(reader.ReadInt(), reader.ReadInt());
                }
                else if (parameters[i].ParameterType == typeof(Vector3Int))
                {
                    data[i] = new Vector3Int(reader.ReadInt(), reader.ReadInt(), reader.ReadInt());
                }
                else
                {
                    throw new System.ArgumentException($"Unhandled RPCType: { parameters[i].ParameterType.ToString() }");
                }


            }
        }
    }

}
