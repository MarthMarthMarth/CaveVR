using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

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

[System.Serializable]
public class Motion {
	public string name;
	public List<Orientation> orientations;

	public Motion (string _name, List<Orientation> _orientations) {
		name = _name;
		orientations = _orientations;
	}
}
	
public class MotionControl : MonoBehaviour {

	private int frame;
	private bool loop;
	private List<List<Orientation>> motions;
	private Orientation starting;

	public void Start() {
		string path = Application.dataPath + "/Resources";
		//string[] filenames = Directory.GetFiles("", "*.path.json");
		//for (int i = 0; i < filenames.Length; i++)
		//	Debug.Log(filenames[i]);
	}

	public IEnumerator Move(List<Orientation> _motion, bool _loop = false) {
		frame = 0;
		loop = _loop;
		/*motion = _motion;
		starting = new Orientation(gameObject.transform);

		while (motion != null) {
			gameObject.transform.position = motion[frame].pos;
			gameObject.transform.rotation = motion[frame].rot;

			frame += 1 % motion.Count;
			if (frame == 0 && !loop)
				motion = null;*/
			yield return null;
		//}
	}
}

// Editor
[CustomEditor(typeof(MotionControl))]
public class MotionEditor : Editor {

	public MotionControl motionControl;
	public _LevelData levelData;
	public string test;

	void OnEnable() {
		levelData = (_LevelData)Resources.Load("_LevelData"); 
		motionControl = (MotionControl)target;
	}

	public override void OnInspectorGUI() {
		 
		DrawDefaultInspector ();

		/*foreach (KeyValuePair <string, List<Orientation>> entry in motions) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(entry.Key);
			if (GUILayout.Button("Play", GUILayout.Width(50))) {
				motionControl.Move(entry.Value);
			}
			EditorGUILayout.EndHorizontal();
		}*/

		EditorGUILayout.Space();

		if(GUILayout.Button("New", GUILayout.Width(50))) {
			MotionBuilder.Show(this);
		}
	}
}

// Motion Builder Window
public class MotionBuilder : EditorWindow {
	
	MotionEditor editor;
	_LevelData levelData;
	List<Orientation> record; 
	string name;
	bool recording;
	Orientation startingOrientation;

	void OnEnable() {
		recording = false;
		maxSize = new Vector2(225, 50);
		levelData = (_LevelData)Resources.Load("_LevelData");
		Debug.Log(levelData.Motions().Count);
		for (int i = 0; i < levelData.Motions().Count; i++)
			Debug.Log(levelData.Motions()[i].name);
		//levelData.Initialize();
	}

	void Update() {

		if (!EditorApplication.isPlaying) {
			Debug.Log("running");
			Close();
		}

		// Record the frames of motion
		if (recording) {
			Orientation frame = new Orientation();
			frame.pos = startingOrientation.pos - editor.motionControl.gameObject.transform.position;
			frame.rot = Quaternion.FromToRotation(startingOrientation.rot * Vector3.forward, editor.motionControl.gameObject.transform.forward);
			record.Add(frame);
		}
	}

	void OnGUI() {
		GUI.Label(new Rect(5, 4, 80, 18), "Enter a name:");

		// Assign name from text field
		name = EditorGUI.TextField(new Rect(85, 4, 125, 15), name);
	
		if (!recording) {
			// Begin Recording
			if (GUI.Button(new Rect(5, 28, 50, 18), "Record")) {
				record = new List<Orientation>();
				startingOrientation = new Orientation(editor.motionControl.gameObject.transform);
				recording = true;
			}
			// Allow Submission of a Recording
			if (!recording && record != null && record.Count > 0) {
				if (GUI.Button(new Rect(60, 28, 50, 18), "Submit")) {
					
					// http://answers.unity3d.com/questions/753058/get-a-json-string-from-a-simplejson-node.html
					/*JSONClass JSON_root = new JSONClass();
					JSONArray JSON_record = new JSONArray();
					JSON_root.Add("name", new JSONData(name));
					JSON_root.Add("record", JSON_record);

					for (int i = 0; i < record.Count; i++) {
						JSONArray JSON_orientation = new JSONArray();
						JSON_orientation.Add( new JSONData(record[i].pos.x) );
						JSON_orientation.Add( new JSONData(record[i].pos.y) );
						JSON_orientation.Add( new JSONData(record[i].pos.z) );
						JSON_orientation.Add( new JSONData(record[i].rot.x) );
						JSON_orientation.Add( new JSONData(record[i].rot.y) );
						JSON_orientation.Add( new JSONData(record[i].rot.z) );
						JSON_record.Add( JSON_orientation );
					}*/

					/*string path = Application.dataPath + "/Resources/" + name + "path.json";
					if (!File.Exists(path)) {
						File.WriteAllText(path, JSON_root.ToString());
						Debug.Log("write");
					}
					else {
						Debug.Log("No Write");
					}*/
				
					levelData.AddMotion(new Motion(name, record));
					//levelData.Test();
					//EditorUtility.SetDirty(editor.motionControl);
					this.Close();
				}
			}
		}
		else {
			// End Recording
			if (GUI.Button(new Rect(5, 28, 50, 18), "Stop")) {
				recording = false;
				editor.motionControl.Move(record);
			}
		}
		// Cancel Motion Creation
		if (GUI.Button(new Rect(170, 28, 50, 18), "Cancel")) {
			this.Close();
		}
	}

	public static void Show(MotionEditor _editor) {
		MotionBuilder window = (MotionBuilder)EditorWindow.GetWindow(typeof(MotionBuilder));
		window.editor = _editor;
	}
}

