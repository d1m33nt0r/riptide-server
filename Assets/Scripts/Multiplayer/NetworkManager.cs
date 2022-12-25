using RiptideNetworking;
using RiptideNetworking.Utils;
using UnityEngine;

namespace Multiplayer
{
    public enum ServerToClientID : ushort
    {
        playerSpawned = 1,
        playerMovement
    }
    
    public enum ClientToServerID : ushort
    {
        name = 1,
        input
    }
    
    public class NetworkManager : MonoBehaviour
    {
        private static NetworkManager _singleton;

        public static NetworkManager Singleton
        {
            get => _singleton;
            private set
            {
                if (_singleton == null)
                    _singleton = value;
                else if (_singleton != value)
                {
                    Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying duplicate!");
                    Destroy(value);
                }
            }
        }

        public Server Server { get; private set; }

        [SerializeField] private ushort port;
        [SerializeField] private ushort maxClientsCount;
        
        private void Awake()
        {
            Singleton = this;
        }

        private void Start()
        {
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
            
            Server = new Server();
            Server.Start(port, maxClientsCount);
            Server.ClientDisconnected += PlayerLeft;
        }

        private void FixedUpdate()
        {
            Server.Tick();
        }

        private void OnApplicationQuit()
        {
            Server.Stop();
        }

        private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
        {
            if (Player.list.TryGetValue(e.Id, out Player player))
                Destroy(player.gameObject);
        }
    }
}