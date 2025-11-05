using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public Image fillImage;         
    public RectTransform knob;     
    public RectTransform startPoint; 
    public RectTransform endPoint;

    [Header("Loading Settings")]
    public float minLoadingTime;
    public float fillSpeed;        

    private bool isConnected = false;
    private float timer = 0f;

    void Start()
    {
        fillImage.fillAmount = 0f;
        PhotonNetwork.ConnectUsingSettings(); 
        StartCoroutine(LoadingRoutine());
    }

    IEnumerator LoadingRoutine()
    {
        
        while (timer < minLoadingTime || !isConnected)
        {
            timer += Time.deltaTime * fillSpeed;

            
            fillImage.fillAmount = Mathf.Clamp01(timer / minLoadingTime);

            
            knob.position = Vector3.Lerp(startPoint.position, endPoint.position, fillImage.fillAmount);

            yield return null;
        }

        
        OnLoadingComplete();
    }

    public override void OnConnectedToMaster()
    {
        isConnected = true;
    }

    void OnLoadingComplete()
    {
        if(isConnected)
        {
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
