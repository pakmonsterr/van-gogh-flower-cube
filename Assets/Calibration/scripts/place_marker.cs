using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class place_marker : MonoBehaviour
{
    [AddComponentMenu("Scripts/MRTK/Examples/centerOnPointerEvent")]
    public TMP_Text debug_text;

    // broader control
    //bool marker_placed;

    // hand & controller
    public OVRHand R_hand;
    public OVRHand L_hand;
    public GameObject R_hand_obj;
    public GameObject R_controller_obj;
    bool hand_tracking;
    GameObject R_active;

    // floor & calibration
    public Collider floor_collider;
    public GameObject calib_marker;
    public LineRenderer calib_ray_LR;
    Vector3[] calib_ray_coords = new Vector3[2];
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // detect if hand or controller is being used, set active game object accordingly
        hand_tracking = OVRPlugin.GetHandTrackingEnabled();
        R_active = hand_tracking ? R_hand_obj : R_controller_obj;

        //debug_text.text = $"{R_hand.HandConfidence == OVRHand.TrackingConfidence.High}";

        if (hand_tracking) 
        {
            if (R_hand.GetFingerIsPinching(OVRHand.HandFinger.Index) && (R_hand.HandConfidence == OVRHand.TrackingConfidence.High))
            {
                CastCalibMarker(R_active);
            }
            else
            {
                calib_ray_LR.positionCount = 0;
            }
        }
        else if (!hand_tracking)
        {
            OVRInput.Update();
            if (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger) > 0.95f)
            {
                CastCalibMarker(R_active);
            }
            else
            {
                calib_ray_LR.positionCount = 0;
            }
        }

    }

    // set calib marker & ray to raycast hit point
    private void CastCalibMarker(GameObject active_R_obj)
    {
        Vector3 endpoint = GetRaycastFloorEndpoint(active_R_obj.transform.position, active_R_obj.transform.rotation);

        calib_ray_LR.positionCount = 2;
        calib_ray_coords[0] = R_active.transform.position;
        calib_ray_coords[1] = endpoint;
        calib_ray_LR.SetPositions(calib_ray_coords);
        
        calib_marker.transform.position = endpoint;
        calib_marker.transform.eulerAngles = new Vector3(90, R_active.transform.eulerAngles.y - 5, 0);
    }

    // cast ray from hand/controller to floor, return hit point on floor
    public Vector3 GetRaycastFloorEndpoint(Vector3 pos_data, Quaternion rot_data)
    {
        Ray handRay = new Ray(pos_data, rot_data * Vector3.forward);

        if (floor_collider.Raycast(handRay, out var hitPoint, 1000.0f))
        {
            return hitPoint.point;
        }

        return new Vector3(pos_data.x, 0, pos_data.z);
    }
}
