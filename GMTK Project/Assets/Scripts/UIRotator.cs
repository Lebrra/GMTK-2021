using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRotator : MonoBehaviour
{
    public Transform playerLocation;
    public Transform targetLocation;

    RectTransform spinningRect;
    [SerializeField]
    bool shown = false;
    public float minDistance = 5F;

    private void Start()
    {
        spinningRect = transform.GetChild(0).GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (!spinningRect || !shown) return;

        if (Vector3.Distance(playerLocation.position, targetLocation.position) < minDistance)
        {
            spinningRect?.gameObject.SetActive(false);
        }
        else spinningRect?.gameObject.SetActive(true);

        if (spinningRect.gameObject.activeInHierarchy)
        {
            Vector3 direction = targetLocation.position - playerLocation.position;
            //Vector3 direction = playerLocation.position - targetLocation.position;
            direction = new Vector3(direction.x, 0, direction.z);

            float angle = Vector3.SignedAngle(direction, targetLocation.forward, Vector3.up);
            spinningRect.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    /*
    IEnumerator UpdateCompass()
    {
        yield return new WaitForSeconds(2F);

        float zDiff = playerLocation.position.z - transform.position.z;
        float xDiff = playerLocation.position.x - transform.position.x;

        float angle = Mathf.Tan(zDiff / xDiff);

        //arrow.Rotate(new Vector3(0, 0, Mathf.Rad2Deg * angle));
        arrow.rotation = Quaternion.Euler(new Vector3(0, 0, Mathf.Rad2Deg * angle));

        StartCoroutine(UpdateCompass());
    }*/

    public void ShowUI(bool show)
    {
        shown = show;

    }
}
