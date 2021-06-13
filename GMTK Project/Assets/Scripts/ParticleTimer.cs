using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTimer : MonoBehaviour
{
    float liveTime = 0F;

    private void Update()
    {
        liveTime += Time.deltaTime;
        if (liveTime > 4F) Destroy(gameObject);
    }

    public void LifeSpan(float time)
    {
        StartCoroutine(EndLife(time));
    }

    IEnumerator EndLife(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }
}
