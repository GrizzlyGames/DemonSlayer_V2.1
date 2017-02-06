using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class Enemy_Script : MonoBehaviour
{
    public bool alive = true;
    public bool canWalk = true;
    public float lookSpeed;
    public int health = 100;
    public int maxHealth = 100;
    public float rayLength = 50;
    public int damage = 15;

    public Image healthBarImage;

    public GameObject soulGO;

    private bool bAttacking = false;
    private Vector3 previousPosition;
    private Animator anim;
    private NavMeshAgent navMeshAgent;

    void Awake()
    {
        previousPosition = transform.position;
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        if (health > 0) // check if enemy is alive
        {
            #region Attack
            float distance = Vector3.Distance(transform.position, Player_Script.instance.PlayerTransform().position);     // Get distence from player // Debug.Log(distance);
            anim.SetFloat("targetDistence", distance);
            if (distance <= 3.1f && Player_Script.instance.health > 0 && !bAttacking)
                StartCoroutine(AttackDelay());
            #endregion
            #region Determine-Actor-Speed
            float curSpeed;
            Vector3 curMove = transform.position - previousPosition;
            curSpeed = curMove.magnitude / Time.deltaTime;      // Debug.Log("curSpeed: " + curSpeed);
            anim.SetFloat("speed", curSpeed);
            previousPosition = transform.position;
            #endregion      // Set actor movement animation state
            #region Chase-Player
            if (alive && canWalk)  // check if actor is alive
                navMeshAgent.destination = Player_Script.instance.PlayerTransform().position;     // Chase player
            else
                navMeshAgent.velocity = Vector3.zero;
            #endregion
            #region Look-At-Player
            Vector3 lookPos = Player_Script.instance.PlayerTransform().position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * lookSpeed);
            #endregion
        }
        else
        {
            navMeshAgent.velocity = Vector3.zero;
        }
    }
    public void TakeDamage(int dmg)
    {
        health -= dmg;  // Take damage Debug.Log(transform.name + " took " + dmg + " dmg. Remaing health: " + health);        
        healthBarImage.fillAmount = (float)health / (float)maxHealth;
        if (health > 0)
        {
            Player_Script.instance.pts += dmg;
            Player_UI_Controller_Script.instance.UpdatePointsText();
            canWalk = false;
            StartCoroutine(WalkDelay());
            int rnd = Random.Range(1, 3);
            switch (rnd)
            {
                case 1:
                    anim.SetTrigger("hit1");
                    break;
                case 2:
                    anim.SetTrigger("hit2");
                    break;
            }
        }
        else
        {
            if (alive)
            {
                healthBarImage.enabled = false;
                canWalk = false;
                anim.SetTrigger("dead");
                
                Player_Script.instance.pts += dmg * 3;
                Player_UI_Controller_Script.instance.UpdatePointsText();
                Game_Manager_Script.instance.currentNumEnemies--;
                if (Game_Manager_Script.instance.currentNumEnemies <=  (int)(Game_Manager_Script.instance.spawnNumEnemies / 4))
                {
                    Player_Script.instance.pts *= (int)1.1f;
                    Player_UI_Controller_Script.instance.UpdatePointsText();
                    Game_Manager_Script.instance.spawnNumEnemies *= 2;
                    Game_Manager_Script.instance.SpawnEnemy();
                }
                StartCoroutine(DeathDelay()); 
                alive = false;  // Killed   
            }
        }
    }
    
    IEnumerator AttackDelay()
    {
        bAttacking = true;
        int rnd = Random.Range(1, 4);
        anim.SetInteger("attack", rnd);
        Player_Script.instance.TakeDamage(damage);
        yield return new WaitForSeconds(1);
        anim.SetInteger("attack", 0);
        bAttacking = false;
    }
    IEnumerator WalkDelay()
    {
        yield return new WaitForSeconds(.5f);
        canWalk = true;
    }
    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(4);
        Destroy(gameObject);
    }
}
