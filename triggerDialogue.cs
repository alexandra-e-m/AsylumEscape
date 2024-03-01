using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class triggerDialogue : MonoBehaviour
{
    public GameObject Dialogue;
    public GameObject Catpickup;
    public Animator transition;
    public float transitionTime = 1f;
    itemPickUp cat;
    
    void Awake()
    {
        cat = Catpickup.GetComponent<itemPickUp>(); 
    }
    void OnTriggerEnter(Collider other)
    {   
        Debug.Log("Trigger entered!");
        
        if (other.CompareTag("Player"))
        {
            Dialogue.SetActive(true);
        }

        if(other.CompareTag("Player2") && cat.catpickedup)
        {
            Debug.Log("LastTrigger entered!");

            Dialogue.SetActive(false);
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
            FindObjectOfType<AudioManager>().Play("DoorOpening");
            FindObjectOfType<AudioManager>().Stop("Footsteps");
            FindObjectOfType<AudioManager>().Stop("Footstepsfast");
            
        }
    
    }

    IEnumerator LoadLevel(int levelIndex)
    {   
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
