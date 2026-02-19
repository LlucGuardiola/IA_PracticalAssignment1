
using UnityEngine;

public class FishGroupManager : Steerings.GroupManager
{
    //public int numInstances = 20;
    //public float delay = 0.5f;
    private Camera cam;
    public GameObject fishPrefab;
    public bool around = false;
    public GameObject attractor;


    private int created = 0;
    //private float elapsedTime = 0f;

    // the following attributes are specifically created to help listeners of UI
    // components get the initial values for the UI elements they're attached to
    [HideInInspector]
    public float maxSpeed, maxAcceleration, cohesionThreshold, repulsionThreshold, coneOfVisionAngle,
    cohesionWeight, repulsionWeight, alignmentWeight, seekWeight;

    void Start()
    {
        cam = Camera.main;
        GameObject dummy = Instantiate(fishPrefab);
        Steerings.SteeringContext context = dummy.GetComponent<Steerings.SteeringContext>();
        maxSpeed = context.maxSpeed;
        maxAcceleration = context.maxAcceleration;
        cohesionThreshold = context.cohesionThreshold;
        repulsionThreshold = context.repulsionThreshold;
        coneOfVisionAngle = context.coneOfVisionAngle;
        cohesionWeight = context.cohesionWeight;
        repulsionWeight = context.repulsionWeight;
        alignmentWeight = context.alignmentWeight;
        seekWeight = context.seekWeight;
        Destroy(dummy);
    }

    // Update is called once per frame
    void Update()
    {
        CheckSpawn();
    }


    private void CheckSpawn()
    {
        //if (created == numInstances) return;

        //if (elapsedTime < delay)
        //{
        //    elapsedTime += Time.deltaTime;
        //    return;
        //}

        if (Input.GetMouseButtonDown(1)) // click dret
        {
            GameObject clone = Instantiate(fishPrefab);
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            clone.transform.Rotate(0, 0, Random.value * 360);
            clone.transform.position = position;
            Debug.Log(around);
            if (around)
            {
                clone.AddComponent<Steerings.FlockingAround>();
                clone.GetComponent<Steerings.FlockingAround>().attractor = attractor;
                clone.GetComponent<Steerings.FlockingAround>().rotationalPolicy = Steerings.SteeringBehaviour.RotationalPolicy.LWYGI;
            }
            else
            {
                clone.AddComponent<Steerings.Flocking>();
                clone.GetComponent<Steerings.Flocking>().rotationalPolicy = Steerings.SteeringBehaviour.RotationalPolicy.LWYGI;
            }



            if (created == 1)
            {
                // first one and only it
                ShowRadiiPro shr = clone.GetComponent<ShowRadiiPro>();
                shr.componentTypeName = "Steerings.SteeringContext";
                shr.innerFieldName = "repulsionThreshold";
                shr.outerFieldName = "cohesionThreshold";
                shr.enabled = true;

                if (around)
                {
                    if (clone.GetComponent<TrailRenderer>() != null)
                    {
                        clone.AddComponent<ToggleTrail>();
                        clone.GetComponent<TrailRenderer>().enabled = true;
                    }
                }
                created++;
            }

            AddBoid(clone);
            //elapsedTime = 0f;
        }

    }
}
