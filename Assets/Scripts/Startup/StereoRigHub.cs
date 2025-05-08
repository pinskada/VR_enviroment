using UnityEngine;
using System;
using System.Collections.Generic;

public class StereoRigHub : MonoBehaviour
{
    public static event Action<StereoCameraProjection> OnStereoRigReady;
    public static event Action<IPDConfig> OnIPDconfigReady;
    private static StereoCameraProjection cachedProjection;
    private static IPDConfig cachedIPD;

    private static List<StereoCameraProjection> cachedProjections = new List<StereoCameraProjection>();

    public static void InvokeReadyProjection(StereoCameraProjection projection)
    {
        if (!cachedProjections.Contains(projection))
            cachedProjections.Add(projection);

        OnStereoRigReady?.Invoke(projection);
    }

    public static List<StereoCameraProjection> GetAllProjections()
    {
        return cachedProjections;
    }

    public static void InvokeReadyIPD(IPDConfig ipd) 
    {
        cachedIPD = ipd;
        OnIPDconfigReady?.Invoke(ipd);
    }

    public static StereoCameraProjection GetCurrentProjection()
    {
        return cachedProjection;
    }
    public static IPDConfig GetCurrentIPD()
    {
        return cachedIPD;
    }
}
