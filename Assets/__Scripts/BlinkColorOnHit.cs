using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkColorOnHit : MonoBehaviour
{
    [Header("Dynamic")]
    public bool showingColor = false;
    public float blinkCompleteTime;
    public bool ignoreOnCollisionEnter = false;

    private Material[] materials;
    private Color[] originalColors;
    private BoundsCheck bndCheck;
    void Awake()
    {
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for(int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
        bndCheck = GetComponent<BoundsCheck>();
    }

    void onCollisionEnter(Collision coll)
    {
        if(ignoreOnCollisionEnter)
        {
            return;
        }
        if(bndCheck != null && bndCheck.offDown)
        {
            return;
        }
        SetColors();
    }

    public void SetColors()
    {
        showingColor = true;
        blinkCompleteTime = Time.time + 0.1f;
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i].color = Color.red;
        }
    }
}
