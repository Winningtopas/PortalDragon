using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Beam : MonoBehaviour
{
    private VisualEffect vfx;
    private VFXEventAttribute eventVFX;
    public float beamDuration = 2.0f;


    // Start is called before the first frame update
    void Start()
    {
        vfx = GetComponent<VisualEffect>();
        VFXEventAttribute eventVFX = vfx.CreateVFXEventAttribute();
        vfx.SendEvent("Start", eventVFX);
    }

    // Update is called once per frame
    void Update()
    {
        beamDuration -= Time.deltaTime;

        if (beamDuration <= 0.0f)
        {
            beamEnded();
        }
    }

    void beamEnded()
    {
        vfx.SendEvent("Stop", eventVFX);
    }
}
