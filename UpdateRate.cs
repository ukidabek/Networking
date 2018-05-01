using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

[SerializeField]
public class UpdateRate
{
    [SerializeField] private int _frequency = 60;
    public float Interval { get { return 1f / _frequency; } }
}
