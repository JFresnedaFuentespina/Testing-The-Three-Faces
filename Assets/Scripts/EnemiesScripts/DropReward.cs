using UnityEngine;

public class DropReward : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject heartReward;
    public GameObject moneyReward;
    public float dropHeartChance = 0.4f; // 40% chance to drop the reward
    public float dropCoinChance = 0.4f; // 40% chance to drop the reward

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
        Vector3 dropPosition = transform.position + new Vector3(1f, 1f, 0);
        if (randomValue <= dropHeartChance)
        {
            Instantiate(heartReward, dropPosition, Quaternion.identity);
        }

        float randomValueCoin = Random.Range(0f, 1f);
        Vector3 dropCoinPosition = transform.position + new Vector3(-1, 1f, 0);
        if (randomValueCoin <= dropCoinChance)
        {
            Instantiate(moneyReward, dropCoinPosition, Quaternion.identity);
        }
    }
}
