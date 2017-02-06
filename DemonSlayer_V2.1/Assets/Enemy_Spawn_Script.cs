using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Spawn_Script : MonoBehaviour {

    public float appearTimeDelay = 3;
    private GameObject subRootGO;
    void Start()
    {
        StartCoroutine(AppearDelay());
    }
    IEnumerator AppearDelay()
    {
        yield return new WaitForSeconds(appearTimeDelay);
        transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(DisableParticleDelay());
        GetComponent<Enemy_Script>().enabled = true;
    }
    IEnumerator DisableParticleDelay()
    {
        yield return new WaitForSeconds(1);
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
    }
}
