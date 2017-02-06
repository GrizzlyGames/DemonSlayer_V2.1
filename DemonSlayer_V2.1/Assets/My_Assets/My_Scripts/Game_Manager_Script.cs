using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager_Script : MonoBehaviour
{
    public static Game_Manager_Script instance;

    public GameObject playerGO;                             // Get the player GO from prefabs folder
    public GameObject healthPickUpGO;
    public GameObject ammoPickUpGO;
    public GameObject dropShipGO;
    public GameObject[] Enemy_GO_Ary;

    public Transform spawnLoc;
    public float xSpwnRng;
    public float ySpwn;
    public float zSpwnRng;

    public int spawnNumEnemies = 1;
    public int currentNumEnemies = 0;

    void Awake()                                            // First function to run in scene
    {
        instance = this;                                    // This object is the only instance of Game_Controller_Script
    }

    // Use this for initialization
    void Start()
    {
        // SpawnDropShip();
        //Instantiate(playerGO, new Vector3(-50, 2, 0), Quaternion.Euler(0, 90, 0));         // Create player in scene
        SpawnEnemy();
    }

    public void SpawnPickUps()
    {
        GameObject[] ammoGO = GameObject.FindGameObjectsWithTag("MaxAmmo") as GameObject[];
        GameObject[] healthGO = GameObject.FindGameObjectsWithTag("MaxHealth") as GameObject[];

        for (var i = 0; i < ammoGO.Length; i++)
        {
            Destroy(ammoGO[i]);
        }
        for (var i = 0; i < healthGO.Length; i++)
        {
            Destroy(healthGO[i]);
        }

        Instantiate(healthPickUpGO, new Vector3(-12, 2.5f, 28), Quaternion.Euler(0, 0, 90));
        Instantiate(ammoPickUpGO, new Vector3(-12, 2.5f, -28), Quaternion.Euler(0, 0, 90));
    }
    public void SpawnDropShip()
    {
        SpawnPickUps();
        Instantiate(dropShipGO, new Vector3(-26, 500, 0), Quaternion.identity);
    }
    public void SpawnEnemy()
    {
        for (int i = 0; i < spawnNumEnemies; i++)
        {
            int rndNum = Random.Range(0, Enemy_GO_Ary.Length);
            GameObject go = Instantiate(Enemy_GO_Ary[rndNum]) as GameObject;
            go.transform.position = TeleportationSpawnLocation(xSpwnRng, ySpwn, zSpwnRng);
            go.transform.parent = spawnLoc;
            currentNumEnemies++;
            Debug.Log("Current number of enemies: " + currentNumEnemies);
        }
    }

    private Vector3 TeleportationSpawnLocation(float x, float y, float z)
    {
        float mX = 0;
        float mZ = 0;

        mX = Random.Range(-x, x + 1);
        mZ = Random.Range(-z, z + 1);
               
        Vector3 newPos = new Vector3(spawnLoc.position.x + mX, y, spawnLoc.position.z + mZ);

        Debug.Log("Enemy spawn position: " + newPos);

        return (newPos);
    }
}