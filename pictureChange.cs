using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pictureChange : MonoBehaviour
{
    public Material changedMaterial;
    bool alreadychanged = false;
    public GameObject TPainting;
    private AudioManager audioManager;
    TriggerPainting triggerP;
    // Start is called before the first frame update
    void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found in the scene.");
        }

        triggerP = TPainting.GetComponent<TriggerPainting>();
        
    }    
    void Update()
    {
        if(triggerP.change && !alreadychanged) ChangePaintingMaterial();
    }

    public void ChangePaintingMaterial()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && changedMaterial != null)
        {
            audioManager.Play("Piano"); 
            renderer.material = changedMaterial;
            alreadychanged = true;
            
        }
    }
}
