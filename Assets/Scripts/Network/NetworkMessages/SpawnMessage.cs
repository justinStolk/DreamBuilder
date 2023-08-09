using ChatClientExample;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class SpawnMessage : MessageHeader
{
    public override NetworkMessageType Type => NetworkMessageType.NETWORK_SPAWN;
    public NetworkSpawnObject SpawnedObject;

    public override void SerializeObject(ref DataStreamWriter writer)
    {
        base.SerializeObject(ref writer);
        writer.WriteUInt((uint)SpawnedObject);
    }
    public override void DeserializeObject(ref DataStreamReader reader)
    {
        base.DeserializeObject(ref reader);
        SpawnedObject = (NetworkSpawnObject)reader.ReadUInt();
    }

}
