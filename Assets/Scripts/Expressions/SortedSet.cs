using System.Collections.Generic;

namespace Expressions
{
	public class SortedSet<T>
	{
		private readonly List<KeyValuePair<IExpressionElement, T>> list;
		private readonly IComparer<KeyValuePair<IExpressionElement, T>> comparer;
		private readonly Dictionary<IExpressionElement, KeyValuePair<IExpressionElement, T>> table;

		private SortedSet (List<KeyValuePair<IExpressionElement, T>> list, IComparer<KeyValuePair<IExpressionElement, T>> comparer)
		{
			this.list = list;
			this.comparer = comparer;

			table = new Dictionary<IExpressionElement, KeyValuePair<IExpressionElement, T>> ();

			for (int i = 0; i < list.Count; i++)
				table[list[i].Key] = list[i];
		}

		public SortedSet () : this (new List<KeyValuePair<IExpressionElement, T>> (), Comparer<KeyValuePair<IExpressionElement, T>>.Default)
		{
		}

		public SortedSet (IComparer<KeyValuePair<IExpressionElement, T>> comparer) : this (new List<KeyValuePair<IExpressionElement, T>> (), comparer)
		{
		}

		public int Count
		{
			get { return list.Count; }
		}

		public T this [int index]
		{
			get { return Get (index); }
			set { Add (list[index].Key, value); }
		}

		public T this [IExpressionElement source]
		{
			get { return Get (source); }
			set { Add (source, value); }
		}

		public T Get (int index)
		{
			return list[index].Value;
		}

		public T Get (IExpressionElement source)
		{
			return table[source].Value;
		}

		public void Add (IExpressionElement source, T item)
		{
			KeyValuePair<IExpressionElement, T> kvp;

			if (Contains (source))
			{
				kvp = table[source];
				list.Remove (kvp);
			}

			kvp = new KeyValuePair<IExpressionElement, T> (source, item);
			table[source] = kvp;

			int index = list.BinarySearch (kvp, comparer);
			if (index < 0)
				index = ~index;

			list.Insert (index, kvp);
		}

		public void Remove (IExpressionElement source)
		{
			if (!Contains (source))
				return;

			list.Remove (table[source]);
			table.Remove (source);
		}

		public bool Contains (IExpressionElement source)
		{
			return table.ContainsKey (source);
		}

		public void Clear ()
		{
			list.Clear ();
			table.Clear ();
		}
	}
}