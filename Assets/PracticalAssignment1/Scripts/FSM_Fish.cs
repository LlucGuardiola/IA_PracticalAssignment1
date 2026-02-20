using FSMs;
using UnityEngine;
using Steerings;
using Unity.Hierarchy;

[CreateAssetMenu(fileName = "Fish_FSM", menuName = "Finite State Machines/Fish_FSM", order = 1)]
public class FSM_Fish : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private Fish_BLACKBOARD blackboard;
    private Flee flee;
    private FlockingAround flockingAround;
    private GameObject shark;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<Fish_BLACKBOARD>();
        flee = GetComponent< Flee>();
        flockingAround = GetComponent<FlockingAroundPlusAvoidance>();   
        
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

        State FlockingAround = new State("FlockingAround",
           () => { flockingAround.enabled = true; }, // write on enter logic inside {}
           () => { Debug.Log("Flocking"); }, // write in state logic inside {}
           () => { flockingAround.enabled = false; }  // write on exit logic inisde {}  
       );

        State EvadeShark = new State("EvadeShark",
           () => { flee.enabled = true;  flee.target = shark; } ,// write on enter logic inside {}
           () => { Debug.Log("Evade"); }, // write in state logic inside {}
           () => { flee.enabled = false;  }  // write on exit logic inisde {}  
       );
        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        Transition SharkNear= new Transition("SharkNear",
            () => { shark = SensingUtils.FindInstanceWithinRadius(gameObject,"HEN",blackboard.sharkNearRadious); return shark != null;  }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition SharkFarAway = new Transition("SharkFarAway",
           () => { return SensingUtils.DistanceToTarget(gameObject, shark) >=blackboard.sharkNearRadious;  }, // write the condition checkeing code in {}
           () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
       );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(FlockingAround, EvadeShark);

        AddTransition(FlockingAround, SharkNear, EvadeShark);
        AddTransition(EvadeShark, SharkFarAway, FlockingAround);

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = FlockingAround;
    }
}
