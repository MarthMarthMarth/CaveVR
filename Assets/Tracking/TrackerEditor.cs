using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using SimpleJSON;

[CustomEditor(typeof(Tracker))]
public class TrackerEditor : Editor {
	Tracker tracker;
	bool[] foldouts;

	void OnEnable() {
		tracker = (Tracker)target;
		foldouts = new bool[tracker.serialized_objs.Count];
		//for (int i = 0; i < tracker.serialized_objs.Count; i++)
		//	foldouts[i] = true;
	}

	public override void OnInspectorGUI() {

		for (int i = 0; i < foldouts.Length; i++) {
			foldouts[i] = EditorGUILayout.Foldout(foldouts[i], tracker.serialized_objs[i].id);
			if (foldouts[i]) {
				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField("X: ", GUILayout.Width(25));
				GUI.enabled = false;
				EditorGUILayout.FloatField(tracker.serialized_objs[i].x, GUILayout.Width(75));
				GUI.enabled = true;

				EditorGUILayout.LabelField("Y: ", GUILayout.Width(25));
				GUI.enabled = false;
				EditorGUILayout.FloatField(tracker.serialized_objs[i].y, GUILayout.Width(75));
				GUI.enabled = true;

				EditorGUILayout.LabelField("Z: ", GUILayout.Width(25));
				GUI.enabled = false;
				EditorGUILayout.FloatField(tracker.serialized_objs[i].z, GUILayout.Width(75));
				GUI.enabled = true;

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();

				EditorGUILayout.LabelField("H: ", GUILayout.Width(25));
				GUI.enabled = false;
				EditorGUILayout.FloatField(tracker.serialized_objs[i].h, GUILayout.Width(75));
				GUI.enabled = true;

				EditorGUILayout.LabelField("P: ", GUILayout.Width(25));
				GUI.enabled = false;
				EditorGUILayout.FloatField(tracker.serialized_objs[i].p, GUILayout.Width(75));
				GUI.enabled = true;

				EditorGUILayout.LabelField("R: ", GUILayout.Width(25));
				GUI.enabled = false;
				EditorGUILayout.FloatField(tracker.serialized_objs[i].r, GUILayout.Width(75));
				GUI.enabled = true;

				EditorGUILayout.EndHorizontal();
			}
		}
	}
}