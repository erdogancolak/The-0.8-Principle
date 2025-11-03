using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_Text roomIdText;
    public TMP_Text infoText;
    public Button startButton;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            roomIdText.text = "Room ID: " + PhotonNetwork.CurrentRoom.Name;

            startButton.interactable = PhotonNetwork.IsMasterClient;
            infoText.text = PhotonNetwork.IsMasterClient ?
                "You are the host. Click Start when ready!" :
                "Waiting for host to start...";
        }
    }

    public void OnClickStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene"); 
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        infoText.text = "Player joined: " + newPlayer.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        infoText.text = "Player left: " + otherPlayer.NickName;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButton.interactable = PhotonNetwork.IsMasterClient;
        infoText.text = "New host: " + newMasterClient.NickName;
    }
}
