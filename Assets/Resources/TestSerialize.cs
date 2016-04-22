using UnityEngine;
using System.Collections;
using UnityEditor;

public class TestSerialize : MonoBehaviour {

	// Use this for initialization
	void Start () {
		string path = "Assets/Resources/_LevelData.asset";
		_LevelData testData = (_LevelData)AssetDatabase.LoadAssetAtPath(path, typeof(_LevelData));
		if (testData == null) {
			Debug.Log("CREATE NEW");
			testData = ScriptableObject.CreateInstance<_LevelData> ();
			AssetDatabase.CreateAsset(testData, path);
		}
		else {
			Debug.Log("LOADED EXISTING");
		}
		testData.Initialize();
		EditorUtility.SetDirty(testData);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
