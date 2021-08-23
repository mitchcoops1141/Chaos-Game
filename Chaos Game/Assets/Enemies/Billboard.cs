using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Slider healthbar;
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }

    public void UpdateHealthBarMaxValue(float value)
    {
        healthbar.maxValue = value;
    }

    public void UpdateHealthBarValue(float value)
    {
        healthbar.value = value;
    }
}
