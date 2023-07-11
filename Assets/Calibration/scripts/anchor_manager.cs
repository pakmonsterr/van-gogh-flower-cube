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
    public OVRSpatialAnchor anchor_prefab;
    Action<OVRSpatialAnchor.UnboundAnchor, bool> _onLoadAnchor;
    
    private void Awake()
    {
        _onLoadAnchor = OnLocalized;
    }
    
    void Start()
    {
        spatial_anchor = main_anchor.GetComponent<OVRSpatialAnchor>();

        //PlayerPrefs.DeleteAll();
        if (checkUuid())
        {
            Palm_menu.calibrated = true;
        }

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

        /*var uA = new OVRSpatialAnchor.UnboundAnchor();
        uA = spatial_anchor;
        var pose = uA.Pose;
        debug_text.text = $"{pose.position}";*/
    }

    void Update()
    {
    }

    public void onPressConfirm()
    {
        // spawn main scene @ calib marker position & (corrected) rotation
        main_scene.transform.position = calib_marker.transform.position;
        main_scene.transform.rotation = calib_marker_rotation.transform.rotation;

        // system management stuff
        Palm_menu.calibrated = true;
        main_scene.SetActive(true);
        calib_system.SetActive(false);

        // save anchor locally
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

            // save anchor to player prefs (persistent)
            PlayerPrefs.SetString("main_uuid", anchor.Uuid.ToString());
        });
    }

    public void onPressRedo()
    {   
        // system management stuff
        Palm_menu.calibrated = false;
        main_scene.SetActive(false);
        calib_system.SetActive(true);
        
        // erase anchor locally
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

            // erase anchor from player prefs (persistent)
            PlayerPrefs.DeleteKey("main_uuid");

            checkUuid();
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
        var spatialAnchor = Instantiate(anchor_prefab, pose.position, pose.rotation);
        unboundAnchor.BindTo(spatialAnchor);

        if (spatialAnchor.TryGetComponent<Anchor>(out var anchor))
        {
            // We just loaded it, so we know it exists in persistent storage.
            anchor.ShowSaveIcon = true;
        }
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

    private bool checkUuid()
    {
        if (PlayerPrefs.HasKey("main_uuid"))
        {
            debug_text.text = $"uuid exists: {PlayerPrefs.GetString("main_uuid")}";
            return true;
        }
        else
        {
            debug_text.text = "no main_uuid exists";
            return false;
        }
    }

    private void checkAnchor()
    {
        if (spatial_anchor)
        {
            debug_text.text = "anchor already exists";
            return;
        }
        debug_text.text = "no existing anchor";
    }
}
