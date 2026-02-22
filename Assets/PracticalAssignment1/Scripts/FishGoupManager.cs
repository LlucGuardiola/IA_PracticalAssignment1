
using UnityEngine;

public class FishGroupManager : Steerings.GroupManager
{
    public int numInstances = 20;
    public float delay = 0.5f;
    public bool around = false;
    public GameObject attractor;

    private int created = 0;
    private float elapsedTime = 0f;
    public Camera cam;
    public GameObject FishPrefab;
    public GameObject SharkPrefab;

    // the following attributes are specifically created to help listeners of UI
    // components get the initial values for the UI elements they're attached to
    [HideInInspector]
    public float maxSpeed, maxAcceleration, cohesionThreshold, repulsionThreshold, coneOfVisionAngle,
    cohesionWeight, repulsionWeight, alignmentWeight, seekWeight;

    void Start()
    {
        GameObject dummy = Instantiate(FishPrefab);
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
        cam = Camera.main;  
    }

    // Update is called once per frame
    void Update()
    {
        Spawn();
    }


    private void Spawn()
    {
        if (Input.GetMouseButtonDown(1)) // click dret
        {
            GameObject clone = Instantiate(FishPrefab);
            SharkPrefab.GetComponent<Shark_BLACKBOARD>().fishesOnScene++;
            var position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            clone.transform.Rotate(0, 0, Random.value * 360);
            clone.transform.position = position;
            Debug.Log(around);

            if (around)
            {
                clone.AddComponent<Steerings.FlockingAroundPlusAvoidance>();
                clone.GetComponent<Steerings.FlockingAroundPlusAvoidance>().attractor = attractor;
                clone.GetComponent<Steerings.FlockingAroundPlusAvoidance>().rotationalPolicy = Steerings.SteeringBehaviour.RotationalPolicy.LWYGI;

                clone.AddComponent<Steerings.EvadePlusOA>();
                //clone.GetComponent<Steerings.FleePlusOA>().target = SharkPrefab;
                clone.GetComponent<Steerings.EvadePlusOA>().rotationalPolicy = Steerings.SteeringBehaviour.RotationalPolicy.LWYGI;
                clone.GetComponent<Steerings.EvadePlusOA>().enabled = false;
            }
            else
            {
                clone.AddComponent<Steerings.Flocking>();
                clone.GetComponent<Steerings.Flocking>().rotationalPolicy = Steerings.SteeringBehaviour.RotationalPolicy.LWYGI;
            }



            if (created == 0)
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
            }

            AddBoid(clone);
            created++;
            elapsedTime = 0f;
        }
    }
}
