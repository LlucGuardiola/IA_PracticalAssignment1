using FSMs;
using UnityEngine;
using Steerings;
using UnityEngine.InputSystem.DualShock;

[CreateAssetMenu(fileName = "FSM_GettingCloser", menuName = "Finite State Machines/FSM_GettingCloser", order = 1)]
public class FSM_GettingCloser : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private WanderAround wanderAround;
    private SteeringContext steeringContext;
    private Seek seek;
    private GameObject attractor;
    private HEN_Blackboard blackboard;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        seek = GetComponent<Seek>();
        wanderAround = GetComponent<WanderAround>();
        steeringContext = GetComponent<SteeringContext>();
        blackboard = GetComponent<HEN_Blackboard>();
        wanderAround.attractor = attractor;
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
        FiniteStateMachine eatAlone = GetComponent<FSM_DriveAway>();
        eatAlone.name = "eatAlone";


        State gettingCloser = new State("gettingCloser",
            () => { steeringContext.seekWeight = 0.7f; wanderAround.enabled = true; }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { wanderAround.enabled = false; }  // write on exit logic inisde {}  
        );
        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */

        Transition toFarFromAtractor = new Transition("toFarFromAtractor",
            () => { return SensingUtils.DistanceToTarget(gameObject, attractor) >= blackboard.tooFarFromAttractor; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition closeEnoughFromAtractor = new Transition("closeEnoughFromAtractor",
          () => { return SensingUtils.DistanceToTarget(gameObject, attractor) <= blackboard.closeEnoughToAttractor; }, // write the condition checkeing code in {}
          () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(eatAlone, gettingCloser);
        AddTransition(eatAlone, toFarFromAtractor, gettingCloser);
        AddTransition(gettingCloser,closeEnoughFromAtractor,eatAlone);

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = eatAlone;

    }
}
