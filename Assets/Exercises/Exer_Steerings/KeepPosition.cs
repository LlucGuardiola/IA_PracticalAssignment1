using UnityEngine;

namespace Steerings
{

    public class KeepPosition : SteeringBehaviour
    {

        public GameObject target;
        public float requiredDistance;
        public float requiredAngle;

        /* COMPLETE */
        public override GameObject GetTarget()
        {
            return target;
        }
        public override Vector3 GetLinearAcceleration()
        {
            /* COMPLETE */
            return GetLinearAcceleration(Context,target,requiredDistance,requiredAngle);
        }

        
        public static Vector3 GetLinearAcceleration (SteeringContext me, GameObject target,
                                                     float distance, float angle)
        {
            /* COMPLETE */
            float desiredAngle;
            Vector3 desireDirectionFromTarget;
            desiredAngle = target.transform.rotation.eulerAngles.z + angle;
            desireDirectionFromTarget = Utils.OrientationToVector(desiredAngle).normalized;

            SURROGATE_TARGET.transform.position = target.transform.position + desireDirectionFromTarget * distance;
            
            return Arrive.GetLinearAcceleration(me, SURROGATE_TARGET); // remove this line when exercise completed
        }

    }
}