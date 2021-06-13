using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTimer : MonoBehaviour
{
   public IEnumerator LifeSpan(float time)
    {
        yield return new WaitForSeconds(time);

        Destroy(gameObject);
    }
}
