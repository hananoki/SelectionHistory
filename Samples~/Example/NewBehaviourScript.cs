using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using HananokiEditor;

public class NewBehaviourScript : MonoBehaviour { }


[CanEditMultipleObjects, CustomEditor( typeof( NewBehaviourScript ) )]
public class NewBehaviourScriptInspector : Editor {
	public override void OnInspectorGUI() {
		using( new GUILayout.HorizontalScope() ) {
			if( GUILayout.Button( "GetPrevList" ) ) {
				foreach( var p in SelectionHistory.GetPrevList() ) Debug.Log( $"{p.Item1} {p.Item2}" );
			}
			if( GUILayout.Button( "GetNextList" ) ) {
				foreach( var p in SelectionHistory.GetNextList() ) Debug.Log( $"{p.Item1} {p.Item2}" );
			}
		}
		//GUILayout.Label( "posision = " + SelectionHistoryParameter.instance.posision );

		for( var i = 0; i < SelectionHistoryParameter.instance.Count; i++ ) {
			var p = SelectionHistoryParameter.instance[ i ];
			if( p == null ) continue;
			GUIStyle st = SelectionHistoryParameter.instance.posision - 1 == i ? EditorStyles.boldLabel : EditorStyles.label;
			using(new GUILayout.HorizontalScope()) {
				GUILayout.Label( $"{i}: {p.name} { p.GetType()}", st );
				GUILayout.FlexibleSpace();
				if( GUILayout.Button( "Select" ) ) {
					SelectionHistory.SetIndex( i );
				}
			}
		}
	}
}


namespace Menu {
	public static class Menu {

		[MenuItem( "SelectionHistory/Prev" )]
		public static void Prev() {
			SelectionHistory.Prev();
		}
		[MenuItem( "SelectionHistory/Prev", true )]
		public static bool PrevCheck() {
			return SelectionHistory.HasPrev();
		}

		[MenuItem( "SelectionHistory/Next" )]
		public static void Next() {
			SelectionHistory.Next();
		}
		[MenuItem( "SelectionHistory/Next", true )]
		public static bool NextCheck() {
			return SelectionHistory.HasNext();
		}
	}
}
