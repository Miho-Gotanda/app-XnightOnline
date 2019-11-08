using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core;

public class DeathBlowLight : MonoBehaviour
{
    private Light _light;

    private void Start()
    {
        _light = GetComponent<Light>();
    }
    private void Update()
    {
        if (_light.intensity <= 30)
        {
            _light.intensity += 1f;
        }
        else
            _light.intensity = 0f;
    }

}
