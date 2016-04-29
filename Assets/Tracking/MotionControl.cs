using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

[System.Serializable]
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
	public bool loop;
	public bool reset;

	public Motion() {
		loop = reset = true;
		name = "";
		orientations = new List<Orientation>();
	}

	public Motion(string _name) {
		loop = reset = true;
		name = _name;
		orientations = new List<Orientation>();
	}

	public Motion (string _name, List<Orientation> _orientations) {
		loop = reset = true;
		name = _name;
		orientations = _orientations;
	}
}

[ExecuteInEditMode]
public class MotionControl : MonoBehaviour {

	private int frame;
	private Motion motion;
	private Orientation starting;

	public void Start() {
		frame = 0;
		string path = Application.dataPath + "/Resources";
		#if UNITY_EDITOR
			EditorApplication.update += Update;
		#endif
	}

	void OnDestroy() {
		#if UNITY_EDITOR
		EditorApplication.update -= Update;
		#endif
	}

	public void Update() {
		if (motion != null) {
			gameObject.transform.position = starting.pos - motion.orientations[frame].pos;
			gameObject.transform.rotation = starting.rot * motion.orientations[frame].rot;

			frame += 1;
			frame %= motion.orientations.Count;
			if (frame == 0 && !motion.loop)
				Stop();
		}
	}

	public void Move(Motion _motion) {
		frame = 0;
		motion = _motion;
		starting = new Orientation(gameObject.transform);
	}

	public void Stop() {
		motion = null;
	}
			
}

// Editor
[CustomEditor(typeof(MotionControl))]
public class MotionEditor : Editor {

	public MotionControl motionControl;
	public _LevelData levelData;
	public string test;
	public int active_index;
	public bool loop;
	public bool reset;

	void OnEnable() {
		string path = "Assets/Resources/_LevelData.asset";
		levelData = (_LevelData)AssetDatabase.LoadAssetAtPath(path, typeof(_LevelData));
		levelData.Initialize();
		motionControl = (MotionControl)target;
	}

	public override void OnInspectorGUI() {
		 
		DrawDefaultInspector ();
		EditorGUILayout.Space();

		for (int i = 0; i < levelData.Motions().Count; i++) {

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(levelData.Motions()[i].name, EditorStyles.boldLabel);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Length: " + levelData.Motions()[i].orientations.Count + " frames");
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			levelData.Motions()[i].loop = EditorGUILayout.ToggleLeft("  Loop", levelData.Motions()[i].loop);
			//reset = EditorGUILayout.ToggleLeft("  Reset", reset);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();

			if (levelData.active_index != i) {
				if(GUILayout.Button("Play", GUILayout.Width(50))) {
					motionControl.Move(levelData.Motions()[i]);
					levelData.active_index = i;
				}
			}
			else {
				if(GUILayout.Button("Stop", GUILayout.Width(50))) {
					motionControl.Stop();
					levelData.active_index = -1;
				}
			}
			if(GUILayout.Button("Trim", GUILayout.Width(50))) {
				MotionTrimmer.Show(this);
			}
			if(GUILayout.Button("Delete", GUILayout.Width(50))) {
				levelData.Motions().RemoveAt(i);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(15);
		}

		if(GUILayout.Button("New", GUILayout.Width(50))) {
			MotionBuilder.Show(this);
		}
	}
}

// Motion Builder Window
public class MotionBuilder : EditorWindow {
	
	MotionEditor editor;
	_LevelData levelData;

	Orientation startingOrientation;
	Motion motion; 
	string name_field;
	bool isRecording;

	void OnEnable() {
		isRecording = false;
		maxSize = new Vector2(225, 50);
		levelData = (_LevelData)Resources.Load("_LevelData");
		levelData.Initialize();
	}

	void Update() {

		/*(if (!EditorApplication.isPlaying) {
			Debug.Log("running");
			Close();
		}*/

		// Record the frames of motion
		if (isRecording) {
			Orientation frame = new Orientation();
			frame.pos = startingOrientation.pos - editor.motionControl.gameObject.transform.position;
			frame.rot = Quaternion.FromToRotation(startingOrientation.rot * Vector3.forward, editor.motionControl.gameObject.transform.forward);
			motion.orientations.Add(frame);
		}
	}

	void OnGUI() {
		GUI.Label(new Rect(5, 4, 80, 18), "Enter a name:");

		// Assign name from text field
		name_field = EditorGUI.TextField(new Rect(85, 4, 125, 15), name_field);
	
		if (!isRecording) {
			// Begin isRecording
			if (GUI.Button(new Rect(5, 28, 50, 18), "Record")) {
				motion = new Motion();
				startingOrientation = new Orientation(editor.motionControl.gameObject.transform);
				isRecording = true;
			}
			// Allow Submission of a isRecording
			if (!isRecording && motion != null && motion.orientations.Count > 0) {
				if (GUI.Button(new Rect(60, 28, 50, 18), "Submit")) {
					motion.name = name_field;
					levelData.AddMotion(motion);
					editor.motionControl.Stop();
					this.Close();
				}
			}
		}
		else {
			// End isRecording
			if (GUI.Button(new Rect(5, 28, 50, 18), "Stop")) {
				isRecording = false;
				editor.motionControl.Move(motion);
			}
		}
		// Cancel Motion Creation
		if (GUI.Button(new Rect(170, 28, 50, 18), "Cancel")) {
			editor.motionControl.Stop();
			this.Close();
		}
	}

	public static void Show(MotionEditor _editor) {
		MotionBuilder window = (MotionBuilder)EditorWindow.GetWindow(typeof(MotionBuilder));
		window.editor = _editor;
	}
}

// Motion Trimmer Window
public class MotionTrimmer : EditorWindow {

	/*MotionEditor editor;
	_LevelData levelData;

	Orientation startingOrientation;
	Motion motion; 
	string name_field;
	bool isRecording;

	void OnEnable() {
		isRecording = false;
		maxSize = new Vector2(225, 50);
		levelData = (_LevelData)Resources.Load("_LevelData");
		levelData.Initialize();
	}

	void Update() {

		if (!EditorApplication.isPlaying) {
			Debug.Log("running");
			Close();
		}

		// Record the frames of motion
		if (isRecording) {
			Orientation frame = new Orientation();
			frame.pos = startingOrientation.pos - editor.motionControl.gameObject.transform.position;
			frame.rot = Quaternion.FromToRotation(startingOrientation.rot * Vector3.forward, editor.motionControl.gameObject.transform.forward);
			motion.orientations.Add(frame);
		}
	}

	void OnGUI() {
		GUI.Label(new Rect(5, 4, 80, 18), "Starting Frame:");
		name_field = EditorGUI.TextField(new Rect(85, 4, 125, 15), name_field);

		if (!isRecording) {
			// Begin isRecording
			if (GUI.Button(new Rect(5, 28, 50, 18), "Record")) {
				motion = new Motion();
				startingOrientation = new Orientation(editor.motionControl.gameObject.transform);
				isRecording = true;
			}
			// Allow Submission of a isRecording
			if (!isRecording && motion != null && motion.orientations.Count > 0) {
				if (GUI.Button(new Rect(60, 28, 50, 18), "Submit")) {
					motion.name = name_field;
					levelData.AddMotion(motion);
					editor.motionControl.Stop();
					this.Close();
				}
			}
		}
		else {
			// End isRecording
			if (GUI.Button(new Rect(5, 28, 50, 18), "Stop")) {
				isRecording = false;
				editor.motionControl.Move(motion);
			}
		}
		// Cancel Motion Creation
		if (GUI.Button(new Rect(170, 28, 50, 18), "Cancel")) {
			editor.motionControl.Stop();
			this.Close();
		}
	}*/

	public static void Show(MotionEditor _editor) {
		MotionBuilder window = (MotionBuilder)EditorWindow.GetWindow(typeof(MotionBuilder));
		//window.editor = _editor;
	}
}