using UnityEditor.Rendering;
using UnityEngine;

namespace Steerings
{

    public class LeaderFollowingBlended : SteeringBehaviour
    {
        
        public GameObject target;
        public float requiredDistance;
        public float requiredAngle;

        public float wlr = 0.5f;

        public override GameObject GetTarget()
        {
            return target;
        }
      
        
        public override Vector3 GetLinearAcceleration()
        {
            /* COMPLETE */
            return LeaderFollowingBlended.GetLinearAcceleration(Context, target,requiredDistance,requiredAngle,wlr);
        }

        
        public static Vector3 GetLinearAcceleration (SteeringContext me,GameObject Target,float reqDist,float reqAngle,float wlr /* COMPLETE */)
        {
            /*
             Compute both steerings
                lr = LinearRepulsion.GetLinearAcceleration(...)
                kp = KeepPosition...
             - if lr is zero return kp
             - else return the blending of lr and kp
             */

            Vector3 LinRepAcc = LinearRepulsion.GetLinearAcceleration( me );
            Vector3 KPAcc = KeepPosition.GetLinearAcceleration(me, Target, reqDist, reqAngle);

            if (LinRepAcc.Equals(Vector3.zero))
            {
                return KPAcc;
            }
            else
            {
                return LinRepAcc * wlr + KPAcc * (1-wlr);
            }

                /* COMPLETE */
               
        }
    }
}