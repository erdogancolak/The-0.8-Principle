using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPanelPrefab; // Her oyuncunun kendi paneli olacak

    private void Start()
    {
        // Her oyuncu için kendi panelini oluþtur
        if (PhotonNetwork.InRoom)
        {
            CreateMyPanel();
        }
    }

    void CreateMyPanel()
    {
        GameObject panel = Instantiate(playerPanelPrefab, FindObjectOfType<Canvas>().transform);
        panel.name = "Panel_" + PhotonNetwork.NickName;

        // Her oyuncu sadece kendi panelini görsün
        foreach (var text in panel.GetComponentsInChildren<Text>())
        {
            text.text = "Hello " + PhotonNetwork.NickName;
        }

        // Paneli diðer oyuncularla paylaþma (herkese görünmesin)
        panel.GetComponent<CanvasGroup>().alpha = 1f;
        panel.GetComponent<CanvasGroup>().interactable = true;
        panel.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
