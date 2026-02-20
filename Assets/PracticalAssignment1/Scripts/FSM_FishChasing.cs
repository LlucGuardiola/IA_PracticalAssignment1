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

        State APROACH = new State("Aproach",
            () => { pursue.enabled = true;
                    pursue.target = SensingUtils.FindInstanceWithinRadius(gameObject, "RED_BOID", blackboard.aproachRadius);
                steeringContext.maxSpeed = initSpeed / 2; 
            },
            () => { },
            () => { }
        );

        State CHASE = new State("Chase",
            () => { steeringContext.maxSpeed = initSpeed; },
            () => { },
            () => { }
        );

        State BITE = new State("Bite",
            () => { elapsedTime = 0; pursue.target.GetComponent<SteeringContext>().maxSpeed = 0; },
            () => { elapsedTime = + Time.deltaTime; },
            () => { Destroy(pursue.target.gameObject); }
        );



        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */

        Transition goingToChase = new Transition("Going To Chase",
            () => { return SensingUtils.DistanceToTarget(gameObject, pursue.target) < blackboard.chaseRadius; }, 
            () => { } 
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

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */

        AddStates(APROACH, CHASE, BITE);

        AddTransition(APROACH, goingToChase, CHASE);
        AddTransition(CHASE, notChased, APROACH);
        AddTransition (CHASE, chased, BITE);
        AddTransition(BITE, bited, APROACH);


        /* STAGE 4: set the initial state
         
        initialState = ... 

         */

        initialState = APROACH;

    }
}
