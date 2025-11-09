using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPanelPrefab;

    private void Start()
    {
        if (PhotonNetwork.InRoom)
        {
            CreateMyPanel();
        }
    }

    void CreateMyPanel()
    {
        GameObject panel = Instantiate(playerPanelPrefab, FindObjectOfType<Canvas>().transform);
        panel.name = "Panel_" + PhotonNetwork.NickName;

        panel.GetComponent<CanvasGroup>().alpha = 1f;
        panel.GetComponent<CanvasGroup>().interactable = true;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
