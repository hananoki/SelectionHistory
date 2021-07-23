using HananokiEditor.SharedModule;
using UnityEditor;
using UnityEngine;
using E = HananokiEditor.SelectionHistory.EditorPref;


namespace HananokiEditor.SelectionHistory {

	public sealed class SettingsGUI {

		[HananokiSettingsRegister]
		public static SettingsItem RegisterSettings() {
			return new SettingsItem() {
				displayName = Package.nameNicify,
				version = Package.version,
				gui = DrawGUI,
			};
		}


		static bool s_changed;


		/////////////////////////////////////////
		public static void DrawGUI() {
			E.Load();

			ScopeChange.Begin();

			E.i.enablePingObject = HEditorGUILayout.ToggleLeft( S._EnablePingObject, E.i.enablePingObject );
			E.i.recordObjectCount = EditorGUILayout.IntSlider( S._RecordObjectCount, E.i.recordObjectCount, 2, 128 );

			using( new GUILayout.HorizontalScope() ) {
				GUILayout.FlexibleSpace();
				if( GUILayout.Button( S._Apply ) ) {
					Singleton.instance.Init( E.i.recordObjectCount );
				}
			}

			if( ScopeChange.End() ) {
				s_changed = true;
			}


			if( s_changed ) {
				E.Save();
			}
		}


	}
}
