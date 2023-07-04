using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class floor_finder : MonoBehaviour
{
    public GameObject calib_marker;
    public TMP_Text debug_text;
    public GameObject left_hand;
    public GameObject right_hand;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        debug_text.text = $"L: {left_hand.transform.position}\nR: {right_hand.transform.position}";
    }
}
