using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate_cube : MonoBehaviour
{
    private bool first_touch;
    private Vector3 start_pos;
    private Quaternion current_rot;
    private Quaternion quat;

    public GameObject R_controller;
    
    // Start is called before the first frame update
    void Start()
    {
        
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
            first_touch = true;
            gameObject.transform.Rotate(new Vector3(15, 15, 15) * Time.deltaTime);
        }
    }
}
