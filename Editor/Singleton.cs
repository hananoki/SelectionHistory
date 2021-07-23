using UnityEditor;
using UnityEngine;


namespace HananokiEditor.SelectionHistory {

	public sealed class Singleton : ScriptableSingleton<Singleton> {

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
}
