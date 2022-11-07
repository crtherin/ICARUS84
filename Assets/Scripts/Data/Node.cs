using System;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
	[Serializable]
	public class Node
	{
		[SerializeField] private string name = string.Empty;
		[SerializeField] private DataTable dataTable = new DataTable ();
		[SerializeField] private List<Node> children = new List<Node> ();

		public Node (string name = default (string))
		{
			this.name = name ?? string.Empty;
			dataTable = new DataTable ();
			children = new List<Node> ();
		}

		public string GetName ()
		{
			return name;
		}

		public void SetName (string name)
		{
			this.name = name;
		}

		#region Children Methods

		public int GetChildCount ()
		{
			return children.Count;
		}

		public Node GetChild (int index)
		{
			return children[index];
		}

		public void SetChild (int index, Node node)
		{
			children[index] = node;
		}

		public void AddChild (Node node)
		{
			children.Add (node);
		}

		public void InsertChild (int index, Node node)
		{
			children.Insert (index, node);
		}

		public void RemoveChild (Node node)
		{
			children.Remove (node);
		}

		public void RemoveChildAt (int index)
		{
			children.RemoveAt (index);
		}

		public void SwitchChildren (int indexA, int indexB)
		{
			children.Switch (indexA, indexB);
		}

		#endregion

		public DataTable GetTable ()
		{
			return dataTable ?? (dataTable = new DataTable ());
		}

		public void SetTable (DataTable dataTable)
		{
			this.dataTable = dataTable;
		}

		public Node FindOrCreate (string path = default (string))
		{
			if (path == null)
				return this;

			int nextIndex = path.IndexOf ('.');
			string nextName = nextIndex > 0 ? path.Substring (0, nextIndex) : path;

			Node node = null;

			for (int i = 0; i < children.Count; i++)
				if (children[i].name == nextName)
					node = children[i];

			if (node == null)
				children.Add (node = new Node (nextName));

			if (nextIndex > 0)
				node = node.FindOrCreate (path.Substring (nextIndex + 1));

			return node;
		}
	}
}