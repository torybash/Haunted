using UnityEngine;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEditor;
using System.Collections.Generic;

public class UtilWindow : EditorWindow {


	private static UtilWindow window;

	[MenuItem ("Haunted/Util Window %u")]
	private static void Init(){
		window = GetWindow<UtilWindow>();
		window.Show();
	}


	void OnGUI(){
		if (!window) Init();

		if (GUILayout.Button("Print all axes")){
			var inputAxes = GetInputAxis().ToArray();
			Debug.Log("Input axes:\n"+ string.Join("\n", inputAxes));
		}

		if (GUILayout.Button("Create next joystick")){
			var inputAxes = GetInputAxis();
			var lastNr = int.Parse(inputAxes[inputAxes.Count - 1][3].ToString());
			var lastAxes = inputAxes.Where(x => x.Contains("Joy" +lastNr)).ToList();;

			Debug.Log("Creating joystick nr " + (lastNr + 1));

			foreach (var item in lastAxes) {
				var axisProp = GetInputAxis(item);
				if (axisProp != null) AddAxis(axisProp, (lastNr + 1));
			}
		}
	}

	/// <summary>
	/// Gets all the input axis defined in the project's Input manager
	/// (gets it from ProjectSettings/InputManager.asset)
	/// </summary>
	public List<string> GetInputAxis()
	{
		var allAxis = new List<string>();
		var serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		var axesProperty = serializedObject.FindProperty("m_Axes");
		axesProperty.Next(true);
		axesProperty.Next(true);
		while (axesProperty.Next(false))
		{
			SerializedProperty axis = axesProperty.Copy();
			var nameProp = axis.FindPropertyRelative("m_Name");
			allAxis.Add(nameProp.stringValue);
		}
		return allAxis;
	}

	public SerializedProperty GetInputAxis(string name)
	{
		var serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		var axesProperty = serializedObject.FindProperty("m_Axes");
		axesProperty.Next(true);
		axesProperty.Next(true);
		while (axesProperty.Next(false))
		{
			var axis = axesProperty.Copy();
			var nameProp = axesProperty.FindPropertyRelative("m_Name");
			if (nameProp.stringValue == name) return axis;
		}
		return null;
	}



	public void AddAxis(SerializedProperty copyFrom, int joyStickNr){
		

		SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
		SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

		axesProperty.arraySize++;
		serializedObject.ApplyModifiedProperties();

		SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

		GetChildProperty(axisProperty, "m_Name").stringValue = GetChildProperty(copyFrom, "m_Name").stringValue;
		GetChildProperty(axisProperty, "descriptiveName").stringValue = GetChildProperty(copyFrom, "descriptiveName").stringValue;
		GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = GetChildProperty(copyFrom, "descriptiveNegativeName").stringValue;
		GetChildProperty(axisProperty, "negativeButton").stringValue = GetChildProperty(copyFrom, "negativeButton").stringValue;;
		GetChildProperty(axisProperty, "positiveButton").stringValue = GetChildProperty(copyFrom, "positiveButton").stringValue;
		GetChildProperty(axisProperty, "altNegativeButton").stringValue = GetChildProperty(copyFrom, "altNegativeButton").stringValue;;
		GetChildProperty(axisProperty, "altPositiveButton").stringValue = GetChildProperty(copyFrom, "altPositiveButton").stringValue;;
		GetChildProperty(axisProperty, "gravity").floatValue = GetChildProperty(copyFrom, "gravity").floatValue;
		GetChildProperty(axisProperty, "dead").floatValue = GetChildProperty(copyFrom, "dead").floatValue;
		GetChildProperty(axisProperty, "sensitivity").floatValue = GetChildProperty(copyFrom, "sensitivity").floatValue;
		GetChildProperty(axisProperty, "snap").boolValue = GetChildProperty(copyFrom, "snap").boolValue;
		GetChildProperty(axisProperty, "invert").boolValue = GetChildProperty(copyFrom, "invert").boolValue;
		GetChildProperty(axisProperty, "type").intValue = (int)GetChildProperty(copyFrom, "type").intValue;
		GetChildProperty(axisProperty, "axis").intValue = GetChildProperty(copyFrom, "axis").intValue;
		GetChildProperty(axisProperty, "joyNum").intValue = GetChildProperty(copyFrom, "joyNum").intValue;

//		axisProperty = copyFrom.Copy();
//		axisProperty.FindPropertyRelative("joyNum").intValue = (joyStickNr + 1);
		var newName = new StringBuilder(copyFrom.FindPropertyRelative("m_Name").stringValue);
		newName[3] = ("" + joyStickNr).ToCharArray()[0];

		var newJoyStickNr = new StringBuilder(copyFrom.FindPropertyRelative("positiveButton").stringValue);
		if (newJoyStickNr.Length > 10) newJoyStickNr[9] = ("" + joyStickNr).ToCharArray()[0];
//		newJoyStickNr = newJoyStickNr.Replace("" + (joyStickNr-1), "" + (joyStickNr));
//		newJoyStickNr.

//		axisProperty.FindPropertyRelative("m_Name").stringValue = newName;
		GetChildProperty(axisProperty, "m_Name").stringValue = newName.ToString();
		GetChildProperty(axisProperty, "positiveButton").stringValue = newJoyStickNr.ToString();
		GetChildProperty(axisProperty, "joyNum").intValue = joyStickNr;

		serializedObject.ApplyModifiedProperties();

		Debug.Log("joyStickNr: " + joyStickNr + ", newName: "+ newName + ", added axis from copy: "+ copyFrom.FindPropertyRelative("m_Name").stringValue +  " joyNum: " + axisProperty.FindPropertyRelative("joyNum").intValue);
	}


	private SerializedProperty GetChildProperty(SerializedProperty parent, string name)
	{
		SerializedProperty child = parent.Copy();
		child.Next(true);
		do
		{
			if (child.name == name) return child;
		}
		while (child.Next(false));
		return null;
	}
}


//From http://plyoung.appspot.com/blog/manipulating-input-manager-in-script.html
//GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
//GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
//GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
//GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
//GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
//GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
//GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
//GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
//GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
//GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
//GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
//GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
//GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
//GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
//GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;
