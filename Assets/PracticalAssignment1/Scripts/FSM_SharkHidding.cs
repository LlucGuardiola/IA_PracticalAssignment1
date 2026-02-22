using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_SharkHiding", menuName = "Finite State Machines/FSM_", order = 1)]
public class FSM_SharkHidding : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private Arrive arrive;
    private Shark_BLACKBOARD blackboard;
    private float elapsedTime;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        arrive = GetComponent<Arrive>();
        blackboard = GetComponent<Shark_BLACKBOARD>();
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
        State GoToKelp = new State("GoToKelp",
            () => { 
                arrive.enabled = true;
                arrive.target = blackboard.kelpZone;
            }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { arrive.enabled = false; }  // write on exit logic inisde {}  
        );

        State Hide = new State("Hide",
           () => { elapsedTime = 0f; }, // write on enter logic inside {}
           () => { elapsedTime += Time.deltaTime; }, // write in state logic inside {}
           () => { }  // write on exit logic inisde {}  
        );

        State Peek = new State("Peek",
            () => { 
                arrive.enabled = true;
                arrive.target = blackboard.PeekZone;
                elapsedTime = 0f;
            }, // write on enter logic inside {}
            () => { elapsedTime += Time.deltaTime; }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );
        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        Transition KelpReached = new Transition("KelpReached",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.kelpZone) < blackboard.KelpZoneReachedRadious; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition TimeOutHide = new Transition("TimeOutHide",
            () => { return elapsedTime >= Random.Range(blackboard.hideTime - 1, blackboard.hideTime + 1); }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition TimeOutPeek = new Transition("TimeOutPeek",
            () => { return elapsedTime >= Random.Range(blackboard.peekTime, blackboard.peekTime+2); }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */

        AddStates(GoToKelp, Hide, Peek);
        AddTransition(GoToKelp,KelpReached,Hide);
        AddTransition(Hide,TimeOutHide,Peek);
        AddTransition(Peek,TimeOutPeek,GoToKelp);

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = GoToKelp;

    }
}
