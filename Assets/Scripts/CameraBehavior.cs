using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour {
    // ahhhh
    public Transform target;

    public Vector3 offsetPosition;

    private Space offsetPositionSpace = Space.Self;

    private bool lookAt = true;

    public float riseLevel = 10f;

    private void Update()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (target == null)
        {
            Debug.LogWarning("Missing target ref !", this);

            return;
        }
        Vector3 targetPos;
        // compute position
        if (offsetPositionSpace == Space.Self)
        {
            targetPos = target.TransformPoint(offsetPosition);
        }
        else
        {
            targetPos = target.position + offsetPosition;
        }
        transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime * 2);

        // compute rotation
        if (lookAt)
        {

            Quaternion look;
            transform.LookAt(new Vector3(target.position.x + target.forward.x, target.position.y + riseLevel, target.position.z + target.forward.z));

            look = Quaternion.LookRotation(target.forward); //Quaternion.LookRotation(carPhysics.velocity.normalized);

            //// Rotate the camera towards the velocity vector.
            look = Quaternion.Slerp(transform.rotation, look, 5.0f * Time.fixedDeltaTime);
            transform.rotation = look;
        }
        else
        {
            transform.rotation = Quaternion.Slerp(target.rotation, Quaternion.LookRotation(new Vector3(target.position.x, target.position.y + riseLevel, target.position.z)), 0.25f * Time.fixedDeltaTime / 2);

            //  transform.rotation = Quaternion.LookRotation(target.position);
        }
    }
}
