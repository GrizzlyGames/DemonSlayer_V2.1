using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate_Object_Script : MonoBehaviour {

    public int rotationSpeed;
    public float bobSpeed;

	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.localPosition.y + (Mathf.Sin(Time.time * bobSpeed) * 0.001f), transform.position.z);
	}
}
