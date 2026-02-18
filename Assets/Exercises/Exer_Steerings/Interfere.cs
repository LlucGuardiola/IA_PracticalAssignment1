using NUnit.Framework.Internal;
using UnityEngine;

namespace Steerings
{

    public class Interfere : SteeringBehaviour
    {


        // remove comments for steerings that must be provided with a target 
        // remove whole block if no explicit target required
        // (if FT or FTI policies make sense, then this method must be present)
        public GameObject target;
        public float requiredDistance;

        public override GameObject GetTarget()
        {
            return target;
        }


        public override Vector3 GetLinearAcceleration()
        {
            return Interfere.GetLinearAcceleration(Context, target, requiredDistance);
        }

        
        public static Vector3 GetLinearAcceleration(SteeringContext me, GameObject target, float dist)
        {
            /* COMPLETE this method. It must return the linear acceleration (Vector3) */
            SteeringContext targetContext = target.GetComponent<SteeringContext>();

            Vector3 targetVelocity = targetContext.velocity;
            targetVelocity.Normalize();

            Debug.DrawLine(target.transform.position,target.transform.position + targetVelocity *200,Color.red);

            SURROGATE_TARGET.transform.position = target.transform.position + targetVelocity * dist;

            

            return Arrive.GetLinearAcceleration(me, SURROGATE_TARGET);
        }

      

    }
}