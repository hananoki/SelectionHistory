using System;
using E = HananokiEditor.SelectionHistory.EditorPref;


namespace HananokiEditor.SelectionHistory {

	[Serializable]
	public sealed class EditorPref {

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


}
