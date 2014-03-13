using UnityEngine;
using System.Collections;

/*
[System.Obsolete("Not used anymore",true)]
public class TestKZPref : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log ("hi");
		TestAndroid();
		TestPC ();
	}
	private void TestPC() {
		//KZConfigManager config=new KZConfigManager(Application.dataPath+"/test.config");
		KZPref config=new KZPref(KZPref.GetExternalDataPath()+"/test.config");
		
		config.Add("name", "ken");
		config.Add("age", 29);
		config.Add("height", 1.2f);
		config.Save();
		
		Debug.Log (config.GetString("name"));
		Debug.Log (config.GetInt("age"));
		Debug.Log (config.GetFloat("height"));
		//Debug.Log (prop.GetFloat("age"));
		//Debug.Log (prop.GetInt("height"));
		//Debug.Log (KZPropertyManager.GetFloat("sex"));
	}
	private void TestAndroid() {
		Debug.Log ("ken persistent: "+Application.persistentDataPath);
		Debug.Log ("ken: "+Application.dataPath);
		Debug.Log ("ken: external "+KZPref.GetExternalDataPath());
		//KZPropertyManager prop=new KZPropertyManager(Application.persistentDataPath);
	}
}
*/
