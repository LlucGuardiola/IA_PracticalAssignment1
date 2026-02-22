using FSMs;
using UnityEngine;
using Steerings;
using UnityEditor.SceneManagement;

[CreateAssetMenu(fileName = "FSM_FishChasing", menuName = "Finite State Machines/FSM_FishChasing", order = 1)]
public class FSM_FishChasing : FiniteStateMachine
{
    private Pursue pursue;
    private Shark_BLACKBOARD blackboard;
    private SteeringContext steeringContext;

    private float initSpeed;
    private float elapsedTime;

    public override void OnEnter()
    {
        steeringContext = GetComponent<SteeringContext>();
        pursue = GetComponent<Pursue>();
        blackboard = GetComponent<Shark_BLACKBOARD>();
        initSpeed = steeringContext.maxSpeed;

        base.OnEnter(); 
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         
        State varName = new State("StateName",
            () => { }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

         */

        State Approach = new State("APROACH",
            () => {
                steeringContext.maxSpeed = initSpeed / 2;
                pursue.enabled = true;
                pursue.target = null;
            },
            () => {
                GameObject fish = SensingUtils.FindInstanceWithinRadius(
                    gameObject, "RED_BOID", blackboard.aproachRadius
                );
    
                if (fish != null)
                {
                    pursue.target = fish;
                    pursue.enabled = true;
                }
            },
            () => { }
        );


        State Chase = new State("Chase",
            () => { steeringContext.maxSpeed = initSpeed; },
            () => { },
            () => { }
        );

        State Bite = new State("Bite",
            () => { elapsedTime = 0; pursue.target.GetComponent<SteeringContext>().maxSpeed = 0; },
            () => { elapsedTime = + Time.deltaTime; },
            () => { Destroy(pursue.target.gameObject); }
        );

        Transition goingToChase = new Transition("Going To Chase",
            () => pursue.target != null &&
                    SensingUtils.DistanceToTarget(gameObject, pursue.target) < blackboard.chaseRadius
         );


        Transition notChased = new Transition("Not Chased",
            () => { return SensingUtils.DistanceToTarget(gameObject, pursue.target) > blackboard.aproachRadius; }, 
            () => { }  
        );

        Transition chased = new Transition("Chased",
            () => { return SensingUtils.DistanceToTarget(gameObject, pursue.target) < blackboard.biteRadius; },
            () => { }
        );

        Transition bited = new Transition("Bited",
            () => { return elapsedTime > blackboard.biteDuration; },
            () => { elapsedTime = 0; }
        );


        AddStates(Approach, Chase, Bite);

        AddTransition(Approach, goingToChase, Chase);
        AddTransition(Chase, notChased, Approach);
        AddTransition (Chase, chased, Bite);
        AddTransition(Bite, bited, Approach);

        initialState = Approach;

    }
}
