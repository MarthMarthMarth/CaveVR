using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	public Tracker tracker;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (tracker.objs.ContainsKey("Wand")) {
			Tracker_Obj wand = tracker.objs["Wand"];
			transform.position = new Vector3(10 * wand.x, 10 * wand.y, 10 * wand.z);
			transform.rotation = Quaternion.Euler(wand.h, wand.p, wand.r);
		}
	}
}
