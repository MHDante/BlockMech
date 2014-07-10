using System;
namespace AssemblyCSharp
{
		public static class Utils
		{
		public static Side opposite(this Side s){
			switch(s){
			case Side.bottom:
				return Side.top;
			case Side.top:
				return Side.bottom;
			case Side.left:
				return Side.right;
			case Side.right:
				return Side.left;
			}throw new WTFException();
		}
		}

	public class WTFException : Exception{
		public WTFException():base("What the Fuck?!?!");
}

