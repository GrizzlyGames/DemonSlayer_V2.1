using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player_UI_Controller_Script : MonoBehaviour
{
    public static Player_UI_Controller_Script instance;          // Ensures there is only 1 instance of this object

    public Text creditsText;
    public Text AmmoText;                                  // Assign AmmoText from scene
    public Text messageText;

    public Image shieldBarHandle;
    public Image healthBarHandle;

    public Scrollbar shieldScrollbar;
    public Scrollbar healthScrollbar;
    public Scrollbar activeReloadScrollbar;

    public GameObject activeReloadGO;

    void Awake()                                            // First function to run in scene
    {
        instance = this;                                    // This object is the only instance of Game_Controller_Script
    }

    void Start()                                            // Sceond function to run in scene
    {
        activeReloadScrollbar = activeReloadGO.GetComponent<Scrollbar>();
        
        if (AmmoText == null)                               // Check if the AmmoText has been assigned already
            AmmoText = GameObject.Find("Ammo_Text").GetComponent<Text>();       // If the AmmoText hasn't been assigned assign it

        if (healthScrollbar == null)
            healthScrollbar = GameObject.Find("Health_Scrollbar").GetComponent<Scrollbar>();     
    }

    public void UpdatePointsText()
    {
        creditsText.text = Player_Script.instance.pts.ToString("N0") + "cr";
    }
    public void UpdateAmmoText(string message)
    {
        if (Player_Script.instance.damageMultiplier == 1)
        {
            AmmoText.color = Color.cyan;
        }
        else if (Player_Script.instance.damageMultiplier == 1)
        {
            AmmoText.color = Color.magenta;
        }
        else if (Player_Script.instance.damageMultiplier == 3)
        {
            AmmoText.color = Color.red;
        }
        else
        {
            AmmoText.color = Color.white;
        }
        AmmoText.text = message;
    }
    public void UpdateHealthScrollbar()
    {
        healthScrollbar.size = (float)Player_Script.instance.health / (float)Player_Script.instance.maxHealth;
    }

    public void UpdateShieldScrollbar()
    {
        shieldScrollbar.size = (float)Player_Script.instance.shield / (float)Player_Script.instance.maxShield;
    }
    public void UpdateMessageText(string message)
    {
        messageText.text = message;
    }
    public void ActiveReloadScrollbarOn(bool on)
    {
        if (on)
        {
            activeReloadGO.SetActive(true);
            AmmoText.text = "";
        }
        else
        {
            activeReloadGO.SetActive(false);
            UpdateAmmoText(Player_Script.instance.currentAmmo.ToString() + " / " + Player_Script.instance.maximumAmmo.ToString());
        }
    }
    public bool ActiveReloadScrollbarState()
    {
        if (activeReloadGO.activeInHierarchy)
        {
            return (true);
        }
        else
        {
            return (false);
        }
    }
    public Scrollbar ActiveReloadScrollbar()
    {
        return (activeReloadScrollbar);
    }
}
