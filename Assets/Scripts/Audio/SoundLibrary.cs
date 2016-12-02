using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif //if UNITY_EDITOR

[CreateAssetMenuAttribute]
public class SoundLibrary : ScriptableObject {

	public AudioClip[] sounds;

}

//
//#if UNITY_EDITOR
//[CustomEditor(typeof(SoundLibrary))]
//
//public class SoundLibraryEditor : Editor{
//	private ReorderableList list;
//
//    private void OnEnable() {
//
//        list = new ReorderableList(serializedObject, serializedObject.FindProperty("sounds"), 
//                true, true, true, true);
//
//		list.drawHeaderCallback = (Rect rect) => {  
//		    EditorGUI.LabelField(rect, "Sounds");
//		};
//
//		list.onSelectCallback = (ReorderableList l) => {  
//			AudioClip clip = l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("clip").objectReferenceValue as AudioClip;
//			if (clip)
//				EditorGUIUtility.PingObject(clip);
//		};
//
//		//Force can not "-" when 0 elem left
//		list.onCanRemoveCallback = (ReorderableList l) => {  
//		    return l.count > 1;
//		};
//
//		//Enable dialog box on "-"
////		list.onRemoveCallback = (ReorderableList l) => {  
////		    if (EditorUtility.DisplayDialog("Warning!", 
////		        "Are you sure you want to delete the wave?", "Yes", "No")) {
////		        ReorderableList.defaultBehaviours.DoRemoveButton(l);
////		    }
////		};
//
//		list.drawElementCallback =  
//		    (Rect rect, int index, bool isActive, bool isFocused) => {
//		    var element = list.serializedProperty.GetArrayElementAtIndex(index);
//		    rect.y += 2;
//		    EditorGUI.PropertyField(
//		        new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight),
//				element.FindPropertyRelative("soundType"), GUIContent.none);
//		    EditorGUI.PropertyField(
//		        new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight),
//				element.FindPropertyRelative("clip"), GUIContent.none);
////		    EditorGUI.PropertyField(
////		        new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight),
////		        element.FindPropertyRelative("Count"), GUIContent.none);
//		};
//    }
//
//    public override void OnInspectorGUI() {
//        serializedObject.Update();
//        list.DoLayoutList();
//        serializedObject.ApplyModifiedProperties();
//    }
//}
//#endif
