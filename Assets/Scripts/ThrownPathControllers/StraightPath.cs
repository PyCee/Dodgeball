using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
public class StraightPath : ThrowPath
{
	private Vector3 beginningPoint;
	private Vector3 endPoint;

	public StraightPath(Vector3 beginningPoint, Vector3 endPoint, float speed){

		Assert.IsTrue(speed > 0.0f);

		this.beginningPoint = beginningPoint;
		this.endPoint = endPoint;

		CalculateDuration(speed);
	}
	protected override Vector3 CalculatePosition(float t){
		Assert.IsTrue(t >= 0.0f);
		Assert.IsTrue(t <= 1.0f);

		Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
		position += (1.0f - t) * beginningPoint;
		position += t * endPoint;
		return position;
	}
}
