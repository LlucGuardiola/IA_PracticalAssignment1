
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
    public float biteDuration = 2.0f;
    public float detectRadius = 2.0f;


    [Header("Resting FSM")]

    public float sleepTime = 4.0f;
    public float vomitTime = 7.0f;
    public float pooTime = 3.0f;
    public float breatheTime = 2.0f;

    public GameObject[] vomitElements;
    public GameObject poo;
    public GameObject sleepIcon;
    public GameObject pooZone;
    public float vomitTimeInterval = 1.0f;
    public GameObject vomitZone;
    public float vomitZoneReachedRadious = 2.0f;
    public float pooZoneReachedRadious = 2.0f;

    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 140);
    }
    private void Awake()
    {
        foreach (GameObject v in vomitElements)
        {
            v.SetActive(false);
        }

        sleepIcon.SetActive(false);
        poo.SetActive(false);
    }
    void Start()
    {
       
    }
}
