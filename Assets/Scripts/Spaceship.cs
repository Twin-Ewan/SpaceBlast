using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Spaceship : MonoBehaviour
{

    [SerializeField][Range(1, 10)] protected int maxHealth;
    public int maxAmmo {get; set; }
    public int health { get; set; }
    public int ammo { get; set; }
    protected int direction = 1;

    public enum shipType { Player, Basic, Glass, Burst, Strong }
    public shipType shipClass { get; set; }

    public float fireRate { get; set; } = 2;
    protected float reloadTime = 1;

    int scoreOnDeath = 500;
    int damagePerShot = 1;

    bool reloading = false;
    float nextTimetoFire = 0;

    [SerializeField] protected GameObject laser;
    [SerializeField] protected GameObject explosionPrefab;
    protected GameObject healthBar;

    [Header("Audio")]
    float audioVolume;
    protected AudioSource soundSource;
    [SerializeField] protected AudioClip audioFire;
    [SerializeField] protected AudioClip audioDeath;
    [SerializeField] protected AudioClip audioHit;


    GameManager GM;
    void Start()
    {
        healthBar = transform.GetChild(1).gameObject;
        soundSource = GetComponent<AudioSource>();
        audioVolume = PlayerPrefs.GetFloat("Volume");
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();

        Material bodyColour = transform.GetChild(0).GetChild(1).GetComponent<Renderer>().material;

        if (shipClass == shipType.Player)
        {
            maxAmmo = 10; maxHealth = 10; fireRate = 3; scoreOnDeath = 0;
            bodyColour.SetColor("_Color", new Color (.5213f, .1383f, .3404f)); //#310D20 "Dark Purple"
        }

        if (shipClass == shipType.Basic)
        {   maxAmmo = 3; maxHealth = 2;
            bodyColour.SetColor("_Color", Color.red); //#FF0000 "Red"
        }

        if (shipClass == shipType.Glass)
        {
            maxAmmo = 2; maxHealth = 1; fireRate = 1; reloadTime = 2; damagePerShot = 2; scoreOnDeath = 500;
            bodyColour.SetColor("_Color", new Color(.729f, .3736f, .5535f)); //#20A4F3 "Light Blue"
        }

        if (shipClass == shipType.Burst)
        {   maxAmmo = 5; maxHealth = 2; fireRate = 5; reloadTime = 2 ; scoreOnDeath = 750;
            bodyColour.SetColor("_Color", new Color(.5263f, .3347f, .1389f)); //#FA9F42 "Mustard"
        }

        if (shipClass == shipType.Strong)
        {
            maxAmmo = 2; maxHealth = 3; fireRate = 2; reloadTime = 2; damagePerShot = 2; scoreOnDeath = 1000;
            bodyColour.SetColor("_Color", new Color(.48f, .3533f, .1667f)); //#483519 "Dark Green"
        }

        ammo = maxAmmo;
        health = maxHealth;
    }

    protected void Shoot()
    {
        if(Time.time <= nextTimetoFire || reloading) return;

        if (ammo <= 0) StartCoroutine(Reload());
        else
        {
            ammo--;
            soundSource.PlayOneShot(audioFire, audioVolume);
            nextTimetoFire = Time.time + 1f / fireRate;

            GameObject LaserObject = Instantiate(laser, transform.position, Quaternion.identity, transform.parent);
            LaserObject.GetComponent<Hazard>().damage = damagePerShot;

            Rigidbody LaserRB = LaserObject.GetComponent<Rigidbody>();
            LaserRB.velocity = new Vector2(20 * direction, 0);
            LaserRB.AddRelativeTorque(2, 0, 0, ForceMode.Impulse);
            LaserRB.angularDrag = 0;

            Destroy(LaserObject, 2);
        }
    }

    protected IEnumerator Reload()
    {
        if (reloading) yield break;
        reloading = true;

        int missingAmmo = maxAmmo - ammo;
        for (int i = 0; i < missingAmmo; i++)
        {
            // reloadTime is how long it takes to reload in seconds but i divided it by maxAmmo so it'll 
            // increases by 1, maxAmmo times (20) over a second to make this nice animation of it ticking up
            yield return new WaitForSeconds(reloadTime / (float)maxAmmo);
            ammo++;
        }
        yield return new WaitForSeconds(.02f);
        reloading = false;
    }

    void Die()
    {
        if (shipClass != shipType.Player)
        {
            GM.UpdateKillCount((int)shipClass);
            GM.UpdateKillCount();
        }
        GM.score += scoreOnDeath;

        GameObject GOExplode = Instantiate(explosionPrefab, this.transform.position, Quaternion.identity);
        GOExplode.AddComponent<AudioSource>();
        GOExplode.GetComponent<AudioSource>().PlayOneShot(audioDeath, audioVolume);

        Destroy(GOExplode, 1f);
        Destroy(this.gameObject);
    }

    void CreatePopup(int damage)
    {
        var popupObj = new GameObject(this.name + " TextPopup");
        Vector3 pos = this.transform.position;
        pos.z = -1;
        popupObj.transform.position = pos;
        popupObj.transform.localScale = new Vector3(0.2f, 0.2f, 1);

        popupObj.AddComponent<TextMesh>();
        popupObj.AddComponent<Rigidbody2D>();

        var popupText = popupObj.GetComponent<TextMesh>();

        popupText.text = (damage * 10).ToString();
        popupText.anchor = TextAnchor.MiddleCenter;
        popupText.fontStyle = FontStyle.Bold;

        popupObj.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 3);
        Destroy(popupObj, 2);
    }

    IEnumerator DisplayHealthBar()
    {
        healthBar.transform.GetChild(2).GetComponent<Image>().fillAmount = (float)health / maxHealth;

        healthBar.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        healthBar.SetActive(false);
    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.GetComponent<Hazard>() != null)
        {
            health -= collision.GetComponent<Hazard>().damage;
            StartCoroutine(DisplayHealthBar());

            CreatePopup(collision.GetComponent<Hazard>().damage);
            soundSource.PlayOneShot(audioHit, audioVolume);

            if (health <= 0) Die();
        }
    }
}
