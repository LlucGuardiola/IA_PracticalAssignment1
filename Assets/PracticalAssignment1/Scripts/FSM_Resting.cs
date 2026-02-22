using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Resting", menuName = "Finite State Machines/FSM_Resting", order = 1)]
public class FSM_Resting : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/


    private Shark_BLACKBOARD blackboard;
    private SteeringContext steeringContext;
    private Arrive arrive;

    private float elapsedTime;

    private int vomitIndex;
    private float vomitTime;
    private Vector3 startScale;
    private Vector3 targetScale;


    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<Shark_BLACKBOARD>();
        steeringContext = GetComponent<SteeringContext>();
        arrive = GetComponent<Arrive>();
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
        State Sleep = new State("Sleep",
         () => { blackboard.sleepIcon.SetActive(true); elapsedTime = 0; }, // write on enter logic inside {}
         () => { elapsedTime += Time.deltaTime; }, // write in state logic inside {}
         () => { blackboard.sleepIcon.SetActive(false); }  // write on exit logic inisde {}  
        );

        State GoToVomitZone = new State("GoToVomitZone",
          () => { arrive.enabled = true; arrive.target = blackboard.vomitZone; }, // write on enter logic inside {}
          () => { }, // write in state logic inside {}
          () => { arrive.enabled = false; }  // write on exit logic inisde {}  
        );


        State Vomit = new State("Vomit",
            () => { 
                elapsedTime = 0;
                vomitTime = 0;  
                vomitIndex = 0;
            }, // write on enter logic inside {}
            () => { 
                elapsedTime += Time.deltaTime;
                vomitTime += Time.deltaTime;

                if((vomitIndex < blackboard.vomitElements.Length) && (vomitTime >= blackboard.vomitTimeInterval))
                {
                    blackboard.vomitElements[vomitIndex].SetActive(true);
                    vomitIndex++;
                    vomitTime = 0;
                }
            }, // write in state logic inside {}
            () => {
                foreach (GameObject v in blackboard.vomitElements)
                {
                    v.SetActive(false);
                }
            }  // write on exit logic inisde {}  
        );
        State PooZone = new State("PooZone",
            () => { arrive.enabled = true; arrive.target = blackboard.pooZone; },
            () => { },
            () => { arrive.enabled = false; }

        );
        State Poo = new State("Poo",
           () => { blackboard.poo.SetActive(true); ; elapsedTime = 0; }, // write on enter logic inside {}
           () => { elapsedTime += Time.deltaTime; }, // write in state logic inside {}
           () => { blackboard.poo.SetActive(false); }  // write on exit logic inisde {}  
        );

        State Breathe = new State("Breathe",
          () => {
               startScale = gameObject.transform.localScale;
               targetScale = startScale * 2.0f;
               elapsedTime = 0; 
          
          
          }, // write on enter logic inside {}
          () => { 
              elapsedTime += Time.deltaTime;
              float t = elapsedTime / blackboard.breatheTime;
              gameObject.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
          }, // write in state logic inside {}
          () => { gameObject.transform.localScale = startScale; }  // write on exit logic inisde {}  
        );
        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        Transition goingToVomit = new Transition("goingToVomit",
            () => { return elapsedTime >= blackboard.sleepTime; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition goingToZone = new Transition("goingToZone",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.vomitZone) < blackboard.vomitZoneReachedRadious; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition goingToPooZone = new Transition("goingToPooZone",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.pooZone) < blackboard.pooZoneReachedRadious; }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        Transition goingToPoo = new Transition("goingToPoo",
            () => { return (elapsedTime >= blackboard.vomitTime) && (vomitIndex >= blackboard.vomitElements.Length); }
        );
        Transition goingToBreathe = new Transition("Breathe",
            () => { return elapsedTime >= blackboard.pooTime; }
        );

        Transition goingToSleep = new Transition("goingTosleep",
            () => { return elapsedTime >= blackboard.breatheTime; }
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */
        AddStates(Sleep, Vomit, Poo, Breathe, GoToVomitZone, PooZone);

        AddTransition(Sleep, goingToVomit, GoToVomitZone);
        AddTransition(GoToVomitZone, goingToZone, Vomit);
        AddTransition (Vomit, goingToPoo, PooZone);
        AddTransition(PooZone, goingToPooZone, Poo);
        AddTransition (Poo, goingToBreathe, Breathe);
        AddTransition (Breathe, goingToSleep, Sleep);

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = Sleep;
    }
}
