
//#define ENABLE_LEGACY_PREFERENCE

using Hananoki.Shared.Localize;
using Hananoki;
using UnityEditor;
using UnityEngine;
using Settings = HananokiEditor.SelectionHistorySettings;

namespace HananokiEditor {
	[System.Serializable]
	public class SelectionHistorySettings {
		public const string PACKAGE_NAME = "SelectionHistory";
		public const string PREF_NAME = "Hananoki.SelectionHistory";
		public const string VER = "v1.00";

		public int recordObjectCount = 30;
		public bool enablePingObject = true;

		public static Settings i;

		public static void Load() {
			if( i != null ) return;
			i = EditorPrefJson<Settings>.Get( Settings.PREF_NAME );
		}

		public static void Save() {
			EditorPrefJson<Settings>.Set( Settings.PREF_NAME, i );
		}
	}



	public class SelectionHistorySettingsWindow : HSettingsEditorWindow {

		static bool s_changed;

		public static void Open() {
			var window = GetWindow<SelectionHistorySettingsWindow>();
			window.SetTitle( new GUIContent( Settings.PACKAGE_NAME, Icon.Get( "SettingsIcon" ) ) );
		}

		void OnEnable() {
			drawGUI = DrawGUI;
			Settings.Load();
		}


		/// <summary>
		/// 
		/// </summary>
		static void DrawGUI() {

			using( new PreferenceLayoutScope() ) {
				EditorGUI.BeginChangeCheck();

				Settings.i.enablePingObject = HEditorGUILayout.ToggleLeft( S._EnablePingObject, Settings.i.enablePingObject );
				Settings.i.recordObjectCount = EditorGUILayout.IntSlider( S._RecordObjectCount, Settings.i.recordObjectCount, 2, 128 );
				
				using( new GUILayout.HorizontalScope() ) {
					GUILayout.FlexibleSpace();
					if( GUILayout.Button( S._Apply ) ) {
						SelectionHistoryParameter.instance.Init( Settings.i.recordObjectCount );
					}
				}

				if( EditorGUI.EndChangeCheck() ) {
					s_changed = true;
				}
			}

			if( s_changed ) {
				Settings.Save();
			}

			GUILayout.Space( 8f );
		}





#if UNITY_2018_3_OR_NEWER && !ENABLE_LEGACY_PREFERENCE
		static void titleBarGuiHandler() {
			GUILayout.Label( $"{Settings.VER}", EditorStyles.miniLabel );
		}
		[SettingsProvider]
		public static SettingsProvider PreferenceView() {
			var provider = new SettingsProvider( $"Preferences/Hananoki/{Settings.PACKAGE_NAME}", SettingsScope.User ) {
				label = $"{Settings.PACKAGE_NAME}",
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
			Settings.Load();
			DrawGUI();
		}
	}
}
