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

    public IEnumerator LifeSpan(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }
}
