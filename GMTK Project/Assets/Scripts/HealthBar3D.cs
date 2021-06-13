using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar3D : MonoBehaviour
{
    public Transform bar;

    public void SetHealth(float percentage)
    {
        if(!gameObject.activeInHierarchy) gameObject.SetActive(true);

        bar.localScale = new Vector3(percentage, bar.localScale.y, bar.localScale.z);
        //bar.localPosition = new Vector3(-bar.localScale.x / 2F, bar.localPosition.y, bar.localPosition.z);
    }
}
