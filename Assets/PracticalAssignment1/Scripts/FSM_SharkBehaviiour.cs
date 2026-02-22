using FSMs;
using UnityEngine;
using Steerings;
using UnityEditor.Experimental.GraphView;

[CreateAssetMenu(fileName = "FSM_SharkBehaviiour", menuName = "Finite State Machines/FSM_SharkBehaviiour", order = 1)]
public class FSM_SharkBehaviiour : FiniteStateMachine
{
    private Shark_BLACKBOARD blackboard;

    public override void OnEnter()
    {
      
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
        FiniteStateMachine FishChasing = ScriptableObject.CreateInstance<FSM_FishChasing>();
        FishChasing.name = "FishChasing";


        FiniteStateMachine SharkResting = ScriptableObject.CreateInstance<FSM_Resting>();
        SharkResting.name = "SharkResting";

        FiniteStateMachine SharkHidding = ScriptableObject.CreateInstance<FSM_SharkHidding>();
        SharkHidding.name = "SharkHidding";

        Transition toChasing = new Transition("toChasing",
            () => {
                blackboard.currentFish = SensingUtils.FindInstanceWithinRadius(gameObject, "RED_BOID", blackboard.aproachRadius);          
                return (blackboard.currentFish != null) && (blackboard.fishesOnScene >= blackboard.fishesToChase);
            }, 
            () => { } 
        );

        Transition fishEaten = new Transition("Fish Eaten",
            ()=> { return blackboard.totalFishesEaten >= blackboard.totalFishToBeFed; }
        );

        Transition toHide = new Transition("toHide",
            ()=> { return blackboard.totalFishesEaten <= 0;},
            () => { }
        );

       

        AddStates(FishChasing, SharkResting, SharkHidding);

        AddTransition(SharkHidding,toChasing, FishChasing);
        AddTransition(FishChasing, fishEaten, SharkResting);
        AddTransition(SharkResting, toHide, SharkHidding);
        initialState  = SharkHidding;

    }
}
