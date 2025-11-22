using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviourPunCallbacks   
{
    public static PlayerStatus Instance;

    public Dictionary<string, float> playerHP = new Dictionary<string, float>();

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (!playerHP.ContainsKey(p.NickName))
                playerHP.Add(p.NickName, 10);
        }
    }

    public override void OnJoinedRoom()
    {
        if (!playerHP.ContainsKey(PhotonNetwork.NickName))
        {
            playerHP.Add(PhotonNetwork.NickName, 10);
            Debug.Log("HP eklendi (local): " + PhotonNetwork.NickName);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!playerHP.ContainsKey(newPlayer.NickName))
        {
            playerHP.Add(newPlayer.NickName, 10);
            Debug.Log("HP eklendi (remote): " + newPlayer.NickName);
        }
    }

    [PunRPC]
    public void RPC_TakeDamage(string nick, float amount)
    {
        playerHP[nick] -= amount;
        Debug.Log($"{nick} Damage aldı! Yeni HP: {playerHP[nick]}");
    }
}
