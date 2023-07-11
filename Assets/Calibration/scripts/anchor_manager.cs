using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using TMPro;

public class anchor_manager : MonoBehaviour
{

    // calibration system
    public TMP_Text debug_text;
    public GameObject main_scene;
    public GameObject calib_marker;
    public GameObject calib_marker_rotation;
    public GameObject calib_system;
    public palm_menu Palm_menu;

    // anchor stuff
    public GameObject main_anchor;
    private OVRSpatialAnchor spatial_anchor;
    
    // Start is called before the first frame update
    void Start()
    {
        spatial_anchor = main_anchor.GetComponent<OVRSpatialAnchor>();
        
        // add spatial anchor to holder if none exist
        if (!spatial_anchor)
        {
            debug_text.text = "no existing anchor, added";
            main_anchor.AddComponent<OVRSpatialAnchor>();
            spatial_anchor = main_anchor.GetComponent<OVRSpatialAnchor>();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void onPressConfirm()
    {
        //debug_text.text = $"calib pressed";
        Palm_menu.calibrated = true;

        // spawn main scene @ calib marker position & (corrected) rotation
        main_scene.transform.position = calib_marker.transform.position;
        main_scene.transform.rotation = calib_marker_rotation.transform.rotation;

        // system management stuff
        Palm_menu.calibrated = true;
        main_scene.SetActive(true);
        calib_system.SetActive(false);

        // save anchor (locally)
        spatial_anchor.Save((anchor, success) =>
        {
            if (!success)
            {
                debug_text.text = "anchor save failed";
            }
            else
            {
                debug_text.text = $"anchor saved: {ConvertUuidToString(spatial_anchor.Uuid)}";
            }
            //SaveUuidToPlayerPrefs(anchor.Uuid);
        });
    }

    public void onPressRedo()
    {   
        // system management stuff
        Palm_menu.calibrated = false;
        main_scene.SetActive(false);
        calib_system.SetActive(true);
        
        // erase anchor
        spatial_anchor.Erase((anchor, success) =>
        {
            if (!success)
            {
                debug_text.text = "anchor erase failed";
                return;
            }
            else
            {
                debug_text.text = $"anchor erased: {ConvertUuidToString(spatial_anchor.Uuid)}";
            }
        });
    }

    static string ConvertUuidToString(System.Guid guid)
    {
        var value = guid.ToByteArray();
        StringBuilder hex = new StringBuilder(value.Length * 2 + 4);
        for (int ii = 0; ii < value.Length; ++ii)
        {
            if (3 < ii && ii < 11 && ii % 2 == 0)
            {
                hex.Append("-");
            }

            hex.AppendFormat("{0:x2}", value[ii]);
        }

        return hex.ToString();
    }
}
