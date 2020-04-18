
//#define ENABLE_LEGACY_PREFERENCE

using Hananoki.SharedModule;
using Hananoki.Extensions;
using UnityEditor;
using UnityEngine;
using E = Hananoki.SelectionHistory.SettingsEditor;

namespace Hananoki.SelectionHistory {
	[System.Serializable]
	public class SettingsEditor {

		public int recordObjectCount = 30;
		public bool enablePingObject = true;

		public static E i;

		public static void Load() {
			if( i != null ) return;
			i = EditorPrefJson<E>.Get( Package.editorPrefName );
		}

		public static void Save() {
			EditorPrefJson<E>.Set( Package.editorPrefName, i );
		}
	}



	public class SettingsEditorWindow : HSettingsEditorWindow {

		static bool s_changed;
		static Vector2 scrollPos;

		public static void Open() {
			var window = GetWindow<SettingsEditorWindow>();
			window.SetTitle( new GUIContent( Package.name, Icon.Get( "SettingsIcon" ) ) );
		}

		void OnEnable() {
			drawGUI = DrawGUI;
			E.Load();
		}


		/// <summary>
		/// 
		/// </summary>
		static void DrawGUI() {

			using( new PreferenceLayoutScope( ref scrollPos ) ) {
				EditorGUI.BeginChangeCheck();

				E.i.enablePingObject = HEditorGUILayout.ToggleLeft( S._EnablePingObject, E.i.enablePingObject );
				E.i.recordObjectCount = EditorGUILayout.IntSlider( S._RecordObjectCount, E.i.recordObjectCount, 2, 128 );

				using( new GUILayout.HorizontalScope() ) {
					GUILayout.FlexibleSpace();
					if( GUILayout.Button( S._Apply ) ) {
						SelectionHistoryParameter.instance.Init( E.i.recordObjectCount );
					}
				}

				if( EditorGUI.EndChangeCheck() ) {
					s_changed = true;
				}
			}

			if( s_changed ) {
				E.Save();
			}

			GUILayout.Space( 8f );
		}





#if UNITY_2018_3_OR_NEWER && !ENABLE_LEGACY_PREFERENCE
		static void titleBarGuiHandler() {
			GUILayout.Label( $"{Package.version}", EditorStyles.miniLabel );
		}
		[SettingsProvider]
		public static SettingsProvider PreferenceView() {
			var provider = new SettingsProvider( $"Preferences/Hananoki/{Package.name}", SettingsScope.User ) {
				label = $"{Package.name}",
				guiHandler = PreferencesGUI,
				titleBarGuiHandler = titleBarGuiHandler,
			};
			return provider;
		}
		public static void PreferencesGUI( string searchText ) {
#else
		[PreferenceItem( Settings.PACKAGE_NAME )]
		public static void PreferencesGUI() {
#endif
			E.Load();
			DrawGUI();
		}
	}
}
