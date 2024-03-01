using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyStateMachine : MonoBehaviour
{ private enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }

    private bool ShouldSwitchToChase => Vector3.Distance(transform.position, player.position) < ChaseDistance;
    private bool ShouldSwitchToAttack => Vector3.Distance(transform.position, player.position) < AttackDistance;
    private bool IsSafe => Vector3.Distance(transform.position, player.position) > safeDistance;
    private bool Flash => Vector3.Distance(transform.position, player.position) <= safeDistance;
    
    
    List<Transform> wayPoints = new List<Transform>();

    private EnemyState currentState;
    private NavMeshAgent navMeshAgent;
    private AudioManager audioManager;
    private Animator animator;
    public Transform player; 
    public Transform player2;
    public Transform Camera;
    public Transform Camera2;
    private bool isChaseSoundPlaying;
    //private AudioSource audioSource;

    [Header("Functional Options")]
    
    [SerializeField] private float ChaseDistance = 6.5f;
    [SerializeField] private float AttackDistance = 1.8f;
    [SerializeField] private float safeDistance = 10f;
    [SerializeField] private float chaseSpeed = 2.5f;
    [SerializeField] private float patrolSpeed = 0.7f;
    [SerializeField] private int damage = 25;

    private float attackTimer = 0f;
    private bool isScreenShaking = false;
    private bool hasAttacked = false;
    private float timeSinceLastWaypointSwitch = 0f;
    private float timer = 0f;
    float minT = 0.3f;
    float maxT =2f;
    float minint = 0.5f;
    float maxint = 2f;
    public GameObject Flight;
    public GameObject FlightII;
    public GameObject Catpickup;
    Light Flight2;
    itemPickUp cat;


     

    void Start()
    {   
        //audioSource = GetComponent<AudioSource>();
        currentState = EnemyState.Idle;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //player = GameObject.Find("FisrtPerson").transform;
        //Camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        //Flight = GameObject.FindGameObjectWithTag("Flashlight");
        Flight2 = Flight.GetComponent<Light>();
        audioManager = FindObjectOfType<AudioManager>();
        cat = Catpickup.GetComponent<itemPickUp>(); 
    
        
    
        if (audioManager == null) 
        {
            Debug.LogError("AudioManager not found in the scene.");
        }
        

        
    }

    void Update()
    {
        if(ShouldSwitchToChase){StartCoroutine(Flashing());}

        if(cat.catpickedup)
        {
            player = player2;
            Camera = Camera2;
            Flight = FlightII;
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                
                SetAnimation("Idle");

                timer += Time.deltaTime;
                if (timer > 10f )
                {
                    timer = 0f;
                    SwitchToPatrol();
                    
                }
                else if (ShouldSwitchToChase) SwitchToChase();
                break;

            case EnemyState.Patrol:
                //PlaySound(idleSound); // You can use a different sound for patrol if needed
                SetAnimation("Walk");
                if (ShouldSwitchToChase) SwitchToChase();

                timer += Time.deltaTime;
                if (timer > 30f )
                {
                    timer = 0f;
                    SwitchToIdle();
                    
                }
                
                break;

            case EnemyState.Chase:
                timer = 0f;
                FacePlayer();
                SetAnimation("Run");
                navMeshAgent.SetDestination(player.position);
                if (ShouldSwitchToAttack) SwitchToAttack();

                else if(!ShouldSwitchToChase) SwitchToPatrol();

                break;

            case EnemyState.Attack:
                if (audioManager != null)
                {
                    //audioManager.Play("Chase");
                }
                FacePlayer();
                 if (!hasAttacked)
                {
                    SetAnimation("Attack");
                    
                    hasAttacked = true;
                    
                }

                attackTimer += Time.deltaTime;

                if (attackTimer >= 2f)
                {
                    // Reset the timer and allow another attack
                    StartCoroutine(ScreenShake());
                    attackTimer = 0f;
                    hasAttacked = false;

                    // Trigger damage to the player (add your logic here)
                    
                    player.GetComponent<FirstPersonController>().TakeDamage(damage);
                    
                }
                
                
            
                if(!ShouldSwitchToAttack) SwitchToChase();

                break;
        }
    }

    void SwitchToIdle()
    {
        currentState = EnemyState.Idle;
        if (audioManager != null && IsSafe)
        {
            audioManager.Stop("Chase");
            isChaseSoundPlaying = false;
        }
    }

    void SwitchToPatrol()
    {
        //navMeshAgent.isStopped = false;
        currentState = EnemyState.Patrol;
        timeSinceLastWaypointSwitch = 0f;
        if (audioManager != null && !IsSafe && isChaseSoundPlaying)
        {
            StartCoroutine(FadeOutChaseSound());
        }

        SetNextWaypoint();
        
    }


    void SwitchToChase()
    {   
        //navMeshAgent.isStopped = false;
        currentState = EnemyState.Chase;
        //navMeshAgent.SetDestination(player.position);
        navMeshAgent.speed = chaseSpeed;
        Debug.Log("Chase Destination: " + navMeshAgent.destination);
        if (audioManager != null && !isChaseSoundPlaying)
        {
            audioManager.Play("Chase");
            audioManager.Play("Scream");
            isChaseSoundPlaying = true;
        }
    }


    void SwitchToAttack()
    {
        currentState = EnemyState.Attack;
        
       
        //navMeshAgent.isStopped = true; // Stop moving when attacking
    }


    void SetNextWaypoint()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Waypoints");
        foreach(Transform t in go.transform)
        wayPoints.Add(t);
        navMeshAgent.SetDestination(wayPoints[Random.Range(0,wayPoints.Count)].position);
        navMeshAgent.speed = patrolSpeed;
    }

    void FacePlayer()
    {
    // Calculate the direction to the player
    Vector3 directionToPlayer = (player.position - transform.position).normalized;

    // Ignore the y-axis to avoid tilting
    directionToPlayer.y = 0f;

    // Rotate the enemy to face the player
    transform.rotation = Quaternion.LookRotation(directionToPlayer);
    }

 



    void SetAnimation(string animationState)
    {
        animator.Play(animationState);
    }

    IEnumerator FadeOutChaseSound()
    {
    // Gradually decrease the volume over a specific duration
        float duration = 2.0f; // Adjust the duration as needed
        float currentTime = 0f;

        while (currentTime < duration)
        {
            float volume = Mathf.Lerp(0.6f, 0f, currentTime / duration);
            audioManager.SetVolume("Chase", volume);
            currentTime += Time.deltaTime;
            yield return null;
        }

    // Stop the sound and reset the volume to avoid potential issues
        audioManager.Stop("Chase");
        audioManager.SetVolume("Chase", 0.6f);

        isChaseSoundPlaying = false;
    }

    IEnumerator Flashing ()
    {
        while(Flash) //while its true 
        {
            yield return new WaitForSeconds(Random.Range(minT, maxT));
            Flight2.intensity = Random.Range(minint, maxint);

        }
    }

    IEnumerator ScreenShake()
    {
        if (!isScreenShaking)
        {
            isScreenShaking = true;
            
            Vector3 originalPosition = Camera.localPosition;
            float shakeDuration = 0.5f;
            float shakeMagnitude = 0.1f;

            float elapsed = 0f;

            while (elapsed < shakeDuration)
            {
                Vector3 newPosition = originalPosition + Random.insideUnitSphere * shakeMagnitude;
                Camera.localPosition = newPosition;

                elapsed += Time.deltaTime;

                yield return null;
            }

            Camera.localPosition = originalPosition;
            isScreenShaking = false;
        }
    }
    
}

  
