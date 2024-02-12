using System;
using UnityEngine;

[Serializable] 
public class CarAIParameters
{
    public bool sleepUntilPlayerDetect;

    public float minChaseSpotLerpSpeed;
    public float maxChaseSpotLerpSpeed;
    public float minChaseSpotOffset;
    public float maxChaseSpotOffset;
    public float lerpSpeedForChaseSpotSpeedLerp;

    public float carSpeedAddedValue;
    public float maxDistanceToAddSpeedValue;
}
