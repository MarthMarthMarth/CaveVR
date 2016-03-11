using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Orientation {
	public Vector3 pos;
	public Quaternion rot;
	public Orientation() {
		pos = new Vector3();
		rot = Quaternion.identity;
	}
	public Orientation(Vector3 _pos, Quaternion _rot) {
		pos = _pos;
		rot = _rot;
	}
	public Orientation(Transform transform) {
		pos = transform.position;
		rot = transform.rotation;
	}
}

// Data Classes
public class Motion {
	private Orientation starting;
	private List<Orientation> fixed_motion;
	private List<Orientation> relative_motion;
	private int frame;
	private bool loop;
	private bool moving; 

	public Motion(List<Orientation> _fixed_motion, bool _loop = false) {
		frame = 0;
		moving = false;
		loop = _loop;
		fixed_motion = _fixed_motion;

		relative_motion = new List<Orientation>();
		relative_motion.Add(new Orientation());
		for (int i = 1; i < fixed_motion.Count; i++) {
			Vector3 new_pos = fixed_motion[i].pos - fixed_motion[0].pos;
			Quaternion new_rot = Quaternion.Inverse(fixed_motion[0].rot) * fixed_motion[i].rot;
			relative_motion.Add(new Quaternion(new_pos, new_rot));
		}
	}
		
	public IEnumerator FixedAnimate(GameObject target) {
		if (moving)
			return;
		moving = true;

		while (moving) {
			target.transform.position = fixed_motion[frame].pos;
			target.transform.rotation = fixed_motion[frame].rot;

			frame += 1 % fixed_motion.Count;
			if (frame == 0 && !loop)
				moving = false;
			yield return null;
		}

	}
}