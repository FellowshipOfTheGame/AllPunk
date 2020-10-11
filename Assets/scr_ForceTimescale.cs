using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scr_ForceTimescale : MonoBehaviour
{
    public float timescale;

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timescale;
    }
}
