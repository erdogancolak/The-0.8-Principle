using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI Elements")]
    public TMP_InputField roomIdInput;

    [Header("Nickname")]
    public GameObject nicknamePanel;
    public TMP_InputField nicknameInput;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public void CreateRoom()
    {
        string randomRoomId = Random.Range(1000, 9999).ToString();
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(randomRoomId, options);
    }

    public void JoinRoom()
    {
        string roomId = roomIdInput.text;
        if (string.IsNullOrEmpty(roomId))
        {
            return;
        }

        PhotonNetwork.JoinRoom(roomId);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);
        PhotonNetwork.LoadLevel("RoomScene");
    }

    public void SetNicknameButton()
    {
        if(!nicknameInput.text.IsNullOrEmpty())
        {
            PhotonNetwork.NickName = nicknameInput.text;
            nicknamePanel.SetActive(false);
        }
    }
}
