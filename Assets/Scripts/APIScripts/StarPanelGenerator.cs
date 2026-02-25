using UnityEngine;
using UnityEngine.UI;

public class StarPanelGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button starButton;
    public int numberOfStars = 5;
    public int rate;
    public string category;
    public Image[] starImages;
    void Start()
    {
        starImages = new Image[numberOfStars];
        GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = numberOfStars;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.MiddleCenter;

        for (int i = 0; i < numberOfStars; i++)
        {
            Button star = Instantiate(starButton, transform);
            star.name = "Star_" + (i + 1);
            
            Image starImage = star.transform.Find("Fill").GetComponent<Image>();
            starImages[i] = starImage;
            starImage.fillAmount = 0;

            int starIndex = i + 1;
            star.onClick.AddListener(() =>
            {
                Debug.Log($"Click en {star.name}, índice = {starIndex}");
                SetRate(starIndex);
            });
        }
    }

    public void SetRate(int value)
    {
        rate = value;
        UpdateStars();
    }

    private void UpdateStars()
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            // Estrella completa si está dentro del rating, vacía si no
            starImages[i].fillAmount = (i < rate) ? 1f : 0f;
        }
    }
}
