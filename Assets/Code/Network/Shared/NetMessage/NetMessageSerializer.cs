using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class NetMessageSerializer
{
    public static byte[] Serialize(NetMessage netMessage, int size)
    {
        // This is where we hold our data
        byte[] buffer = new byte[size];

        // This is where you would crush your data into a byte[]
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        formatter.Serialize(ms, netMessage);

        return buffer;
    }

    public static NetMessage Deserialize(byte[] buffer)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream ms = new MemoryStream(buffer);
        return (NetMessage)formatter.Deserialize(ms);
    }
}
