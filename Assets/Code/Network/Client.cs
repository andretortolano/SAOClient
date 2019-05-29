using UnityEngine;
using UnityEngine.Networking;

public class Client : MonoBehaviour
{
    private const int MAX_USER = 100;
    private const int PORT = 62000;
    private const int WEB_PORT = 62001;
    private const int BYTE_SIZE = 1024;
    private const string SERVER_IP = "127.0.0.1";

    private byte reliableChannel;
    private byte unreliableChannel;

    private int connectionId;
    private int hostId;
    private byte error;

    private bool isStarted = false;

    #region MonoBehavior
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }
    private void Update()
    {
        UpdateMessagePump();
    }
    #endregion

    public void Init()
    {
        NetworkTransport.Init();

        ConnectionConfig connConfig = new ConnectionConfig();
        reliableChannel = connConfig.AddChannel(QosType.Reliable);
        unreliableChannel = connConfig.AddChannel(QosType.Unreliable);

        HostTopology topo = new HostTopology(connConfig, MAX_USER);

        // Server only code
        hostId = NetworkTransport.AddHost(topo, 0);

#if UNITY_WEBGL && !UNITY_EDITOR
        // Web Client
        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, WEB_PORT, 0, out error);   
        Debug.Log("Connecting from web");
#else
        // Standalone Client
        connectionId = NetworkTransport.Connect(hostId, SERVER_IP, PORT, 0, out error);
        Debug.Log("Connecting from standalone");
#endif

        Debug.Log(string.Format("Attempting to connect on {0}...", SERVER_IP));
        isStarted = true;
    }
    public void Shutdown()
    {
        isStarted = false;
        NetworkTransport.Shutdown();
    }

    public void UpdateMessagePump()
    {
        if(!isStarted)
            return;

        int recHostId; // Is this from Web? Or standalone
        int connectionId; // Which user is sending me this?
        int channelId; // Which lane is he sending that message from

        byte[] recBuffer = new byte[BYTE_SIZE];
        int dataSize;

        NetworkEventType type = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, BYTE_SIZE, out dataSize, out error);

        switch(type)
        {
            case NetworkEventType.DataEvent:
                OnData(connectionId, channelId, recHostId, NetMessageSerializer.Deserialize(recBuffer));
                break;

            case NetworkEventType.Nothing:
                break;

            default:
                Debug.Log("Received Event of Type: " + type);
                break;
        }
    }

    #region OnData
    private void OnData(int connectionId, int channelId, int recHostId, NetMessage netMessage)
    {
        switch(netMessage.Code)
        {
            case NetCodes.None:
                Debug.Log("NetOperation Code: NONE");
                break;

            case NetCodes.ClientSpawned:
                SpawnClient((NetClientSpawned)netMessage);
                break;
        }
    }

    private void SpawnClient(NetClientSpawned message)
    {
        Debug.Log(string.Format("Client id: {0}, spawned at position: {1}, {2}, {3}", message.Player.id, message.Player.posX, message.Player.posY, message.Player.posZ));
    }

    #endregion


    #region Send
    public void SendServer(NetMessage netMessage, bool reliable = true)
    {
        if(reliable)
        {
            NetworkTransport.Send(hostId, connectionId, reliableChannel, NetMessageSerializer.Serialize(netMessage, BYTE_SIZE), BYTE_SIZE, out error);
        } else
        {
            NetworkTransport.Send(hostId, connectionId, unreliableChannel, NetMessageSerializer.Serialize(netMessage, BYTE_SIZE), BYTE_SIZE, out error);
        }
    }
    #endregion

    //public void TESTFunctionCreateAccount()
    //{
    //    NetCreateAccount ca = new NetCreateAccount();
    //    ca.Username = "Spinel";
    //    ca.Password = "123456";
    //    ca.Email = "andretortolano@hotmail.com";

    //    SendServer(ca);
    //}
}
