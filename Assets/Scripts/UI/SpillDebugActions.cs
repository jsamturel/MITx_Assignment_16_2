using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class SpillDebugActions : MonoBehaviour
{
    public void BumpSpill(float amount)
    {
        FindObjectOfType<SpillMeterUI>()?.AddSpill(amount);
    }
}

