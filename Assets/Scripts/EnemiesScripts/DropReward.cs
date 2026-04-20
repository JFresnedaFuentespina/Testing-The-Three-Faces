using UnityEngine;

public class DropReward : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject rewardPrefab;
    public float dropChance = 0.4f; // 40% chance to drop the reward

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Drop()
    {
        float randomValue = Random.Range(0f, 1f);
        Vector3 dropPosition = transform.position + new Vector3(0, 1f, 0);
        if (randomValue <= dropChance)
        {
            Instantiate(rewardPrefab, dropPosition, Quaternion.identity);
        }
    }
}
