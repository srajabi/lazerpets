using UnityEngine;
using System.Collections.Generic;

public class Network : MonoBehaviour
{
    public const int CONNECTION_PORT = 64000;

    public enum NetworkMode { CLIENT, SERVER };
    public NetworkMode MyNetworkMode;

    public Server Server;
    public string ServerHostName = "localhost";

    public Client Client;

    void Start()
    {
        switch(MyNetworkMode)
        {
            case NetworkMode.CLIENT:
                Client = new Client(ServerHostName, CONNECTION_PORT);
            break;
            case NetworkMode.SERVER:
                Server = new Server(CONNECTION_PORT);
            break;
        }
    }
}
