
using NUnit.Framework;
using UnityEngine;

public class Shark_BLACKBOARD : MonoBehaviour
{
    [Header("Hiding FSM")]
    public GameObject kelpZone;
    public GameObject PeekZone;

    public float KelpZoneReachedRadious = 2.0f;
    public float hideTime = 3.0f;
    public float peekTime = 2.0f;

    [Header("Fish Chasing FSM")]
    public float aproachRadius = 200f;
    public float chaseRadius = 20.0f;
    public float biteRadius = 2.0f;
    public float biteDuration = 0.1f;
    public float detectRadius = 2.0f;


    [Header("Resting FSM")]

    public float vomitTime = 3f;
    public float vomitTimeInterval = 1.0f;
    public float sleepTime = 4.0f;
    public float pooTime = 3.0f;
    public float breatheTime = 2.0f;

    public int elementsToVomit = 3;
    public GameObject poo;
    public GameObject sleepIcon;
    public GameObject pooZone;
    public GameObject vomitZone;
    public GameObject fishbonePrefab;
    public GameObject mouth;
    public float vomitZoneReachedRadious = 2.0f;
    public float pooZoneReachedRadious = 2.0f;

    public int totalFishesEaten = 0;   

    




    [Header("SharkBehaviour")]
    public int fishesOnScene;
    public int fishesToChase = 30;
    public GameObject currentFish;

    public int totalFishToBeFed = 20;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aproachRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, biteRadius);
    }
    private void Awake()
    {
        sleepIcon.SetActive(false);
        poo.SetActive(false);
        fishesOnScene = 0;
    }
    void Start()
    {
       
    }
}
