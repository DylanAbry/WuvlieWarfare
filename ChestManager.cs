using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Random = UnityEngine.Random;

public class ChestManager : MonoBehaviour
{
    [Header("Chest Rewards")]
    public GameObject bombPrefab;
    public GameObject pistolPrefab;
    public GameObject machineGunPrefab;
    public GameObject chestPointsDisplay;

    [Header("Chest Points")]
    public TextMeshProUGUI chestPointsDisplayText;

    public bool chestBonusOpened;
    public int chestBonusScore;

    [Header("Chest Holders")]
    public Transform[] chestHolders;
    public List<Transform> usedHolders = new List<Transform>();

    private void Start()
    {
        SetupChests();
    }

    public void SetupChests()
    {
        if (chestHolders.Length != 6)
        {
            Debug.LogError("Exactly 6 chest holders are required!");
            return;
        }

        usedHolders.Clear();

        // Create a list of all six rewards
        List<GameObject> chestRewards = new List<GameObject>()
        {
        machineGunPrefab,
        pistolPrefab,
        chestPointsDisplay,
        bombPrefab,
        bombPrefab,
        bombPrefab
        };

        // Randomly shuffle the rewards
        for (int i = chestRewards.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            GameObject temp = chestRewards[i];
            chestRewards[i] = chestRewards[randomIndex];
            chestRewards[randomIndex] = temp;
        }

        chestBonusScore = Random.Range(1, 6);
        chestPointsDisplayText.text = string.Format("+" + chestBonusScore);

        // Assign each reward to a unique chest holder
        for (int i = 0; i < chestHolders.Length; i++)
        {
            Transform holder = chestHolders[i];
            GameObject reward = chestRewards[i];

            if (reward == chestPointsDisplay)
            {
                chestPointsDisplay.transform.SetParent(holder, false);
                chestPointsDisplay.transform.localPosition = Vector3.zero;
                chestPointsDisplay.transform.localRotation = Quaternion.identity;
            }
            else
            {
                // Instantiate the weapon or bomb prefab
                GameObject spawnedReward = Instantiate(reward, holder.position, holder.rotation, holder);

                if (reward == machineGunPrefab)
                {
                    spawnedReward.transform.localPosition = new Vector3(0.14f, 0f, 0f);
                }
            }

            usedHolders.Add(holder);
        }

        Debug.Log("All 6 chest rewards assigned!");
    }
}
