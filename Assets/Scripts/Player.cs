using System;
using System.Collections.Generic;
using Multiplayer;
using RiptideNetworking;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Dictionary<ushort, Player> list = new ();
    
    public ushort ID { get; private set; }
    public string Username { get; private set; }


    private void OnDestroy()
    {
        list.Remove(ID);
    }

    public static void Spawn(ushort ID, string username)
    {
        foreach (var othePlayer in list.Values)
        {
            othePlayer.SendSpawned(ID);
        }
        
        var player = Instantiate(GameLogic.Singleton.PlayerPrefab, new Vector3(0f, 1f, 0f), Quaternion.identity)
            .GetComponent<Player>();
        player.name = $"Player {ID} {(string.IsNullOrEmpty(username) ? "Guest" : username)}";
        player.ID = ID;
        player.Username = string.IsNullOrEmpty(username) ? $"Guest {ID}" : username;
        player.SendSpawned();
        list.Add(ID, player);
    }

    #region Messages

    private void SendSpawned()
    {
        var message = Message.Create(MessageSendMode.reliable, ServerToClientID.playerSpawned);
        NetworkManager.Singleton.Server.SendToAll(AddSpawnData(message));
    }

    private void SendSpawned(ushort toClientID)
    {
        var message = Message.Create(MessageSendMode.reliable, ServerToClientID.playerSpawned);
        NetworkManager.Singleton.Server.Send(AddSpawnData(message), toClientID);
    }

    private Message AddSpawnData(Message message)
    {
        message.AddUShort(ID);
        message.AddString(Username);
        message.AddVector3(transform.position);
        return message;
    }
    
    [MessageHandler((ushort)ClientToServerID.name)]
    public static void Name(ushort fromClientID, Message message)
    {
        Spawn(fromClientID, message.GetString());
    }

    #endregion
    
}