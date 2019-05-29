[System.Serializable]
class NetClientSpawned : NetMessage
{
    public PlayerDTO Player { set; get; }

    public NetClientSpawned()
    {
        Code = NetCodes.ClientSpawned;
    }
}
