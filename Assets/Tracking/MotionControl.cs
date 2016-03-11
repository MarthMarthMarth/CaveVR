using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

	


// Editor
[CustomEditor(typeof(MotionControl))]
public class MotionEditor : Editor {

	public Dictionary<String, Motion> motions;
	public MotionControl motionControl; 

	void OnEnable() {
		motions = new Dictionary<String, Motion>();
		motionControl = (MotionControl)target;
	}

	public override void OnInspectorGUI() {

		DrawDefaultInspector ();

		foreach (KeyValuePair <string, Motion> entry in motions) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(entry.Key);
			if (GUILayout.Button("Play", GUILayout.Width(50))) {
				motionControl.animation = entry.Value;
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.Space();

		if(GUILayout.Button("New", GUILayout.Width(50))) {
			MotionBuilder.Show(this);
		}
	}
}

// Motion Builder Window
public class MotionBuilder : EditorWindow {
	
	MotionEditor editor;
	List<Orientation> record; 
	Motion recorded_motion;
	string name;
	bool recording;
	Transform startingTrans;

	void OnEnable() {
		recording = false;
		maxSize = new Vector2(225, 50);
	}

	void Update() {
		// Record the frames of motion
		if (recording) {
			Orientation frame = new Orientation();
			frame.pos = (editor.motionControl.startingPos - editor.motionControl.gameObject.transform.position);
			frame.rot = editor.motionControl.gameObject.transform.rotation;
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
				editor.motionControl.startingPos = editor.motionControl.gameObject.transform.position;
				recording = true;
			}
			// Allow Submission of a Recording
			if (!recording && record.Count > 0) {
				if (GUI.Button(new Rect(60, 28, 50, 18), "Submit")) {
					editor.motions.Add(name, recorded_motion);
					editor.motionControl.animation = null;
					this.Close();
				}
			}
		}
		else {
			// End Recording
			if (GUI.Button(new Rect(5, 28, 50, 18), "Stop")) {
				recording = false;
				recorded_motion = new Motion(record);
				recorded_motion.FixedAnimate(editor.motionControl);
			}
		}
		// Cancel Motion Creation
		if (GUI.Button(new Rect(170, 28, 50, 18), "Cancel")) {
			editor.motionControl.animation = null;
			this.Close();
		}
	}

	public static void Show(MotionEditor _editor) {
		MotionBuilder window = (MotionBuilder)EditorWindow.GetWindow(typeof(MotionBuilder));
		window.editor = _editor;
	}
}

