using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_DriveAway", menuName = "Finite State Machines/FSM_DriveAway", order = 1)]
public class FSM_DriveAway : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private HEN_Blackboard blackboard;
    private SteeringContext steeringContext;
    private Seek seek;
    private GameObject chick;
    private AudioSource audioSource;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<HEN_Blackboard>();
        steeringContext = GetComponent<SteeringContext>();
        seek = GetComponent<Seek>();
        audioSource = GetComponent<AudioSource>();
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        if (currentState.Name.Equals("ANGRY"))
        {
            gameObject.transform.localScale /= 1.4f;
            steeringContext.maxAcceleration /= 2;
            steeringContext.maxSpeed /= 2;
        }

        audioSource.Stop();
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


        FiniteStateMachine searchingWorms = ScriptableObject.CreateInstance<FSM_SearchWorms>();
        searchingWorms.name = "SearchingWorms";

        State DrivesAway = new State("DrivesAway",
            () => {
                seek.target = chick;
                seek.enabled = true;
                transform.localScale *= 1.4f;
                steeringContext.maxAcceleration *= 2.0f;
                steeringContext.maxSpeed *= 2.0f;
                audioSource.clip = blackboard.angrySound;
                audioSource.Play(); 
            }, 
            () => { },
            () => { 
                seek.enabled = false;
                transform.localScale /= 1.4f;
                steeringContext.maxAcceleration /= 2;
                steeringContext.maxSpeed /= 2;
                audioSource.Stop();
            }  
        );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        Transition ChickTooClose = new Transition("ChickTooClose",
           () => { chick = SensingUtils.FindInstanceWithinRadius(gameObject, "CHICK", blackboard.chickDetectionRadius); return chick != null;}
        );
        Transition ChickFarEnough = new Transition("ChickFarEnough",
           () => { return SensingUtils.DistanceToTarget(gameObject, chick) >= blackboard.chickFarEnoughRadius; }
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(searchingWorms, DrivesAway);

        AddTransition(searchingWorms, ChickTooClose, DrivesAway);
        AddTransition(DrivesAway,ChickFarEnough, searchingWorms);

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = searchingWorms;
    }
}
