
using UnityEngine;
using UnityEditor;
using Hananoki;
using Hananoki.Extensions;
using System.Collections.Generic;

using Settings = HananokiEditor.SelectionHistorySettings;

namespace HananokiEditor {

	public class SelectionHistoryParameter : ScriptableSingleton<SelectionHistoryParameter> {
		public int posision = 0;
		public int command = 0;
		public bool inited = false;


		public Object[] data;
		public int top;
		public int bottom;
		public int mask;

		public Object this[ int i ] {
			get { return this.data[ ( i + this.top ) & this.mask ]; }
			set { this.data[ ( i + this.top ) & this.mask ] = value; }
		}

		public void Init( int capacity ) {
			capacity++;
			capacity = Pow2( (uint) capacity );
			this.data = new Object[ capacity ];
			this.top = this.bottom = 0;
			this.mask = capacity - 1;

			posision = 0;

			inited = true;
		}

		static int Pow2( uint n ) {
			--n;
			int p = 0;
			for( ; n != 0; n >>= 1 ) p = ( p << 1 ) + 1;
			return p + 1;
		}

		public int Count {
			get {
				int count = this.bottom - this.top;
				if( count < 0 ) count += this.data.Length;
				return count;
			}
		}

		public void EraseFirst() {
			this.top = ( this.top + 1 ) & this.mask;
		}

		public void EraseLast() {
			this.bottom = ( this.bottom - 1 ) & this.mask;
		}

		public void InsertLast( Object elem ) {
			if( this.Count >= this.data.Length - 1 ) {
				//this.Extend();
				Debug.Log( "Error" );
			}

			this.data[ this.bottom ] = elem;
			this.bottom = ( this.bottom + 1 ) & this.mask;
		}

	}


	[InitializeOnLoad]
	public class SelectionHistory {
		static SelectionHistoryParameter self;

		const int COMMAND_NONE = 0;
		const int COMMAND_MOVE_POSITION = 1;

		static SelectionHistory() {
			SelectionHistorySettings.Load();
			self = SelectionHistoryParameter.instance;
			self.command = 0;

			if( !self.inited ) self.Init( SelectionHistorySettings.i.recordObjectCount );

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

			if( SelectionHistorySettings.i.recordObjectCount <= self.Count ) {
				self.EraseFirst();
				self.posision--;
			}
			self.InsertLast( Selection.activeObject );
		}


		public static (int, string)[] GetPrevList() {
			var s = new List<(int, string)>();
			for( int i = self.posision - 1; 1 <= i; i-- ) {
				var ii = i-1;
				if( self[ ii ] == null ) continue;
				var ap = AssetDatabase.GetAssetPath( self[ ii  ] );
				if( ap.IsEmpty() ) {
					s.Add( (ii, $"{ii}:{self[ ii ].name}") );
				}
				else {
					s.Add( (ii, $"{ii}:{ap.Replace( "/", "." )}") );
				}
			}
			return s.ToArray();
		}


		public static (int,string)[] GetNextList() {
			var s = new List<(int, string)>();
			for( int i = self.posision; i < self.Count; i++ ) {
				var ap = AssetDatabase.GetAssetPath( self[ i ] );
				if( ap.IsEmpty() ) {
					s.Add( (i, $"{i}:{self[ i ].name}") );
				}
				else {
					s.Add( (i,$"{i}:{ap.Replace( "/", "." )}") );
				}
			}
			return s.ToArray();
		}


		static void Set() {
			Selection.activeObject = self[ self.posision - 1 ];
			if( Settings.i.enablePingObject ) {
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
