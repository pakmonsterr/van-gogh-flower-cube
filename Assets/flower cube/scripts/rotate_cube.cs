using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_cube : MonoBehaviour
{
    private bool first_touch, release, decaying;
    private Vector3 start_pos;
    private Quaternion current_rot, quat;

    public GameObject R_controller;
    
    // Start is called before the first frame update
    void Start()
    {
        first_touch = true;
        release = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.95f)
        {
            if (first_touch)
            {
                start_pos = Vector3.Normalize(2 * R_controller.transform.position - gameObject.transform.position);
                current_rot = gameObject.transform.rotation;
                first_touch = false;
                release = true;
            }
            else
            {
                Vector3 closest_point = Vector3.Normalize(2 * R_controller.transform.position - gameObject.transform.position);
                quat = Quaternion.FromToRotation(start_pos, closest_point);
                gameObject.transform.rotation = quat * quat * current_rot;
            }
        }
        else
        {
            if (release && decaying)
            {
                first_touch = true;
                release = false;
                StartCoroutine(decayMotion());
            }
            
            gameObject.transform.Rotate(new Vector3(15, 15, 15) * Time.deltaTime);
        }
    }

    IEnumerator decayMotion()
    {
        
        yield return new WaitForSeconds(0.0f);
    }
}
