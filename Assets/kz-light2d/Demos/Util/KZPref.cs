using UnityEngine;
using System.Collections;

// A manager for accessing key/value pairs of config files.
// The config files are in txt format, should be easily
// readable and modifiable. 
//
// File format:
//   A config file is a list of key/value pairs, e.g. 
//     xOffset = 23
//     yOffset = 12.2
//     name = ken
//     version = "3.1.2"
//     greeting = "hello, world"
//     ...
//   Note:
//     * value types: int, float, string. 
//     * Each line contains only one pair. 
//     * Whitespaces are allowed within and between lines.
//     * Strings with whtespaces should be surrounded with double quotes. 
//     * Floats are numbers with a dot(.). The rest are integers.
//     * Comments start with a '#'

// 2013.3.13  ken  initial version

/*

[System.Obsolete("Not used anymore",true)]
public class KZPref {
	private static string separator=" = ";
	private static string newLine="\n";
	private static string commentStr="#";
	
	private string dataPath=null;
	private System.Collections.Generic.Dictionary<string, object> properties;
	
//	private static APPropertyManager manager=new APPropertyManager();
//	public static APPropertyManager GetInstance() { return manager; }
	
	public KZPref(string path) {
		dataPath=path;
		properties=new System.Collections.Generic.Dictionary<string, object>();
		if( ! System.IO.File.Exists(path)) {
			System.IO.StreamWriter writer=new System.IO.StreamWriter(path);
			writer.Close();
		} else {
			Load(path);
		}
	}
	
	public void Add(string key, string val) {
		Add(key, (object)val);
	}
	public void Add(string key, float val) {
		Add(key, (object)val);
	}
	public void Add(string key, int val) {
		Add(key, (object)val);
	}
	private void Add(string key, object val) {
		if(properties.ContainsKey(key)) {
			//throw new System.Exception("key already exists: "+key+": "+properties[key]);
			if(val != properties[key]) {
				//Debug.LogWarning("key/value pair already exists. updating with new value: "+key+": "+properties[key]+" -> "+val); //>>>?
				Set (key, val);
			} else {
				//Debug.LogWarning("key/value pair already exists with the same value");
			}
		} else {
			properties.Add (key, val);
		}
	}
	
	//[ setters
	public void Set(string key, string val) {
		Set (key, (object)val);
	}
	public void Set(string key, float val) {
		Set (key, (object)val);
	}
	public void Set(string key, int val) {
		Set (key, (object)val);
	}
	private void Set(string key, object val) {
		properties[key]=val;
	}
	
	//[ getters
	public object Get(string key) {
		return properties[key];
	}
	public int GetInt(string key) {
		return (int)Get(key);
	}
	public float GetFloat(string key) {
		return (float)Get(key);
	}
	public string GetString(string key) {
		return Get(key).ToString();
	}
	public bool Contains(string key) {
		return properties.ContainsKey(key);
	}
	
	public void Remove(string key) {
		properties.Remove(key);
	}
	public void RemoveAll() {
		properties.Clear();
	}
	
	public void Save() { 
		//>>> warning: file exists
		System.Text.StringBuilder content=new System.Text.StringBuilder();
		foreach(var pair in properties) {
			content.Append(pair.Key).Append(separator).Append(pair.Value).Append(newLine);
		}
		System.IO.File.WriteAllText(dataPath, content.ToString());
	}
	public void Load(string path) {
		RemoveAll();
		
		System.IO.StreamReader reader=new System.IO.StreamReader(path);
		string line;
		int lineNumber=1;
		while((line = reader.ReadLine()) != null) {
		    line=line.Trim();
			if(string.IsNullOrEmpty(line)) continue; //empty line
			
			if(line.StartsWith(commentStr)) {
				continue;
			}
			
			string[] pairString=line.Split(
			    new string[] {separator}, 
			    System.StringSplitOptions.RemoveEmptyEntries
			);	
			if(pairString.Length == 1 || pairString.Length>2) { //a key or value is missing
				throw new System.Exception("parse error: invalid pair at line "+lineNumber+": \""+line+"\"");
			}
			string keyStr=pairString[0].Trim();
			string valStr=pairString[1].Trim();
			if(string.IsNullOrEmpty(keyStr)) {
				continue;
			}
			if(string.IsNullOrEmpty(valStr)) {
				throw new System.Exception("parse error: no value specified at line "+lineNumber+": \""+line+"\"");
			}
			
			if( ! DigitsOrDotOnly(valStr)) { //"1.1" is a string
				if(valStr[0]=='"' && valStr[-1]=='"') {
					valStr=valStr.Substring(1, valStr.Length-2);
				}
				Add(keyStr, valStr);
			} else {
				if(valStr.Contains(".")) {
					float res;
					if(float.TryParse(valStr, out res)) {
						Add(keyStr, res);
					} else {
						throw new System.Exception("parse error: cannot convert to float at line "+lineNumber+": \""+valStr+"\"");
					}
				} else {
					int res;
					if(int.TryParse(valStr, out res)) {
						Add(keyStr, res);
					} else {
						throw new System.Exception("parse error: cannot convert to int at line "+lineNumber+": \""+valStr+"\"");
					}
					Add(keyStr, res);
				}	
			}
			lineNumber++;
		}
		reader.Close();
	}
	public static bool DigitsOrDotOnly(string input) {
		for(int i=0; i<input.Length; i++) {
			if( ! (System.Char.IsDigit(input[i]) || input[i]=='.')) return false;
		}
        return true;
	}
	public static string GetExternalDataPath() {
		string absPath=null;
	    using (AndroidJavaClass environmentClass = new AndroidJavaClass("android.os.Environment")) {
			AndroidJavaObject rootPath=environmentClass.CallStatic<AndroidJavaObject>("getExternalStorageDirectory");
			absPath=rootPath.Call<string>("getAbsolutePath"); 	
		}
		return absPath;
	}
	
}

*/
