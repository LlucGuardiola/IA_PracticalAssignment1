using Steerings;
using UnityEngine;

namespace Steerings
{
    public class EvadePlusOA : SteeringBehaviour
    {
        public GameObject target;

        public override GameObject GetTarget()
        {
            return target;
        }

        public override Vector3 GetLinearAcceleration()
        {
            return EvadePlusOA.GetLinearAcceleration(Context, target);
        }

        public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject target)
        {
            Vector3 avoidanceAcceleration = ObstacleAvoidance.GetLinearAcceleration(me);
            if (avoidanceAcceleration.Equals(Vector3.zero))
                return Evade.GetLinearAcceleration(me, target);
            else
                return avoidanceAcceleration;
        }
    }

}
