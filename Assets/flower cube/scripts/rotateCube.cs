using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class rotateCube : MonoBehaviour
{
    public TMP_Text debug_text;
    public GameObject flower_cube;

    // touch sphere stuff
    public GameObject touch_sphere;
    public Material selected_mat;
    public Material not_selected_mat;

    // cube rotation stuff
    private bool first_touch;
    private Vector3 start_pos;
    private Quaternion current_rot;
    private Quaternion quat;
    public int drag_speed;
    
    // Start is called before the first frame update
    void Start()
    {
        first_touch = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter(Collision col) 
    {
    }

    void OnCollisionStay(Collision col) 
    {
        if (col.gameObject.name == "touch_sphere" && (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.95f))
        {
            touch_sphere.GetComponent<MeshRenderer>().material = selected_mat;

            if (first_touch)
            {
                start_pos = Vector3.Normalize(touch_sphere.transform.position - flower_cube.transform.position);
                current_rot = flower_cube.transform.rotation;
                first_touch = false;
            }
            else
            {
                Vector3 closest_point = Vector3.Normalize(touch_sphere.transform.position - flower_cube.transform.position);
                quat = Quaternion.FromToRotation(start_pos, closest_point);
                flower_cube.transform.rotation = quat * quat * current_rot;
            }
        }
        else
        {
            touch_sphere.GetComponent<MeshRenderer>().material = not_selected_mat; 
            first_touch = true;

            //StartCoroutine(releaseDrag());
        }
    }

    void OnCollisionExit(Collision col) 
    {
        touch_sphere.GetComponent<MeshRenderer>().material = not_selected_mat; 
        first_touch = true;
    }

    /*private IEnumerator releaseDrag()
    {
        debug_text.text = $"{quat.eulerAngles}";

        Vector3 rotation  = quat.eulerAngles;

        float time = 1;

        while (time < 6.0f)
        {
            time += Time.deltaTime;
            rotation = rotation / time;
            flower_cube.transform.Rotate(rotation, Space.Self);
            debug_text.text = $"{time}";
        }

        yield return new WaitForSeconds(0.0f);
    }*/
}
