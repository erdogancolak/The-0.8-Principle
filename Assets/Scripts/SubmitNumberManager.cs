using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SubmitNumberManager : MonoBehaviourPun
{
    public static SubmitNumberManager Instance;

    public GameObject resultPanelPrefab;
    public GameObject resultPlayerPrefab;

    private GameObject spawnedResultPanel;

    private Dictionary<string, int> submittedNumbers = new Dictionary<string, int>();

    void Awake()
    {
        Instance = this;
    }

    public void SubmitNumber(int selectedNum)
    {
        if (spawnedResultPanel == null)
        {
            spawnedResultPanel = Instantiate(resultPanelPrefab, FindObjectOfType<Canvas>().transform);
            Transform content = spawnedResultPanel.transform.Find("Scroll View/Viewport/Content");

            foreach (Transform child in content)
                Destroy(child.gameObject);
        }

        photonView.RPC("RPC_PlayerSubmit", RpcTarget.All, PhotonNetwork.NickName, selectedNum);
    }


    [PunRPC]
    void RPC_PlayerSubmit(string playerName, int number)
    {
        if (!submittedNumbers.ContainsKey(playerName))
        {
            submittedNumbers.Add(playerName, number);
        }

        if (spawnedResultPanel != null)
        {
            Transform content = spawnedResultPanel.transform.Find("Scroll View/Viewport/Content");

            foreach (Transform child in content)
                Destroy(child.gameObject);

            foreach (var kvp in submittedNumbers)
            {
                AddPlayerToScroll(kvp.Key, kvp.Value);
            }
        }

        CheckIfAllSubmitted();
    }

    void AddPlayerToScroll(string playerName, int number)
    {
        Transform content = spawnedResultPanel.transform.Find("Scroll View/Viewport/Content");
        GameObject obj = Instantiate(resultPlayerPrefab, content);

        obj.transform.Find("Nickname").GetComponent<TMP_Text>().text = playerName;

        if (RuleManager.Instance.activeRule == GameRule.HiddenVotes)
        {
            obj.transform.Find("ResultNumber").transform.GetChild(0).GetComponent<TMP_Text>().text = "?";
        }
        else
        {
            obj.transform.Find("ResultNumber").transform.GetChild(0).GetComponent<TMP_Text>().text = "?";
        }

        float hp = PlayerStatus.Instance.playerHP[playerName];
        obj.transform.Find("PlayerHP").transform.GetChild(0).GetComponent<TMP_Text>().text = hp.ToString();

        bool isLocal = (playerName == PhotonNetwork.NickName);
        obj.transform.Find("PlayerShow").gameObject.SetActive(isLocal);
    }

    void CheckIfAllSubmitted()
    {
        if (submittedNumbers.Count == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            if (RuleManager.Instance.activeRule != GameRule.HiddenVotes)
            {
                photonView.RPC("RPC_RevealAllNumbers", RpcTarget.All);
            }

            StartCoroutine(PlayAverageNumber());
        }
    }

    IEnumerator PlayAverageNumber()
    {
        TMP_Text averageText = spawnedResultPanel.transform.Find("AverageNumber")?.GetComponent<TMP_Text>();
        TMP_Text lastResultText = spawnedResultPanel.transform.Find("LastResultNumber")?.GetComponent<TMP_Text>();
        TMP_Text winnerNicknameText = spawnedResultPanel.transform.Find("WinnerNickname")?.GetComponent<TMP_Text>();

        if (averageText == null || lastResultText == null || winnerNicknameText == null)
            yield break;

        float duration = 3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            int randomNumber = Random.Range(0, 999);
            averageText.text = randomNumber.ToString();
            elapsed += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        float avg = (float)submittedNumbers.Values.Average();
        averageText.text = avg.ToString();

        yield return new WaitForSeconds(0.5f);

        duration = 1.5f;
        elapsed = 0f;

        float multiplier = 0.8f;
        if (RuleManager.Instance.activeRule == GameRule.RandomMultiplier)
        {
            multiplier = Random.Range(0.5f, 0.9f);
        }

        float target = avg * multiplier;
        float epsilon = 0.01f;

        while (elapsed < duration)
        {
            float randomNumber = Random.Range(0f, target + 50f);
            lastResultText.text = randomNumber.ToString("F1");
            elapsed += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        lastResultText.text = target.ToString("F1");

        yield return new WaitForSeconds(1f);

        float minDiff = submittedNumbers.Min(kvp => Mathf.Abs(kvp.Value - target));

        var winners = submittedNumbers
            .Where(kvp => Mathf.Abs(Mathf.Abs(kvp.Value - target) - minDiff) < epsilon)
            .Select(kvp => kvp.Key)
            .ToList();

        if (RuleManager.Instance.activeRule == GameRule.TiePlayersLose)
        {
            if (winners.Count > 1)
            {
                Debug.Log("Kural aktif: TiePlayersLose → Berabere kalanlar kaybetti.");

                winners.Clear();

                float secondMin = submittedNumbers
                    .Where(kvp => Mathf.Abs(kvp.Value - target) > minDiff)
                    .Min(kvp => Mathf.Abs(kvp.Value - target));

                winners = submittedNumbers
                    .Where(kvp => Mathf.Abs(kvp.Value - target) == secondMin)
                    .Select(kvp => kvp.Key)
                    .ToList();
            }
        }

        winnerNicknameText.text = $"Winner(s): {string.Join(", ", winners)}";

        yield return new WaitForSeconds(0.8f);

        if (PhotonNetwork.IsMasterClient)
        {
            foreach (var kvp in submittedNumbers)
            {
                string player = kvp.Key;

                if (!winners.Contains(player))
                {
                    PhotonView pv = PlayerStatus.Instance.GetComponent<PhotonView>();
                    pv.RPC("RPC_TakeDamage", RpcTarget.All, player, 0.5f);
                }
            }

            photonView.RPC("RPC_UpdateHPUI", RpcTarget.All);
        }
    }

    [PunRPC]
    void RPC_UpdateHPUI()
    {
        Transform content = spawnedResultPanel.transform.Find("Scroll View/Viewport/Content");

        foreach (Transform item in content)
        {
            string name = item.transform.Find("Nickname").GetComponent<TMP_Text>().text;
            float hp = PlayerStatus.Instance.playerHP[name];
            item.transform.Find("PlayerHP").transform.GetChild(0).GetComponent<TMP_Text>().text = hp.ToString();
        }
    }

    [PunRPC]
    void RPC_RevealAllNumbers()
    {
        Transform content = spawnedResultPanel.transform.Find("Scroll View/Viewport/Content");

        foreach (Transform item in content)
        {
            string name = item.transform.Find("Nickname").GetComponent<TMP_Text>().text;

            if (submittedNumbers.ContainsKey(name))
            {
                int number = submittedNumbers[name];
                item.transform.Find("ResultNumber").transform.GetChild(0).GetComponent<TMP_Text>().text = number.ToString();
            }
        }
    }
}
