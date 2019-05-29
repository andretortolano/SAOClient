[System.Serializable]
public abstract class NetMessage
{
    public byte Code { set; get; }

    public NetMessage()
    {
        Code = NetCodes.None;
    }
}
