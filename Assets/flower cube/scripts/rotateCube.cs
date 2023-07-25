using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class rotateCube : MonoBehaviour
{
    private bool first_touch, release, decaying;
    private Vector3 start_pos, vec_start, vec_end, move_delta, cross_prod;
    private float angle;
    private Quaternion current_rot, quat, drag_quat;
    public float timer;

    public GameObject R_controller;
    
    void Start()
    {
        // initialize touch sequence stuff
        first_touch = true;
        release = false;
        decaying = false;
    }

    void Update()
    {
        if (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.95f)
        {
            if (first_touch)
            {
                // when cube first grabbed, get vector from starting position to cube center & cube rotation
                start_pos = Vector3.Normalize(R_controller.transform.position - gameObject.transform.position);
                current_rot = gameObject.transform.rotation;
                first_touch = false;
                release = true;
            }
            
            StartCoroutine(getMovementVector());
            
            // when controller dragged, get vector from closest point on cube, make quaternion from starting vector to current pos vector
            Vector3 closest_point = Vector3.Normalize(R_controller.transform.position - gameObject.transform.position);
            quat = Quaternion.FromToRotation(start_pos, closest_point);

            // apply quaternion to cube (quat * quat to double movement speed)
            gameObject.transform.rotation = quat * quat * current_rot;
        }
        else
        {
            if (!decaying)
            {
                if (release)
                {
                    StartCoroutine(onRelease());
                }
                else
                {
                    // if cube released & done decaying, idle rotate 
                    gameObject.transform.Rotate(new Vector3(15, 15, 15) * Time.deltaTime);
                }
            }
            else 
            {        
                timer += Time.deltaTime;

                if (Mathf.Abs(angle) > 0.01f)
                {
                    // update angle of rotation at an inverse log rate to time
                    angle = angle * (Mathf.Log(-(timer / 12) + 1) + 1);
                    drag_quat = Quaternion.AngleAxis(angle, cross_prod);
                    gameObject.transform.rotation = drag_quat * gameObject.transform.rotation;
                }
                else
                {
                    decaying = false;
                }
            }
        }
    }

    IEnumerator getMovementVector()
    {
        // sample controller position at small intervals
        vec_start = R_controller.transform.position - gameObject.transform.position;

        yield return new WaitForSeconds(0.05f);

        vec_end = R_controller.transform.position - gameObject.transform.position;

        // make quaternion with axis of rotation and angle
        cross_prod = Vector3.Cross(vec_start, vec_end);
        angle = Vector3.Angle(vec_start, vec_end);
        drag_quat = Quaternion.AngleAxis(angle, cross_prod);
    }

    IEnumerator onRelease()
    {
        // upon release of cube, start decaying motion
        first_touch = true;
        release = false;
        decaying = true;

        timer = 0;

        yield return new WaitForSeconds(0.00f);
    }
}
