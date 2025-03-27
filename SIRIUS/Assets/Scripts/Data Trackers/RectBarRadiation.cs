using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RectBarRadiation : MonoBehaviour
{
    public Image fill;
    public TextMeshProUGUI amount;
    public int currentValue;

    void Start()
    {
        fill.fillAmount = Normalize();
        amount.text = $"{currentValue}" + "%";
    }

    public void Add(int val)
    {
        if (currentValue > 100) 
        {
            currentValue = 100;
        }
        else {
            currentValue += val;
        }

        fill.fillAmount = Normalize();
        amount.text = $"{currentValue}" + "%";

    }

    private float Normalize()
    {
        return (float)(currentValue / 100.0);
    }
}
    