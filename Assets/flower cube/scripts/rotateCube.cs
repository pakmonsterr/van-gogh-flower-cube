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

    public GameObject R_controller;

    public TMP_Text debug_text_1;
    public TMP_Text debug_text_2;
    public float timer = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        first_touch = true;
        release = false;
        decaying = false;
    }

    // Update is called once per frame
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
            else
            {
                StartCoroutine(getMovementVector());
                
                // when controller dragged, get vector from closest point on cube, make quaternion from starting vector to current pos vector
                Vector3 closest_point = Vector3.Normalize(R_controller.transform.position - gameObject.transform.position);
                quat = Quaternion.FromToRotation(start_pos, closest_point);

                // apply quaternion to cube (quat * quat to double movement speed)
                gameObject.transform.rotation = quat * quat * current_rot;
            }
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

                if (timer < 2.0f)
                {
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
        vec_start = R_controller.transform.position - gameObject.transform.position;

        yield return new WaitForSeconds(0.05f);

        vec_end = R_controller.transform.position - gameObject.transform.position;
        cross_prod = Vector3.Cross(vec_start, vec_end);
        angle = Vector3.Angle(vec_start, vec_end);

        drag_quat = Quaternion.AngleAxis(angle, cross_prod);

        debug_text_1.text = $"{angle}";
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
