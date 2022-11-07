using Data;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Procedures
{
	public class SerializedProcedure : ScriptableObject, IDataDriven
	{
		public const string ContainerNode = "Procedures";

		private Node node;
		[SerializeField] private Container container;
		[SerializeField] private new string name = string.Empty;
		[SerializeField] private List<SerializedProcess> processes = new List<SerializedProcess> ();
		[SerializeField] private VariationTree tree = new VariationTree ();

#if UNITY_EDITOR
		[SerializeField] private bool isTreeFolded = true;
		[SerializeField] private bool applyTree;

		public bool ApplyTree
		{
			get { return applyTree; }
		}
#endif

		public VariationTree GetTree ()
		{
			return tree;
		}

		public Container GetContainer ()
		{
			return container;
		}

		public void SetContainer (Container container)
		{
			this.container = container;
		}

		public string GetName ()
		{
			return name;
		}

		public void SetName (string name)
		{
			this.name = name;
#if UNITY_EDITOR
			EditorUtility.SetDirty (this);
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh ();
			UpdateNodeName (name);
#endif
		}

		public void OnEnable ()
		{
#if UNITY_EDITOR
			container = container.GetAssetReference ();
#endif

			node = FindNode ();

			if (node == null)
				return;

			for (int i = 0; i < processes.Count; i++)
				processes[i].GetTable ().ConnectTo (node.GetChild (i).GetTable ());
		}

		public void AddProcessesTo (Procedure procedure)
		{
			for (int i = 0; i < processes.Count; i++)
			{
				procedure
					.AddProcess (processes[i].GetType ().ToType<Process> (), processes[i].GetHash (), false)
					.LoadFrom (processes[i].GetTable ());
			}
		}

		public Node FindNode (DataField field = null)
		{
			if (container == null)
				return null;

			node = container.GetRootNode ().FindOrCreate (ContainerNode + "." + name);

			if (node == null || field == null)
				return node;

			for (int i = 0; i < processes.Count; i++)
			{
				if (node.GetChildCount () <= i)
				{
					node.AddChild (new Node (processes[i].GetType ()));
#if UNITY_EDITOR
					SetContainerDirty ();
#endif
				}

				DataField[] fields = processes[i].GetTable ().GetAllFields ();

				for (int j = 0; j < fields.Length; j++)
					if (fields[j] != null && fields[j] == field)
						return node.GetChild (i);
			}

			return null;
		}

#if UNITY_EDITOR

		#region Custom Editor Helper Methods

		public int GetHash (int index)
		{
			if (index < 0 || index >= processes.Count)
				return 0;

			return processes[index].GetHash ();
		}

		public bool IsTreeFolded ()
		{
			return isTreeFolded;
		}

		public SerializedProcess Instantiate (int index)
		{
			return processes[index];
		}

		public void AddProcess (SerializedProcess process)
		{
			processes.Add (process);

			if (node != null)
			{
				node.AddChild (new Node (process.GetType ()));
				SetContainerDirty ();
			}

			if (tree != null)
			{
				int hash = process.GetHash ();
				tree.Exclude (hash);
				tree.ToggleInSelected (hash);
			}
		}

		public void AddNewProcess ()
		{
			AddProcess (new SerializedProcess ());
		}

		public void InsertProcess (int index, SerializedProcess process)
		{
			processes.Insert (index, process);

			if (node != null)
			{
				node.InsertChild (index, new Node ());
				SetContainerDirty ();
			}
		}

		public void InsertProcessAfter (int index)
		{
			if (processes.Count > index + 1)
				InsertProcess (index + 1, new SerializedProcess ());
			else
				AddNewProcess ();
		}

		public void RemoveProcessAt (int index)
		{
			processes.RemoveAt (index);

			if (node != null)
			{
				node.RemoveChildAt (index);
				SetContainerDirty ();
			}
		}

		public void SwitchProcesses (int indexA, int indexB)
		{
			processes.Switch (indexA, indexB);

			if (node != null)
			{
				node.SwitchChildren (indexA, indexB);
				SetContainerDirty ();
			}
		}

		public void UpdateNodeName (string name)
		{
			FindNode ();

			this.name = name;

			if (node != null)
			{
				node.SetName (name);
				SetContainerDirty ();
			}
		}

		public void UpdateChildNodeName (int index)
		{
			if (node != null)
			{
				node.GetChild (index).SetName (processes[index].GetType ());
				SetContainerDirty ();
			}
		}

		private void SetContainerDirty ()
		{
			if (container != null)
				EditorUtility.SetDirty (container);
		}

		public void SetTree (VariationTree tree)
		{
			this.tree = tree;
		}

		#endregion

		[MenuItem ("Assets/Create/Procedure")]
		public static void CreateAsset ()
		{
			ScriptableObjectUtility.CreateAsset<SerializedProcedure> ();
		}
#endif
	}
}