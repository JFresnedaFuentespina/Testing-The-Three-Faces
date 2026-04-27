using System.Collections;
using UnityEngine;

public class CantoCastMagicAttack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject projectile;
    private Transform suelo;
    public int amount = 10;
    public float spawnAreaX = 2f;
    public float spawnAreaZ = 2f;
    void Start()
    {
        suelo = GameObject.Find("SueloBossRoom").transform;
        if (suelo == null)
        {
            Debug.LogWarning("No se encontró el objeto 'SueloBossRoom' ");
        }
    }

    public void CastThunders()
    {
        Renderer r = suelo != null ? suelo.GetComponent<Renderer>() : null;
        Bounds bounds = r != null ? r.bounds : new Bounds(transform.position, new Vector3(spawnAreaX * 2, 0, spawnAreaZ * 2));
        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPos = new Vector3(Random.Range(bounds.min.x, bounds.max.x),
                                            0f,
                                            Random.Range(bounds.min.z, bounds.max.z));
            StartCoroutine(CastThunder(spawnPos));
        }
    }

    private IEnumerator CastThunder(Vector3 spawnPos)
    {
        float random = Random.Range(0f, 1f);
        yield return new WaitForSecondsRealtime(random);
        Instantiate(projectile, spawnPos, Quaternion.identity);
    }
}
