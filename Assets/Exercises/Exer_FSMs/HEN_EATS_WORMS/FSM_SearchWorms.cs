using FSMs;
using UnityEngine;
using Steerings;


[CreateAssetMenu(fileName = "FSM_SearchWorms", menuName = "Finite State Machines/FSM_SearchWorms", order = 1)]
public class FSM_SearchWorms : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    private HEN_Blackboard blackboard;
    private WanderAround wanderAround;
    private Arrive arrive;
    private AudioSource audioSource;
    private GameObject theWorm;
    private float elapsedTime;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        /* COMPLETE */

        blackboard = GetComponent<HEN_Blackboard>();
        wanderAround = GetComponent<WanderAround>();
        arrive = GetComponent<Arrive>();
        audioSource = GetComponent<AudioSource>();
        theWorm = GetComponent<GameObject>();
        elapsedTime = 0;

        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */

        /* COMPLETE */
        audioSource.Stop();
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* COMPLETE */
        /*
         STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
        */
        State WANDER = new State("WANDER",
            () => { wanderAround.enabled = true; audioSource.clip = blackboard.cluckingSound; audioSource.Play(); }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { wanderAround.enabled = false; audioSource.Stop(); }  // write on exit logic inisde {}  
        );


        State REACHWORM = new State("REACHWORM",
            () => { arrive.target = theWorm; arrive.enabled = true; },
            () => { },
            () => { arrive.enabled = false; }
        );

        State EAT = new State("EAT",
           () => { elapsedTime = 0; audioSource.clip = blackboard.eatingSound; },
           () => { elapsedTime += Time.deltaTime; },
           () => { GameObject.Destroy(theWorm); audioSource.Stop(); }
       );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------
        */
        Transition wormDetected = new Transition("wormDetected",
            () => { return theWorm = SensingUtils.FindInstanceWithinRadius(gameObject, "WORM", blackboard.wormDetectableRadius); }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        Transition wormReached = new Transition("wormReached",

            () => { return SensingUtils.FindInstanceWithinRadius(gameObject, "WORM", blackboard.wormReachedRadius); }


        );
        Transition wormVanished = new Transition("wormVanished",

           () =>
           {

               if (theWorm == null || theWorm.Equals(null))
               {
                   return true;
               }
               return false;
           }

       );

        Transition timeOut = new Transition("timeOut",
            () => { return elapsedTime >= blackboard.timeToEatWorm; }
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
           
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(WANDER, REACHWORM, EAT);

        AddTransition(WANDER, wormDetected, REACHWORM);
        AddTransition(REACHWORM,wormReached,EAT);
        AddTransition(REACHWORM,wormVanished,WANDER);
        AddTransition(EAT,timeOut,WANDER);

        /* STAGE 4: set the initial state
         
        initialState = ... 

        

         */

        initialState = WANDER;
    }
}
