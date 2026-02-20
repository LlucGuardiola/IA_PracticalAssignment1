
using NUnit.Framework;
using UnityEngine;

public class Shark_BLACKBOARD : MonoBehaviour
{
    //[Header("Two point wandering")]
    [Header("SHARK")]
    public GameObject kelpZone;
    public GameObject PeekZone;

    public float KelpZoneReachedRadious = 2.0f;
    public float hideTime = 3.0f;
    public float peekTime = 2.0f;

    public float aproachRadius = 2.0f;
    public float chaseRadius = 2.0f;
    public float biteRadius = 2.0f;
    public float biteDuration = 2.0f;

    
    //[Header("Seed colecting")]

    void Start()
    {
       
    }
}
