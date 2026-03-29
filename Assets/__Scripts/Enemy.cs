using System.Collections;  //Required for Arrays & other Collections
using System.Collections.Generic;  //Required for Lists and Dictionaries
using UnityEngine;  //Required for Unity

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector")]
    public float speed = 10f;  //The speed in m/s
    public float fireRate = 0.3f;  //Seconds/shot (Unused)
    public float health = 10;
    public int score = 100;  //Points earned for destroying this
    public float showDamageDuration = 0.1f; // # seconds to show damage
    public float powerUpDropChance = 1f; //Chance to drop a powerup

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials; //All the Materials of this and its children
    public bool showingDamage = false;
    public float damageDoneTime; //Time to stop showing damage
    public bool notifiedOfDestruction = false; //Will be used later

    [Header ("Enemy shooting"  )]
    public GameObject projectilePrefab;
    public float shootDelay = 5f;
    private float lastShotTime;

    protected BoundsCheck bndCheck;

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        //Get materials and colors for this GameObject and its children
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i=0; i<materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    // This is a Property: A method that acts like a field
    public Vector3 pos
    {
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }
    void Update()
    {
        Move();

        if (showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

        if (bndCheck != null && bndCheck.offDown)
        {
            //We're off the bottom, so destroy this GameObject
            Destroy(gameObject);
        }
        TrytoShoot();
    }
    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }
    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                //If this enemy is off screen dont damage it
                if(!bndCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }
                //Hurt this enemy
                ShowDamage();
                //Get the damage amount from the Main WEAP_DICT
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if(health <= 0)
                {
                    //Tell the main singelton that this ship was destroyed
                    if (!notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    //Destroy this Enemy
                    Destroy(this.gameObject);
                }
                Destroy(otherGO);
                break;
            default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name);
                break;
        }
    }
    void ShowDamage()
    {
        foreach (Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }
    void UnShowDamage()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }
    void TrytoShoot()
    {
        if (projectilePrefab == null) return;

        if (Time.time - lastShotTime < shootDelay) return;

        lastShotTime = Time.time;

        GameObject proj = Instantiate(projectilePrefab);

        proj.transform.position = transform.position + Vector3.down * 1.0f;

        Projectile p = proj.GetComponent<Projectile>();
        if (p != null && p.rigid != null)
        {
            p.type = WeaponType.blaster; //Projectile will get the color and damage from this
            p.rigid.velocity = Vector3.down * 50; //Move down at 50 m/s

        }
    }
}
