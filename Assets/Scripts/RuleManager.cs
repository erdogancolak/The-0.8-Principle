using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public enum GameRule
{
    None,
    TiePlayersLose,
    RandomMultiplier,
    HiddenVotes
}

public class RuleManager : MonoBehaviourPun
{
    public static RuleManager Instance;

    public GameRule activeRule = GameRule.None;
    public int currentRound = 1;

    private List<GameRule> unusedRules = new List<GameRule>();

    private void Awake()
    {
        Instance = this;

        unusedRules.Clear();
        unusedRules.Add(GameRule.RandomMultiplier);
        unusedRules.Add(GameRule.HiddenVotes);
    }

    public void OnNewRound()
    {
        currentRound++;

        if (currentRound == 4)
        {
            photonView.RPC("RPC_SetRule", RpcTarget.All, (int)GameRule.TiePlayersLose);
            return;
        }

        if (currentRound == 8)
        {
            int randomIndex = Random.Range(0, unusedRules.Count);
            GameRule chosen = unusedRules[randomIndex];

            unusedRules.RemoveAt(randomIndex);

            photonView.RPC("RPC_SetRule", RpcTarget.All, (int)chosen);
            return;
        }

        if (currentRound == 12)
        {
            if (unusedRules.Count == 1)
            {
                photonView.RPC("RPC_SetRule", RpcTarget.All, (int)unusedRules[0]);
                unusedRules.Clear();
            }
            return;
        }
    }

    [PunRPC]
    void RPC_SetRule(int rule)
    {
        activeRule = (GameRule)rule;

        if (activeRule == GameRule.RandomMultiplier)
        {
            float m = Random.Range(0.5f, 0.9f);
            Debug.Log("Rastgele Çarpan aktif → " + m);
        }

        Debug.Log("Yeni Aktif Kural: " + activeRule);
    }
}
