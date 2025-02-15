using System;
using UnityEngine;
using System.Collections;

public class BillBoardWorldCanvas : MonoBehaviour 
{
	private void Update()
	{
		this.transform.rotation = Quaternion.Slerp(this.transform.rotation,Camera.main.transform.rotation,5f*Time.deltaTime);
	}
}