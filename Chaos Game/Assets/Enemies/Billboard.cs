using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Slider healthbar;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI critText;
    [SerializeField] private Transform textSpawnLocation;

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

    public void SpawnDamageNumber(string txt, bool isCrit)
    {
        TextMeshProUGUI cacheObj;

        if (isCrit)
            //instantiate damage number at location
            cacheObj = Instantiate(critText, textSpawnLocation);
        else
            cacheObj = Instantiate(damageText, textSpawnLocation);


        //set text
        cacheObj.text = txt;

        //play damage number animation
        cacheObj.GetComponent<Animator>().Play("DamageNumber");

        //destroy damage number (animation.time)
        Destroy(cacheObj.gameObject, 1);
    }
}
