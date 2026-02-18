using UnityEngine;

// Leader following combines Keep position with linear repulsion
// (linear respulsion prevents the agent from colliding against the leader 
// and against other agents following the same leader)

namespace Steerings
{

    public class LeaderFollowingArbitrated : SteeringBehaviour
    {

        
        public GameObject target;
        public float requiredDistance;
        public float requiredAngle;

        public override GameObject GetTarget()
        {
            return target;
        }
      
        
        public override Vector3 GetLinearAcceleration()
        {
            /* COMPLETE */
            return LeaderFollowingArbitrated.GetLinearAcceleration(Context, target, requiredDistance, requiredAngle);
        }

        
        public static Vector3 GetLinearAcceleration (SteeringContext me,GameObject Target,float ReqDist,float reqAngle)
        {
            // Give priority to linear repulsion
            // (if linear repulsion is not Vector3.Zero return linear repulsion
            // else return Keep Position)
            /* COMPLETE */

            Vector3 linearRepulsionAcceleration = LinearRepulsion.GetLinearAcceleration(me);

            if(!(linearRepulsionAcceleration.Equals(Vector3.zero )))
            {
                return linearRepulsionAcceleration;
            }
            else
            {
                return KeepPosition.GetLinearAcceleration(me,Target,ReqDist,reqAngle);
            }

                
        }

    }
}