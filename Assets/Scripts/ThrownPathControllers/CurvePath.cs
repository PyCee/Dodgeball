using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
public class CurvePath : ThrowPath
{
	// TODO: implement curve from various directions
    // https://en.wikipedia.org/wiki/B%C3%A9zier_curve
    // base time on length of curve and speed
	private Vector3 beginningPoint;
	private Vector3 controlPoint;
	private Vector3 endPoint;

	public CurvePath(Vector3 beginningPoint, Vector3 controlPoint, 
			Vector3 endPoint, float speed){

		Assert.IsTrue(speed > 0.0f);

		this.beginningPoint = beginningPoint;
		this.controlPoint = controlPoint;
		this.endPoint = endPoint;
		
		CalculateDuration(speed);
	}
	protected override Vector3 CalculatePosition(float t){
		Assert.IsTrue(t >= 0.0f);
		Assert.IsTrue(t <= 1.0f);

		Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
		position += (1.0f - t) * (1.0f - t) * beginningPoint;
		position += 2.0f * (1.0f - t) * t * controlPoint;
		position += t * t * endPoint;
		return position;
	}
}
