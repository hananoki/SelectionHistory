
using Hananoki.Extensions;
using Hananoki.SharedModule;
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

		public static void Open() {
			var w = GetWindow<SettingsEditorWindow>();
			w.SetTitle( new GUIContent( Package.name, EditorIcon.settings ) );
			w.headerMame = Package.name;
			w.headerVersion = Package.version;
			w.gui = DrawGUI;
		}


		/// <summary>
		/// 
		/// </summary>
		public static void DrawGUI() {
			E.Load();

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


			if( s_changed ) {
				E.Save();
			}

			GUILayout.Space( 8f );
		}


#if !ENABLE_HANANOKI_SETTINGS
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
		[PreferenceItem( Package.name )]
		public static void PreferencesGUI() {
#endif
			using( new LayoutScope() ) DrawGUI();
		}
#endif
	}



#if ENABLE_HANANOKI_SETTINGS
	[SettingsClass]
	public class SettingsEvent {
		[SettingsMethod]
		public static SettingsItem RegisterSettings() {
			return new SettingsItem() {
				displayName = Package.name,
				version = Package.version,
				gui = SettingsEditorWindow.DrawGUI,
			};
		}
	}
#endif
}
