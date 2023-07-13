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
    public TMP_Text debug_text_2;
    public GameObject main_scene;
    public GameObject calib_marker;
    public GameObject calib_system;
    public palm_menu Palm_menu;

    // anchor stuff
    private OVRSpatialAnchor main_anchor;
    Action<OVRSpatialAnchor.UnboundAnchor, bool> _onLoadAnchor;

 
    
    private void Awake()
    {
        _onLoadAnchor = OnLocalized;
    }
    
    void Start()
    {
        if (checkUuid())
        {
            Palm_menu.calibrated = true;
            calib_system.SetActive(false);

            // make uuid from stored string, use that to load anchor
            var main_uuid = new Guid(PlayerPrefs.GetString("main_uuid"));
            var uuids = new Guid[1];
            uuids[0] = main_uuid;

            Load(new OVRSpatialAnchor.LoadOptions
            {
                Timeout = 0,
                StorageLocation = OVRSpace.StorageLocation.Local,
                Uuids = uuids
            });
        }

    }

    void Update()
    {
    }

    public void onPressConfirm()
    {
        // spawn main scene @ calib marker position & (corrected) rotation
        main_scene.transform.position = calib_marker.transform.position;
        main_scene.transform.eulerAngles = new Vector3 (0, calib_marker.transform.eulerAngles.y, 0);

        // add spatial anchor component to anchor holder, store anchor in main_anchor
        main_scene.AddComponent<OVRSpatialAnchor>();
        main_anchor = main_scene.GetComponent<OVRSpatialAnchor>();

        // system management stuff
        Palm_menu.calibrated = true;
        main_scene.SetActive(true);
        calib_system.SetActive(false);

        StartCoroutine(waitThenSave(main_anchor));
    }

    public void onPressRedo()
    {   
        // system management stuff
        Palm_menu.calibrated = false;
        main_scene.SetActive(false);
        calib_system.SetActive(true);

        // get rid of anchor component
        Destroy(main_scene.GetComponent<OVRSpatialAnchor>());
        
        // erase anchor locally
        main_anchor.Erase((anchor, success) =>
        {
            if (!success)
            {
                debug_text.text = $"anchor erase failed";
                return;
            }
            else
            {
                debug_text.text = $"anchor erased: {ConvertUuidToString(main_anchor.Uuid)}";
            }

            // erase anchor from player prefs (persistent)
            PlayerPrefs.DeleteKey("main_uuid");
        });
    }

    private IEnumerator waitThenSave(OVRSpatialAnchor spatial_anchor)
    {
        yield return new WaitForSeconds(1.0f);
        
        // save anchor locally
        spatial_anchor.Save((anchor, success) =>
        {
            if (!success)
            {
                debug_text.text = "anchor save failed";
            }
            else
            {
                debug_text.text = $"anchor saved: {ConvertUuidToString(anchor.Uuid)}";
            }

            // save anchor to player prefs (persistent)
            PlayerPrefs.SetString("main_uuid", anchor.Uuid.ToString());
        });
    }

    private void Load(OVRSpatialAnchor.LoadOptions options) => OVRSpatialAnchor.LoadUnboundAnchors(options, anchors =>
    {
        if (anchors == null)
        {
            debug_text.text = "Query failed.";
            return;
        }

        foreach (var anchor in anchors)
        {
            if (anchor.Localized)
            {
                _onLoadAnchor(anchor, true);
            }
            else if (!anchor.Localizing)
            {
                anchor.Localize(_onLoadAnchor);
            }
        }
    });

    private void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
    {
        if (!success)
        {
            debug_text.text = $"{unboundAnchor} Localization failed!";
            return;
        }

        var pose = unboundAnchor.Pose;

        main_scene.transform.position = pose.position;
        main_scene.transform.rotation = pose.rotation;
        main_scene.SetActive(true);

        main_scene.AddComponent<OVRSpatialAnchor>();
        main_anchor = main_scene.GetComponent<OVRSpatialAnchor>();

        unboundAnchor.BindTo(main_anchor);
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

    public bool checkUuid()
    {
        if (PlayerPrefs.HasKey("main_uuid"))
        {
            debug_text_2.text = $"uuid exists: {PlayerPrefs.GetString("main_uuid")}";
            return true;
        }
        else
        {
            debug_text_2.text = "no main_uuid exists";
            return false;
        }
    }
}
