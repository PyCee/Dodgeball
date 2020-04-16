using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShooterController : MonoBehaviour
{
	public float throwSpeed;
	public GameObject prefab;
	private ArrayList throwables = new ArrayList();
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
			GameObject throwable = Instantiate(prefab, transform.position, Quaternion.identity);
			throwables.Add(throwable);
			Rigidbody rb = throwable.GetComponent<Rigidbody>();
			rb.AddForce(transform.forward * throwSpeed);
		}
		if(Input.GetMouseButtonDown(1)){
			foreach(GameObject obj in throwables){
				Destroy(obj);
			}
			throwables.Clear();
		}
    }
}
