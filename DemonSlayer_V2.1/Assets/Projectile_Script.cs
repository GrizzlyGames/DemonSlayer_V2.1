using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_Script : MonoBehaviour
{

    public float lifeTime = 3;
    public float speed = 10;

    void Start()
    {
        StartCoroutine(DestroyDelay());
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (!other.tag.Equals("Player"))
            Destroy(gameObject);
    }

    IEnumerator DestroyDelay()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}
