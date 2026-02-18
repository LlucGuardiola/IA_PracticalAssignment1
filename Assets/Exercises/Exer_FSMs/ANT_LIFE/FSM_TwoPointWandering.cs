using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_TwoPointWandering", menuName = "Finite State Machines/FSM_TwoPointWandering", order = 1)]
public class FSM_TwoPointWandering : FiniteStateMachine
{
    

    private WanderAround wanderAround;
    private SteeringContext steeringContext;
    private ANT_Blackboard blackboard;

    private float elapsedTime = 0;


    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is executed every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        /* COMPLETE */

        wanderAround = GetComponent<WanderAround>();

        steeringContext = GetComponent<SteeringContext>();

        blackboard = GetComponent<ANT_Blackboard>();

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        /* COMPLETE */

        base.DisableAllSteerings();

        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         */

        State goingA = new State("Going_A",
           () => { 
                    elapsedTime = 0;
                    wanderAround.attractor = blackboard.locationA;
                    wanderAround.enabled = true;
           },
           () => { elapsedTime += Time.deltaTime;}, 
           () => { wanderAround.enabled = false; }
       );

        State goingB = new State("Going_B",
           () => {
               elapsedTime = 0;
               wanderAround.attractor = blackboard.locationB;
               wanderAround.enabled = true;
           },
           () => { elapsedTime += Time.deltaTime; },
           () => { wanderAround.enabled = false; }
       );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------
        */


        Transition locationAReached = new Transition("LOCATION A REACHED",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.locationA) < blackboard.locationReachedRadius; }, // write the condition checkeing code in {}
            () => { steeringContext.seekWeight = blackboard.initialSeekWeight; }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition locationBReached = new Transition("LOCATION B REACHED",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.locationB) < blackboard.locationReachedRadius; },
            () => { steeringContext.seekWeight = blackboard.initialSeekWeight; }
        );

        Transition timeOut = new Transition("TIMEOUT",
            () => { return elapsedTime > blackboard.intervalBetweentimeOuts; },
            () => { steeringContext.seekWeight+= blackboard.seekIncrement; elapsedTime = 0.0f; }
        );
        
        /* COMPLETE, create the transitions */

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
         */

        AddStates(goingA, goingB);

                /* COMPLETE, add the transitions */
                AddTransition(goingA, locationAReached, goingB);
                AddTransition(goingB, locationBReached, goingA);
                AddTransition(goingA, timeOut, goingA);
                AddTransition(goingB, timeOut, goingB);

        /* STAGE 4: set the initial state */

        initialState = goingA;
    }
}
