using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class palm_menu : MonoBehaviour
{
    public TMP_Text debug_text;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        debug_text.text = "left palm down";
    }

    public void onLeftPalmUp()
    {
        debug_text.text = "left palm up";
    }
}
