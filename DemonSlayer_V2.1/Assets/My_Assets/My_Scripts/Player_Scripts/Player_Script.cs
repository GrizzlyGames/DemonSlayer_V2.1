using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
public class Player_Script : MonoBehaviour
{
    #region Public_Variables
    public static Player_Script instance;
    #region Player_Stats
    public int pts;
    public bool bShieldRecharge;
    public float shieldRechargeWaitTime = 1;
    public int maxShield;
    public float shield;
    public int maxHealth;
    public int health;
    #endregion
    #region Weapon_Stats
    public float reloadBarSpeed;
    public int damage = 1;                                     // Set the number of hitpoints that this gun will take away from shot objects with a health script
    public float damageMultiplier = 1;
    public float fireRate = 0.25f;                                      // Number in seconds which controls how often the player can fire
    public float weaponRange = 50f;                                     // Distance in Unity units over which the player can fire
    public float hitForce = 100f;                                       // Amount of force which will be added to objects with a rigidbody shot by the player

    public int currentAmmo;
    public int magazineCapacity;
    public int maximumAmmo;
    #endregion
    #region Objects
    public Camera fpsCam;
    public Transform gunNozzleTrans;
    #endregion
    #endregion
    #region Private_Variables
    #region Stats

    #endregion
    #region
    private GameObject projectileGO;                                             // Holds a reference to the first person camera
    private AudioSource gunAudio;
    #endregion
    private LayerMask myLayerMask = 1 << 8;
    private float nextFire;
    private bool bReloading;
    #endregion

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        magazineCapacity = GetMagazineCapacity();
        currentAmmo = magazineCapacity;
        maximumAmmo = GetMaximumAmmo();
        Player_UI_Controller_Script.instance.UpdateAmmoText(currentAmmo.ToString() + " / " + maximumAmmo.ToString());

        // Get and store a reference to our AudioSource component
        gunAudio = GetComponent<AudioSource>();
    }



    void Update()
    {
        if (bShieldRecharge && shield < maxShield)
        {
            if (Player_UI_Controller_Script.instance.shieldBarHandle.enabled == false && shield > 0)
                Player_UI_Controller_Script.instance.shieldBarHandle.enabled = true;
            shield += 1.5f / shieldRechargeWaitTime * Time.deltaTime;
            Player_UI_Controller_Script.instance.UpdateShieldScrollbar();
        }
        if (health > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentAmmo >= 1 && !bReloading)
                {
                    currentAmmo--;      // Reduces current ammo
                    if (Input.GetButtonDown("Fire1") && Time.time > nextFire)       // Check if the player has pressed the fire button and if enough time has elapsed since they last fired
                    {
                        nextFire = Time.time + fireRate;        // Update the time when our player can fire next
                        gunAudio.Play();     // Play gunshot sound effect

                        #region Raycast
                        Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));     // Create a vector at the center of our camera's viewport
                        Debug.DrawRay(rayOrigin, fpsCam.transform.forward * weaponRange, Color.green);      // Draw a line in the Scene View  from the point rayOrigin in the direction of fpsCam.transform.forward * weaponRange, using the color green
                        RaycastHit hit;     // Declare a raycast hit to store information about what our raycast has hit

                        if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange, myLayerMask))     // Check if our raycast has hit anything
                        {
                            if (hit.transform.GetComponent<Enemy_Script>() != null)
                                hit.transform.GetComponent<Enemy_Script>().TakeDamage((int)damage * (int)damageMultiplier);
                            if (hit.rigidbody != null)      // Check if the object we hit has a rigidbody attached
                            {
                                hit.rigidbody.AddForce(-hit.normal * hitForce);     // Add force to the rigidbody we hit, in the direction from which it was hit
                            }
                        }
                        #endregion      // Handles raycast of weapon being shot
                    }
                    Player_UI_Controller_Script.instance.UpdateAmmoText(currentAmmo.ToString() + " / " + maximumAmmo.ToString());       // Updates reload text
                }
                if (currentAmmo < 1 && maximumAmmo > 1 && !bReloading)
                {
                    damageMultiplier = 4;
                    Player_UI_Controller_Script.instance.UpdateAmmoText("'R' to  reload");
                }
                else if (maximumAmmo < 1 && currentAmmo < 1)
                {
                    damageMultiplier = 4;
                    Player_UI_Controller_Script.instance.UpdateAmmoText("No ammo!");
                }
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                if (!bReloading && maximumAmmo + currentAmmo > 0)
                {
                    if (!Player_UI_Controller_Script.instance.ActiveReloadScrollbarState())
                    {
                        Player_UI_Controller_Script.instance.ActiveReloadScrollbarOn(true);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!bReloading)
                {
                    if (Player_UI_Controller_Script.instance.ActiveReloadScrollbarState())
                    {
                        damageMultiplier = Player_UI_Controller_Script.instance.ActiveReloadScrollbar().value;

                        if (damageMultiplier <= 0.2f)
                        {
                            damageMultiplier = 1;
                        }
                        else if (damageMultiplier > 0.2f && damageMultiplier <= 0.4f)
                        {
                            damageMultiplier = 2;
                        }
                        else if (damageMultiplier > 0.4f && damageMultiplier <= 0.6f)
                        {
                            damageMultiplier = 3;
                        }
                        else if (damageMultiplier > 0.6f && damageMultiplier <= 0.8f)
                        {
                            damageMultiplier = 2;
                        }
                        else if (damageMultiplier > 0.8f)
                        {
                            damageMultiplier = 1;
                        }

                        Player_UI_Controller_Script.instance.ActiveReloadScrollbarOn(false);
                        StartCoroutine("ReloadDelay");
                    }
                }
            }
            if (Player_UI_Controller_Script.instance.ActiveReloadScrollbarState())
                Player_UI_Controller_Script.instance.ActiveReloadScrollbar().value = Mathf.PingPong(reloadBarSpeed * Time.time, 1);
        }
    }

    protected int GetMaximumAmmo()
    {
        return (Weapons_Class.instance.MaxAmmo());
    }
    protected int GetMagazineCapacity()
    {
        return (Weapons_Class.instance.MagazineCapacity());
    }

    public Transform PlayerTransform()
    {
        return (transform);
    }

    public void TakeDamage(int dmg)
    {
        bShieldRecharge = false;
        if (!shieldRechargeCalled)
            StartCoroutine(ShieldRechargeDelay());
        if (shield > 0)
        {
            shield -= dmg;
            if (shield < 0)
                shield = 0;
            Player_UI_Controller_Script.instance.UpdateShieldScrollbar();
        }
        else
        {
            Player_UI_Controller_Script.instance.shieldBarHandle.enabled = false;
            health -= dmg;
            Player_UI_Controller_Script.instance.UpdateHealthScrollbar();
            if (health <= 0)
            {
                Player_UI_Controller_Script.instance.healthBarHandle.enabled = false;
                Player_UI_Controller_Script.instance.UpdateMessageText("GAME OVER");
                GetComponent<RigidbodyFirstPersonController>().enabled = false;
                GetComponent<Player_Script>().enabled = false;
            }
        }
    }

    IEnumerator ReloadDelay()
    {
        bReloading = true;
        Player_UI_Controller_Script.instance.UpdateAmmoText("Reloading...");

        yield return new WaitForSeconds(damageMultiplier);

        if (currentAmmo + maximumAmmo >= magazineCapacity)
        {
            maximumAmmo = maximumAmmo - (magazineCapacity - currentAmmo);
            currentAmmo = magazineCapacity;
        }
        else if (currentAmmo + maximumAmmo < magazineCapacity)
        {
            currentAmmo += maximumAmmo;
            maximumAmmo = 0;
        }
        Player_UI_Controller_Script.instance.UpdateAmmoText(currentAmmo.ToString() + " / " + maximumAmmo.ToString());
        bReloading = false;
    }
    private bool shieldRechargeCalled = false;
    IEnumerator ShieldRechargeDelay()
    {
        shieldRechargeCalled = true;
        yield return new WaitForSeconds(5);
        bShieldRecharge = true;
        shieldRechargeCalled = false;
    }
    void OnTriggerEnter(Collider other)
    {
        #region Get_PickUp
        if (other.gameObject.tag.Equals("Ammo"))
        {
            Debug.Log("Ammo picked up");

            maximumAmmo = GetMaximumAmmo();
            Player_UI_Controller_Script.instance.UpdateAmmoText(currentAmmo.ToString() + " / " + maximumAmmo.ToString());
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag.Equals("Health") && health < maxHealth)
        {
            Debug.Log("Ammo picked up");

            health = maxHealth;
            Player_UI_Controller_Script.instance.UpdateHealthScrollbar();
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag.Equals("Shield") && shield < maxShield)
        {
            shield = maxShield;
            Player_UI_Controller_Script.instance.UpdateShieldScrollbar();
            Destroy(other.gameObject);
        }
        #endregion
    }
}
