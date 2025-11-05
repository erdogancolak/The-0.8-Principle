using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomPlayerListManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Transform contentParent;       
    public GameObject playerItemPrefab;

    public TMP_Text roomCode;

    private Dictionary<Player, GameObject> playerItems = new Dictionary<Player, GameObject>();


    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        RefreshPlayerList();
    }

    public override void OnJoinedRoom()
    {
        RefreshPlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        RefreshPlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RefreshPlayerList();
    }

    void RefreshPlayerList()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        playerItems.Clear();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject newItem = Instantiate(playerItemPrefab, contentParent);
            playerItems[player] = newItem;

            TMP_Text nicknameText = newItem.transform.Find("Nickname").GetComponent<TMP_Text>();
            Image playershow = newItem.transform.Find("PlayerShow").GetComponent<Image>();
            Image ismasterclient = newItem.transform.Find("isMasterClient").GetComponent<Image>();

            nicknameText.text = player.NickName;

            playershow.gameObject.SetActive(player == PhotonNetwork.LocalPlayer);

            ismasterclient.gameObject.SetActive(player.IsMasterClient);
        }

        roomCode.text = PhotonNetwork.CurrentRoom.Name;
    }

    public void StartGameButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
        else
        {

        }
    }
}
