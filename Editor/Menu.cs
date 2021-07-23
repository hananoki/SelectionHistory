using UnityEditor;


namespace HananokiEditor.SelectionHistory {

	public sealed class Menu {

		/////////////////////////////////////////
		[MenuItem( "Tools/Selection History/Prev" )]
		public static void Prev() {
			Main.Prev();
		}


		/////////////////////////////////////////
		[MenuItem( "Tools/Selection History/Prev", true )]
		public static bool PrevCheck() {
			return Main.HasPrev();
		}


		/////////////////////////////////////////
		[MenuItem( "Tools/Selection History/Next" )]
		public static void Next() {
			Main.Next();
		}


		/////////////////////////////////////////
		[MenuItem( "Tools/Selection History/Next", true )]
		public static bool NextCheck() {
			return Main.HasNext();
		}


	}
}
