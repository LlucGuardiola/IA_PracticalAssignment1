using FSMs;
using UnityEngine;
using Steerings;
using System.Runtime.InteropServices.WindowsRuntime;

[CreateAssetMenu(fileName = "FSM_Ghost", menuName = "Finite State Machines/FSM_Ghost", order = 1)]
public class FSM_Ghost : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private Arrive arrive;
    private GHOST_Blackboard blackboard;
    private SteeringContext stearingContext;
    private Pursue pursue;
    private float elapsedTime;
    private GameObject victim;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        arrive = GetComponent<Arrive>();
        blackboard = GetComponent<GHOST_Blackboard>();
        stearingContext = GetComponent<SteeringContext>();
        pursue = GetComponent<Pursue>();
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         */
        State GOCASTLLE = new State("GOCASTLLE",
            () => { arrive.target = blackboard.castle; arrive.enabled = true; stearingContext.maxSpeed *= 4; }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { stearingContext.maxSpeed /= 4; arrive.enabled = false; }  // write on exit logic inisde {}  
        );

        State HIDE = new State("HIDE",

            () => { elapsedTime = 0; },
            () => { elapsedTime += Time.deltaTime; },
            () => { }
        );

        State SELECTTARGET = new State("SELECTTARGET",

            () => { },
            () => { },
            () => { }
        );

        State APROACH = new State("APROACH",

          () => { pursue.target = victim; pursue.enabled = true; },
          () => { },
          () => { pursue.enabled = false; }
        );


        State CRYBOO = new State("CRYBOO",

          () => { blackboard.CryBoo(true); },
          () => { },
          () => { blackboard.CryBoo(false); }
        );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------
        */
        Transition CastleReached = new Transition("CastleReached",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.castle) < blackboard.castleReachedRadius; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );


        Transition TimeOut = new Transition("TimeOout",
            () => { return elapsedTime >= blackboard.hideTime; },
            () => { }
        );

        Transition TargetSelected = new Transition("TargetSelected",
            () => {victim = SensingUtils.FindRandomInstanceWithinRadius(gameObject, victim.tag, blackboard.nerdDetectionRadius); return victim != null; },
            () => { }
        );

        Transition TargetIsClose = new Transition("TargetIsClose",
            ()=> { return SensingUtils.DistanceToTarget(gameObject, victim) <= blackboard.closeEnoughToScare; },
            () => { }
        );

        Transition TimeOutCRYBOO = new Transition("TimeOutCRYBOO",
            () => { return elapsedTime >= blackboard.booDuration; }, () => { });





        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */


        /* STAGE 4: set the initial state
         
        initialState = ... 

         */

    }
}
