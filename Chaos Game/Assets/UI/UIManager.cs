using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider healthbar;

    public static UIManager instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
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
