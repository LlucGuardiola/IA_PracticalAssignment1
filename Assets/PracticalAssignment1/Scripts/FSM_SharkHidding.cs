using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_SharkHiding", menuName = "Finite State Machines/FSM_", order = 1)]
public class FSM_SharkHidding : FiniteStateMachine
{
    private Arrive arrive;
    private Shark_BLACKBOARD blackboard;
    private float elapsedTime;

    public override void OnEnter()
    {
        arrive = GetComponent<Arrive>();
        blackboard = GetComponent<Shark_BLACKBOARD>();
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        State GoToKelp = new State("GoToKelp",
            () => { 
                arrive.enabled = true;
                arrive.target = blackboard.kelpZone;
            }, 
            () => { }, 
            () => { arrive.enabled = false; } 
        );

        State Hide = new State("Hide",
           () => { elapsedTime = 0f; }, 
           () => { elapsedTime += Time.deltaTime; }, 
           () => { }  
        );

        State Peek = new State("Peek",
            () => { 
                arrive.enabled = true;
                arrive.target = blackboard.PeekZone;
                elapsedTime = 0f;
            }, 
            () => { elapsedTime += Time.deltaTime; }, 
            () => { }  
        );
      
        Transition KelpReached = new Transition("KelpReached",
            () => { return SensingUtils.DistanceToTarget(gameObject, blackboard.kelpZone) < blackboard.KelpZoneReachedRadious; }, 
            () => { } 
        );
        Transition TimeOutHide = new Transition("TimeOutHide",
            () => { return elapsedTime >= Random.Range(blackboard.hideTime - 1, blackboard.hideTime + 1); },
            () => { }  
        );

        Transition TimeOutPeek = new Transition("TimeOutPeek",
            () => { return elapsedTime >= Random.Range(blackboard.peekTime, blackboard.peekTime+2); }, 
            () => { }  
        );

        AddStates(GoToKelp, Hide, Peek);
        AddTransition(GoToKelp,KelpReached,Hide);
        AddTransition(Hide,TimeOutHide,Peek);
        AddTransition(Peek,TimeOutPeek,GoToKelp);

        initialState = GoToKelp;
    }
}
