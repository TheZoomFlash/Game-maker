using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
	public Slider slider;
	public Gradient gradient;
	public Image fill;

    public bool IsBarLengthChange = true;
    public int maxBarLength = 200;

    public void SetMaxHealth(int health)
	{
        //Debug.Log("SetMaxHealth" + health);
		slider.maxValue = health;
		slider.value = health;

		fill.color = gradient.Evaluate(1f);

        if(IsBarLengthChange)
        {
            RectTransform rec = transform.parent.GetComponent<RectTransform>();
            if (rec)
                rec.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                    Mathf.Clamp(health * 20, 0, maxBarLength));
        }
    }

    public void SetHealth(int health)
	{
        //Debug.Log("SetHealth" + health);
        slider.value = health;

		fill.color = gradient.Evaluate(slider.normalizedValue);
	}

}
