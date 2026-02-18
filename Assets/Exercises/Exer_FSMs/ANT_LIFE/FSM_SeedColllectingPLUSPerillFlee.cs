using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_", menuName = "Finite State Machines/FSM_", order = 1)]
public class FSM_ : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private ANT_Blackboard blackboard;
    private Flee flee;
    private GameObject thePredator;


    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<ANT_Blackboard>();
        flee = GetComponent<Flee>();
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
         
        State varName = new State("StateName",
            () => { }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

         */
        FiniteStateMachine SeedCollecting = ScriptableObject.CreateInstance<FSM_SeedCollecting>();
        SeedCollecting.name = "SeedCollecting";

        State FreeFromPerill = new State("FreeFromPerill",
            () => { flee.target = thePredator; flee.enabled = true; }, 
            () => { }, 
            () => { flee.enabled = false; }  
        );
        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        Transition PredatorNearby = new Transition("PredatorNearby",
            () => {thePredator = SensingUtils.FindInstanceWithinRadius(gameObject, "PREDATOR", blackboard.predatorRadiousDistance); return thePredator != null;}
        );

        Transition PredatorFarAWay = new Transition("PredatorNearby",
            () => { return thePredator = SensingUtils.FindInstanceWithinRadius(gameObject, "PREDATOR", blackboard.predatorFarAwayDistance);}
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(SeedCollecting, FreeFromPerill);
        AddTransition(SeedCollecting, PredatorNearby, FreeFromPerill);
        AddTransition(FreeFromPerill, PredatorFarAWay, SeedCollecting);

        /* STAGE 4: set the initial state
         
        initialState = ... 



         */
        initialState = SeedCollecting;
    }
}
