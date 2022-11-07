using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Data
{
	[Serializable]
	public class Container : ScriptableObject
	{
		[SerializeField] private Node root;

		public Container ()
		{
			root = new Node ("Root");
		}

		public Node GetRootNode ()
		{
			return root;
		}

#if UNITY_EDITOR
		[MenuItem ("Assets/Create/Data Container")]
		public static void CreateAsset ()
		{
			ScriptableObjectUtility.CreateAsset<Container> ();
		}
#endif
	}
}