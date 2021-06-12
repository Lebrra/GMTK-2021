using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubReference : MonoBehaviour
{
    public static HubReference reference;

    void Awake()
    {
        if (reference) Destroy(gameObject);
        else reference = this;
    }
}
