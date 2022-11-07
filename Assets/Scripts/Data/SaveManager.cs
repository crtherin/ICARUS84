using UnityEngine;

public class SaveManager : MonoBehaviour
{
	private const string Key = "Player";

	[SerializeField] private ScriptableObject[] targets;

	protected void Start ()
	{
		Load ();
	}

	public void Save ()
	{
		NameJsonPairs pairs = new NameJsonPairs (targets);
		PlayerPrefs.SetString (Key, JsonUtility.ToJson (pairs));
	}

	public void Load ()
	{
		if (!PlayerPrefs.HasKey (Key))
			return;

		NameJsonPairs pairs = JsonUtility.FromJson<NameJsonPairs> (PlayerPrefs.GetString (Key));

		for (int i = 0; i < targets.Length; i++)
		{
			string json = pairs.Get (targets[i].name);

			if (string.IsNullOrEmpty (json))
				continue;

			JsonUtility.FromJsonOverwrite (json, targets[i]);
		}
	}

	[System.Serializable]
	public class NameJsonPairs
	{
		[SerializeField] private readonly Pairs[] pairs;

		public NameJsonPairs (ScriptableObject[] targets)
		{
			pairs = new Pairs[targets.Length];

			for (int i = 0; i < targets.Length; i++)
				pairs[i] = new Pairs (targets[i].name, JsonUtility.ToJson (targets[i]));
		}

		public string Get (string name)
		{
			if (pairs == null)
				return null;

			for (int i = 0; i < pairs.Length; i++)
				if (pairs[i].Name == name)
					return pairs[i].Json;

			return null;
		}

		[System.Serializable]
		public class Pairs
		{
			[SerializeField] private string name;
			[SerializeField] private string json;

			public string Name
			{
				get { return name; }
			}

			public string Json
			{
				get { return json; }
			}

			public Pairs (string name, string json)
			{
				this.name = name;
				this.json = json;
			}
		}
	}
}