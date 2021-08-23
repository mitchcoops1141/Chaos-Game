using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemAnimListener : MonoBehaviour
{
    [SerializeField] private GameObject punchOneHitbox;
    [SerializeField] private GameObject punchTwoHitbox;
    [SerializeField] private GameObject singlePunchHitbox;
    public void ShowPunchOneHitbox()
    {
        punchOneHitbox.SetActive(true);
    }

    public void HidePunchOneHitbox()
    {
        punchOneHitbox.SetActive(false);
    }

    public void ShowPunchTwoHitbox()
    {
        punchTwoHitbox.SetActive(true);
    }

    public void HidePunchTwoHitbox()
    {
        punchTwoHitbox.SetActive(false);
    }

    public void SinglePunchShowHitbox()
    {
        singlePunchHitbox.SetActive(true);
    }

    public void SinglePunchHideHitbox()
    {
        singlePunchHitbox.SetActive(false);
    }
}
