using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyLife : MonoBehaviour
{
    // Start is called before the first frame update
    public float totalHp = 10f;
    public float currentHp;
    private bool isAlive = true;
    public GameObject healthBar;
    private Image fillImage;

    void Start()
    {
        currentHp = totalHp;
        if (healthBar != null)
        {
            Transform fillTransform = healthBar.transform.Find("Canvas/Fill");
            if (fillTransform != null)
                fillImage = fillTransform.GetComponent<Image>();
        }
    }
    public void Damage(float hit)
    {
        if (!isAlive) return;

        currentHp -= hit;
        currentHp = Mathf.Clamp(currentHp, 0f, totalHp);

        UpdateHealthBar();
        UpdateIsAlive();
    }

    public void UpdateIsAlive()
    {
        if (currentHp <= 0 && isAlive)
        {
            isAlive = false;
            Die();
        }
    }

    public bool GetIsAlive()
    {
        return isAlive;
    }

    void UpdateHealthBar()
    {
        if (fillImage != null)
        {
            fillImage.fillAmount = currentHp / totalHp;
        }
    }

    public void Die()
    {
        Debug.Log($"{gameObject.name} murió");
        Destroy(gameObject);
    }
}
