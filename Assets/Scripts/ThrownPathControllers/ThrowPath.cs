using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
public abstract class ThrowPath : System.Object
{
    public float duration = -1.0f;

    private float progress = 0.0f;
    public bool IsFinished(){return progress == 1.0f;}

    private static float estimateStep = 1f / 1000f;

    public void CalculateDuration(float speed)
    {
        duration = EstimatePathLength() / speed;
    }

    public Vector3 UpdatePosition(float deltaT)
    {
        Assert.IsTrue(duration > 0.0f);
		progress += deltaT / duration;
		progress = Mathf.Max(0.0f, Mathf.Min(1.0f, progress));
		return CalculatePosition(progress);
    }
    private float EstimatePathLength()
    {
        float lengthSum = 0.0f;
        Vector3 lastPosition = CalculatePosition(0.0f);
        for(float t = estimateStep; t <= 1.0f; t += estimateStep){
            Vector3 newPosition = CalculatePosition(t);
            lengthSum += (newPosition - lastPosition).magnitude;
            lastPosition = newPosition;
        }
        return lengthSum;
    }
    
    protected abstract Vector3 CalculatePosition(float t);
}
