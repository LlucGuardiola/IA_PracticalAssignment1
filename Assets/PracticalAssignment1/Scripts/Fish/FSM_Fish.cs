using FSMs;
using UnityEngine;
using Steerings;
using Unity.Hierarchy;

[CreateAssetMenu(fileName = "Fish_FSM", menuName = "Finite State Machines/Fish_FSM", order = 1)]
public class FSM_Fish : FiniteStateMachine
{
    private Fish_BLACKBOARD blackboard;
    private EvadePlusOA evadePlusOA;
    private FlockingAroundPlusAvoidance flockingAround;
    private GameObject shark;

    public override void OnEnter()
    {
        blackboard = GetComponent<Fish_BLACKBOARD>();
        evadePlusOA = GetComponent<EvadePlusOA>();
        flockingAround = GetComponent<FlockingAroundPlusAvoidance>();

        base.OnEnter();
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        State FlockingAround = new State("FlockingAround",
           () => { flockingAround.enabled = true; },
           () => { Debug.Log("Flocking"); },
           () => { flockingAround.enabled = false; }
        );

        State EvadeShark = new State("EvadeShark",
           () =>
           {
               evadePlusOA.enabled = true;
               evadePlusOA.target = shark;

           },
           () => { Debug.Log("EvadePLusOA"); },
           () =>
           {
               evadePlusOA.enabled = false;
           }
        );

        Transition SharkNear = new Transition("SharkNear",
            () => { shark = SensingUtils.FindInstanceWithinRadius(gameObject, "SHARK", blackboard.sharkNearRadious); return shark != null; },
            () => { }
        );
        Transition SharkFarAway = new Transition("SharkFarAway",
           () => { return SensingUtils.DistanceToTarget(gameObject, shark) >= blackboard.sharkNearRadious; },
           () => { }
        );

        AddStates(FlockingAround, EvadeShark);

        AddTransition(FlockingAround, SharkNear, EvadeShark);
        AddTransition(EvadeShark, SharkFarAway, FlockingAround);

        initialState = FlockingAround;
    }
}