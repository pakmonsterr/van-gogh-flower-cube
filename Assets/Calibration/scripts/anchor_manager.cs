using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class anchor_manager : MonoBehaviour
{
    // anchor data struct
    [Serializable]
    public class anchor_data
    {
        // handle representing anchor in this runtime
        public ulong space_handle;

        // name of instantiated prefab for this anchor
        public string prefab_name;

        // ref to gameobject instantiated in scene for this anchor
        public GameObject instantiatedObject = null;
    }



    // calibration system
    public TMP_Text debug_text;
    public GameObject main_scene;
    public GameObject calib_marker;
    public GameObject calib_marker_rotation;
    public GameObject calib_system;
    public palm_menu Palm_menu;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void onPressConfirm()
    {
        debug_text.text = $"calib pressed";
        
        Palm_menu.calibrated = true;

        // turn calib_marker transform into OVRPose
        OVRPose calib_pose = new OVRPose()
        {
            position = calib_marker.transform.position,
            orientation = calib_marker_rotation.transform.rotation
        };

        // convert pose to world coords
        OVRPose world_calib_pose = OVRExtensions.ToWorldSpacePose(calib_pose);

        main_scene.transform.position = world_calib_pose.position;
        main_scene.transform.rotation = world_calib_pose.orientation;
        main_scene.SetActive(true);
        calib_system.SetActive(false);

        /*// create info abt spatial anchor (time & position) that has same info as calib_marker
        OVRPlugin.SpatialEntityAnchorCreateInfo createInfo = new OVRPlugin.SpatialEntityAnchorCreateInfo()
        {
            Time = OVRPlugin.GetTimeInSeconds(),
            BaseTracking = OVRPlugin.GetTrackingOriginType(),
            PoseInSpace = controllerPose.ToPosef() //notice that we take the pose in tracking coordinates and convert it from left handed to right handed reference system
        };*/
    }

    public void onPressRedo()
    {
        debug_text.text = $"redo pressed";

        main_scene.SetActive(false);
        calib_system.SetActive(true);

        Palm_menu.calibrated = false;
    }
}
