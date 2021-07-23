using HananokiRuntime.Extensions;
using System.Collections.Generic;
using UnityEditor;
using E = HananokiEditor.SelectionHistory.EditorPref;


namespace HananokiEditor.SelectionHistory {

	[InitializeOnLoad]
	public class Main {
		static Singleton self;

		const int COMMAND_NONE = 0;
		const int COMMAND_MOVE_POSITION = 1;

		static Main() {
			E.Load();
			self = Singleton.instance;
			self.command = 0;

			if( !self.inited ) self.Init( E.i.recordObjectCount );

			Selection.selectionChanged += OnSelectionChanged;
		}


		static void OnSelectionChanged() {
			if( Selection.activeObject == null ) return;
			//Debug.Log( AssetDatabase.GetAssetPath( Selection.activeObject ) );
			int old = self.command;
			self.command = COMMAND_NONE;
			if( old != COMMAND_NONE ) return;

			while( self.posision < self.Count ) {
				self.EraseLast();
			}
			self.posision++;

			if( E.i.recordObjectCount <= self.Count ) {
				self.EraseFirst();
				self.posision--;
			}
			self.InsertLast( Selection.activeObject );
		}


		public static (int, string)[] GetPrevList() {
			var s = new List<(int, string)>();
			for( int i = self.posision - 1; 1 <= i; i-- ) {
				var ii = i - 1;
				if( self[ ii ] == null ) continue;
				var ap = AssetDatabase.GetAssetPath( self[ ii ] );
				if( ap.IsEmpty() ) {
					s.Add( (ii, $"{ii}:{self[ ii ].name}") );
				}
				else {
					s.Add( (ii, $"{ii}:{ap.Replace( "/", "." )}") );
				}
			}
			return s.ToArray();
		}


		public static (int, string)[] GetNextList() {
			var s = new List<(int, string)>();
			for( int i = self.posision; i < self.Count; i++ ) {
				var ap = AssetDatabase.GetAssetPath( self[ i ] );
				if( ap.IsEmpty() ) {
					s.Add( (i, $"{i}:{self[ i ].name}") );
				}
				else {
					s.Add( (i, $"{i}:{ap.Replace( "/", "." )}") );
				}
			}
			return s.ToArray();
		}


		static void Set() {
			Selection.activeObject = self[ self.posision - 1 ];
			if( E.i.enablePingObject ) {
				EditorGUIUtility.PingObject( Selection.activeObject );
			}
		}


		public static void SetIndex( int index ) {
			self.command = COMMAND_MOVE_POSITION;
			self.posision = index + 1;
			Set();
		}


		public static void Prev() {
			if( !HasPrev() ) return;

			self.command = COMMAND_MOVE_POSITION;
			self.posision--;
			Set();
		}


		public static void Next() {
			if( !HasNext() ) return;

			self.command = COMMAND_MOVE_POSITION;
			self.posision++;
			Set();
		}


		public static bool HasPrev() {
			return 2 <= self.posision;
		}


		public static bool HasNext() {
			return self.posision < self.Count;
		}
	}
}
