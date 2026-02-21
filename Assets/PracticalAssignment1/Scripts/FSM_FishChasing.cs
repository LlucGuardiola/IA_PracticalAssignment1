using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_FishChasing", menuName = "Finite State Machines/FSM_FishChasing", order = 1)]
public class FSM_FishChasing : FiniteStateMachine
{
    private Pursue pursue;
    private Shark_BLACKBOARD blackboard;
    private SteeringContext steeringContext;

    private float initSpeed;
    private float elapsedTime;

    private GameObject targetFish;

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
        DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* -------- ESTADOS -------- */

        State APROACH = new State("APROACH",
            () => {
                pursue.enabled = false;
                pursue.target = null;
                targetFish = null;
                steeringContext.maxSpeed = initSpeed / 2;
            },
            () => {
                if (targetFish == null)
                {
                    targetFish = SensingUtils.FindInstanceWithinRadius(
                        gameObject, "RED_BOID", blackboard.aproachRadius
                    );

                    if (targetFish != null)
                    {
                        pursue.target = targetFish;
                        pursue.enabled = true;
                    }
                }
            },
            () => { }
        );

        State CHASE = new State("CHASE",
            () => {
                steeringContext.maxSpeed = initSpeed;
            },
            () => { },
            () => { }
        );

        State BITE = new State("BITE",
            () => {
                elapsedTime = 0;

                if (targetFish != null)
                {
                    SteeringContext fishSC = targetFish.GetComponent<SteeringContext>();
                    if (fishSC != null) fishSC.maxSpeed = 0;
                }
            },
            () => {
                elapsedTime += Time.deltaTime;
            },
            () => {
                if (targetFish != null)
                {
                    Destroy(targetFish);
                    targetFish = null;
                }

                pursue.target = null;
                pursue.enabled = false;
            }
        );

        /* -------- TRANSICIONES -------- */

        Transition goingToChase = new Transition("Going To Chase",
            () => targetFish != null &&
                  SensingUtils.DistanceToTarget(gameObject, targetFish) < blackboard.chaseRadius
        );

        Transition notChased = new Transition("Not Chased",
            () => targetFish == null ||
                  SensingUtils.DistanceToTarget(gameObject, targetFish) > blackboard.aproachRadius
        );

        Transition chased = new Transition("Chased",
            () => targetFish != null &&
                  SensingUtils.DistanceToTarget(gameObject, targetFish) < blackboard.biteRadius
        );

        Transition bited = new Transition("Bited",
            () => elapsedTime > blackboard.biteDuration
        );

        /* -------- REGISTRO -------- */

        AddStates(APROACH, CHASE, BITE);

        AddTransition(APROACH, goingToChase, CHASE);

        AddTransition(CHASE, notChased, APROACH);
        AddTransition(CHASE, chased, BITE);

        AddTransition(BITE, bited, APROACH);

        initialState = APROACH;
    }
}
