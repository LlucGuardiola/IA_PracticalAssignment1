using FSMs;
using UnityEngine;
using Steerings;
using UnityEditor.SceneManagement;

[CreateAssetMenu(fileName = "FSM_FishChasing", menuName = "Finite State Machines/FSM_FishChasing", order = 1)]
public class FSM_FishChasing : FiniteStateMachine
{
    private Pursue pursue;
    private Shark_BLACKBOARD blackboard;
    private SteeringContext steeringContext;

    private float initSpeed;
    private float elapsedTime;

    public override void OnEnter()
    {
        steeringContext = GetComponent<SteeringContext>();
        pursue = GetComponent<Pursue>();
        blackboard = GetComponent<Shark_BLACKBOARD>();
        initSpeed = steeringContext.maxSpeed;

        base.OnEnter(); 
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        State APROACH = new State("APROACH",
            () => {
                steeringContext.maxSpeed = initSpeed / 2;
                pursue.enabled = false;
                pursue.target = null;
            },
            () => {

                GameObject fish = SensingUtils.FindInstanceWithinRadius(
                    gameObject, "FISH", blackboard.aproachRadius
                );

                if (fish != null)
                {
                    pursue.target = fish;
                    pursue.enabled = true;
                }
            },
            () => { }
        );

        State Chase = new State("Chase",
            () => { steeringContext.maxSpeed = initSpeed *2; },
            () => { },
            () => { }
        );

        State Bite = new State("Bite",
            () => { elapsedTime = 0; pursue.target.GetComponent<SteeringContext>().maxSpeed = 0; },
            () => { elapsedTime += Time.deltaTime;},
            () => {
                blackboard.totalFishesEaten++;
                blackboard.fishesOnScene--;
                Destroy(pursue.target.gameObject);
                pursue.enabled = false;    
            }
        );

        Transition goingToChase = new Transition("Going To Chase",
            () => pursue.target != null &&
                    SensingUtils.DistanceToTarget(gameObject, pursue.target) < blackboard.chaseRadius
         );


        Transition notChased = new Transition("Not Chased",
            () => { return SensingUtils.DistanceToTarget(gameObject, pursue.target) > blackboard.aproachRadius; }, 
            () => { }  
        );

        Transition chased = new Transition("Chased",
            () => { return SensingUtils.DistanceToTarget(gameObject, pursue.target) < blackboard.biteRadius; },
            () => { }
        );

        Transition bited = new Transition("Bited",
            () => { return elapsedTime > blackboard.biteDuration; },
            () => { }
        );

        AddStates(APROACH, Chase, Bite);

        AddTransition(APROACH, goingToChase, Chase);
        AddTransition(Chase, notChased, APROACH);
        AddTransition (Chase, chased, Bite);
        AddTransition(Bite, bited, APROACH);

        initialState = APROACH;

    }
}
