using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Trigger : MonoBehaviour
{   
    [SerializeField] string tagFilter;
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;
    
    public float transitionTime = 1f;
    public Animator transition;
    public GameObject TreadPaper;
    TReadPaper treadpaper;

    void Awake()
    {
    
        treadpaper = TreadPaper.GetComponent<TReadPaper>();
        
    }    
    
    void OnTriggerEnter(Collider other)
    {   
        if(!string.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter) || !treadpaper.read ) return;
        
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        FindObjectOfType<AudioManager>().Play("DoorOpening");
        FindObjectOfType<AudioManager>().Stop("Footsteps");
        FindObjectOfType<AudioManager>().Stop("Footstepsfast");
        
        onTriggerEnter.Invoke();
        
    }

    IEnumerator LoadLevel(int levelIndex)
    {   
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }


    void OnTriggerExit(Collider other)
    {   
        if(!string.IsNullOrEmpty(tagFilter) && !other.gameObject.CompareTag(tagFilter)) return;
        onTriggerExit.Invoke();
    }
}
