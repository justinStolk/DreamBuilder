using ChatClientExample;
using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class PlacementMessage : MessageHeader
{
    public override NetworkMessageType Type => NetworkMessageType.PLACE_ELEMENT;
    public uint TargetID;
    public Vector3 Location;
    public override void SerializeObject(ref DataStreamWriter writer)
    {
        base.SerializeObject(ref writer);
        writer.WriteUInt(TargetID);
        writer.WriteFloat(Location.x);
        writer.WriteFloat(Location.y);
        writer.WriteFloat(Location.z);

    }

    public override void DeserializeObject(ref DataStreamReader reader)
    {
        base.DeserializeObject(ref reader);
        TargetID = reader.ReadUInt();
        Location = new Vector3(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());
    }

}
