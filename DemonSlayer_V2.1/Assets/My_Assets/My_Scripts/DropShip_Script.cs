using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShip_Script : MonoBehaviour
{ 
    private bool descending = true;

    private int descendSpeed = 100;
    private int ascendingSpeed = 25;

    void Update()
    {
        if (transform.position.y > 5 && descending)
        {
            if (transform.position.y < 101 && transform.position.y > 17)
                descendSpeed = 100;
            else if (transform.position.y <= 17)
                descendSpeed = 3;
            transform.Translate(Vector3.up * -descendSpeed * Time.deltaTime);
            if (transform.position.y <= 5)
                StartCoroutine("DescendingDelay");
        }
        else if (!descending)
        {           
            if (transform.position.y > 17)
                ascendingSpeed = 50;

            transform.Translate(Vector3.up * ascendingSpeed * Time.deltaTime);

            if (transform.position.y > 500)
            {                
                Destroy(gameObject);
            }                
        }
    }

    IEnumerator DescendingDelay()
    {
        Game_Manager_Script.instance.SpawnEnemy();
        Game_Manager_Script.instance.spawnNumEnemies = Game_Manager_Script.instance.spawnNumEnemies * 2;
        yield return new WaitForSeconds(4);
        descending = false;
    }  
}
