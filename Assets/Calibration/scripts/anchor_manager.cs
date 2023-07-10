using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class anchor_manager : MonoBehaviour
{
    public TMP_Text debug_text;
    public GameObject main_scene;
    public GameObject calib_marker;
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
        main_scene.transform.position = calib_marker.transform.position;
        main_scene.transform.eulerAngles = new Vector3 (0, calib_marker.transform.eulerAngles.y, 0);
        main_scene.SetActive(true);
        calib_system.SetActive(false);

        Palm_menu.calibrated = true;
    }

    public void onPressRedo()
    {
        main_scene.SetActive(false);
        calib_system.SetActive(true);

        Palm_menu.calibrated = false;
    }
}
