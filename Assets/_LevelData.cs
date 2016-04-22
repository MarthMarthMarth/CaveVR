using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class _LevelData : ScriptableObject {
	[SerializeField]
	List<Motion> motions;

	[MenuItem("Assets/Create/_LevelData")]
	public static void CreateAsset() {
		ScriptableObjectUtility.CreateAsset<_LevelData>();
	}

	public void Initialize() {
		if (motions == null)
			motions = new List<Motion>();
	}

	public void AddMotion(Motion _motion) {
		motions.Add(_motion);
		EditorUtility.SetDirty(this);
		AssetDatabase.SaveAssets();
		Debug.Log(motions.Count);
	}

	public List<Motion> Motions() {
		return motions;
	}
}

