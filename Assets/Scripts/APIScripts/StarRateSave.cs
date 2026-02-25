using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StarRateSave : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string name;
    public Image[] stars;
    private int rate;

    void Start()
    {
        stars = new Image[5];
        for (int i = 0; i < stars.Count(); i++)
        {
            int starIndex = i + 1;
            Button btn = stars[i].GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => SetRate(starIndex));
            }
        }
    }

    public void SetRate(int value)
    {
        Debug.Log("Rate = " + value);
        rate = value;
    }

    public int GetRate()
    {
        return rate;
    }
}
