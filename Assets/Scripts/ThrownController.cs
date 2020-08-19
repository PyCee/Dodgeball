using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]
public class ThrownController : MonoBehaviour
{
	private ThrowPath throwPath;
	public void SetThrowPath(ThrowPath t){throwPath = t;}

	private Rigidbody rb;
	private Vector3 velocity;
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.isKinematic = true;
	}
    void Update()
    {
		Vector3 calculatedPosition = throwPath.UpdatePosition(Time.deltaTime);
		velocity = (calculatedPosition - transform.position) / Time.deltaTime;
		transform.position = calculatedPosition;
		if(throwPath.IsFinished()){
			Release();
		}
	}
	void OnCollisionEnter(Collision collision)
	{
		Release();
	}
	private void Release()
	{
		rb.isKinematic = false;
		rb.AddForce(velocity, ForceMode.Impulse);
		Destroy(this);
	}
}
