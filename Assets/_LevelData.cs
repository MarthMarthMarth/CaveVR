using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class _LevelData : ScriptableObject {
	[SerializeField]
	List<Motion> motions;
	[SerializeField]
	public int active_index;

	[MenuItem("Assets/Create/_LevelData")]
	public static void CreateAsset() {
		ScriptableObjectUtility.CreateAsset<_LevelData>();
	}

	public void Initialize() {
		if (motions == null) {
			active_index = -1;
			motions = new List<Motion>();
		}
	}

	public void AddMotion(Motion _motion) {
		motions.Add(_motion);
		Save();
	}

	public void SetActive(int _active_index) {
		active_index = _active_index;
		Save();
	}

	public void Save() {
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();
	}

	public List<Motion> Motions() {
		return motions;
	}
}

