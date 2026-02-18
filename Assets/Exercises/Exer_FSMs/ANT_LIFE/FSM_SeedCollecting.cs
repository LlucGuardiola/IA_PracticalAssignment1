using FSMs;
using UnityEngine;
using Steerings;
using System.Runtime.InteropServices.WindowsRuntime;

[CreateAssetMenu(fileName = "FSM_SeedCollecting", menuName = "Finite State Machines/FSM_SeedCollecting", order = 1)]
public class FSM_SeedCollecting : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private Arrive arrive;
    private ANT_Blackboard blackboard;
    private GameObject seed;
    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        arrive = GetComponent<Arrive>();
        blackboard = GetComponent<ANT_Blackboard>();
        seed = GameObject.FindGameObjectWithTag("SEED");
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */


        if (currentState != null && currentState.Name == "TRANSPORTING TO NEST")
        {
            seed.transform.parent = null; 
            seed.tag = "SEED";
        }
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
        FiniteStateMachine FSMTwoPointWandering = ScriptableObject.CreateInstance<FSM_TwoPointWandering>();
        FSMTwoPointWandering.name = "TwoPointsWander";

        State GOINGTOSEED = new State("GOINGTOSEED",
           () => { arrive.target = seed; arrive.enabled = true; }, // write on enter logic inside {}
           () => { }, // write in state logic inside {}
           () => { arrive.enabled = false; }  // write on exit logic inisde {}  
        );


        State TRANSPORTINGSEED = new State("TRANSPORTINGSEED",
          () => { seed.transform.SetParent(gameObject.transform); arrive.target = blackboard.nest; arrive.enabled = true; }, // write on enter logic inside {}
          () => { }, // write in state logic inside {}
          () => { arrive.enabled = false; seed.transform.SetParent(null); seed.tag = "NO_SEED"; }  // write on exit logic inisde {}  
        );
        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------
        */
        Transition nearbySeedDetected = new Transition("nearbySeedDetected",
            () => {  
                 seed = SensingUtils.FindInstanceWithinRadius(gameObject, "SEED", blackboard.seedDetectionRadius); 
              if( seed != null)
              {
                    seed.tag = "OTHER_SEED";
                    return true;
              }else return false;
            }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition seedReached = new Transition("seedReached",
            ()=> { return SensingUtils.DistanceToTarget(gameObject, seed) < blackboard.seedReachedRadius; }
        );
        Transition nestReached = new Transition("nestReached",
          () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.nest) < blackboard.nestReachedRadius; }

        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(FSMTwoPointWandering,GOINGTOSEED,TRANSPORTINGSEED);
        AddTransition(FSMTwoPointWandering,nearbySeedDetected,GOINGTOSEED);
        AddTransition(GOINGTOSEED, seedReached, TRANSPORTINGSEED);
        AddTransition(TRANSPORTINGSEED,nestReached,FSMTwoPointWandering);

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */

        initialState = FSMTwoPointWandering;

    }
}
