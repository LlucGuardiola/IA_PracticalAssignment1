using FSMs;
using UnityEngine;
using Steerings;


[CreateAssetMenu(fileName = "FSM_Resting", menuName = "Finite State Machines/FSM_Resting", order = 1)]
public class FSM_Resting : FiniteStateMachine
{
    private Shark_BLACKBOARD blackboard;
    private Arrive arrive;

    private float elapsedTime;

    private int vomitIndex;
    private float vomitTimeInterval;
    private float vomitTimer;
    private Vector3 startScale;
    private Vector3 targetScale;
    


    public override void OnEnter()
    {
        blackboard = GetComponent<Shark_BLACKBOARD>();
        arrive = GetComponent<Arrive>();
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        State Sleep = new State("Sleep",
         () => { blackboard.sleepIcon.SetActive(true); elapsedTime = 0; },
         () => { elapsedTime += Time.deltaTime; },
         () => { blackboard.sleepIcon.SetActive(false); }
        );

        State GoToVomitZone = new State("GoToVomitZone",
          () => { arrive.enabled = true; arrive.target = blackboard.vomitZone; },
          () => { },
          () => { arrive.enabled = false; }
        );

        State Vomit = new State("Vomit",
            () => { 
                vomitTimeInterval = blackboard.vomitTime / blackboard.elementsToVomit;
                vomitIndex = 0; 
            },
            () => { 
                vomitTimer += Time.deltaTime;

                if (vomitTimer >= vomitTimeInterval)
                {
                    vomitTimer = 0f;

                    GameObject fishbone = Instantiate(blackboard.fishbonePrefab);
                    fishbone.transform.position = blackboard.mouth.gameObject.transform.position;
                    fishbone.transform.localRotation = Quaternion.Euler(0, 0,gameObject.transform.rotation.z);
                    blackboard.totalFishesEaten--;
                    vomitIndex++;
                }
            }, 
            () => { }  
        );
        State PooZone = new State("PooZone",
            () => { arrive.enabled = true; arrive.target = blackboard.pooZone; },
            () => { },
            () => { arrive.enabled = false; }
        );
        State Poo = new State("Poo",
           () => { blackboard.poo.SetActive(true); ; elapsedTime = 0; },
           () => { elapsedTime += Time.deltaTime; },
           () => { blackboard.poo.SetActive(false); }
        );

        State Breathe = new State("Breathe",
          () => {
               startScale = gameObject.transform.localScale;
               targetScale = startScale * 2.0f;
               elapsedTime = 0; 
          
          },
          () => { 
              elapsedTime += Time.deltaTime;
              float t = elapsedTime / blackboard.breatheTime;
              gameObject.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
          },
          () => { gameObject.transform.localScale = startScale; }
        );

        Transition goingToVomit = new Transition("goingToVomit",
            () => { return elapsedTime >= blackboard.sleepTime; },
            () => { }
        );
        Transition goingToZone = new Transition("goingToZone",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.vomitZone) < blackboard.vomitZoneReachedRadious; },
            () => { }  
        );
        Transition goingToPooZone = new Transition("goingToPooZone",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.pooZone) < blackboard.pooZoneReachedRadious; },
            () => { }
        );
        Transition goingToPoo = new Transition("goingToPoo",
            () => { return vomitIndex == blackboard.elementsToVomit; }
        );
        Transition goingToBreathe = new Transition("Breathe",
            () => { return elapsedTime >= blackboard.pooTime; }
        );

        Transition goingToSleep = new Transition("goingTosleep",
            () => { return elapsedTime >= blackboard.breatheTime; }
        );

        AddStates(Sleep, Vomit, Poo, Breathe, GoToVomitZone, PooZone);

        AddTransition(Sleep, goingToVomit, GoToVomitZone);
        AddTransition(GoToVomitZone, goingToZone, Vomit);
        AddTransition (Vomit, goingToPoo, PooZone);
        AddTransition(PooZone, goingToPooZone, Poo);
        AddTransition (Poo, goingToBreathe, Breathe);
        AddTransition (Breathe, goingToSleep, Sleep);

        initialState = GoToVomitZone;
    }
}
