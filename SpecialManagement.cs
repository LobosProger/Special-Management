using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SpecialManagement : InterestManagement
{

	[Tooltip("The maximum range that objects will be visible at.")]
	public int visRange = 30;

	// if we see 8 neighbors then 1 entry is visRange/3
	public int resolution => visRange / 3;

	[Tooltip("Rebuild all every 'rebuildInterval' seconds.")]
	public float rebuildInterval = 1;
	double lastRebuildTime;

	public enum CheckMethod
	{
		XZ_FOR_3D,
		XY_FOR_2D
	}
	[Tooltip("Spatial Hashing supports 3D (XZ) and 2D (XY) games.")]
	public CheckMethod checkMethod = CheckMethod.XZ_FOR_3D;

	// debugging
	public bool showSlider;

	private Dictionary<Vector2Int, HashSet<NetworkConnection>> CellsObservers = new Dictionary<Vector2Int, HashSet<NetworkConnection>>();

	Vector2Int[] neighbourOffsets =
	{
				Vector2Int.up,
				Vector2Int.left,
				Vector2Int.zero,
				Vector2Int.right,
				Vector2Int.down,
				Vector2Int.down + Vector2Int.left,
				Vector2Int.down + Vector2Int.right,
	};

	Vector2Int ProjectToGrid(Vector3 position) =>
				checkMethod == CheckMethod.XZ_FOR_3D
				? Vector2Int.RoundToInt(new Vector2(position.x, position.z) / resolution)
				: Vector2Int.RoundToInt(new Vector2(position.x, position.y) / resolution);


	internal void Update()
	{
		// only on server
		if (!NetworkServer.active) return;

		ClearNonAlloc();

		if (NetworkTime.localTime >= lastRebuildTime + rebuildInterval)
		{
			foreach (NetworkConnection connection in NetworkServer.connections.Values)
				if (connection.isAuthenticated && connection.identity != null)
					AddPlayerIntoGrid(connection);

			RebuildAll();
			lastRebuildTime = NetworkTime.localTime;
		}
	}

	public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnection newObserver)
	{
		// calculate projected positions
		Vector2Int projected = ProjectToGrid(identity.transform.position);
		Vector2Int observerProjected = ProjectToGrid(newObserver.identity.transform.position);

		return (projected - observerProjected).sqrMagnitude <= 2;
	}

	public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnection> newObservers, bool initialize)
	{
		Vector2Int key = ProjectToGrid(identity.transform.position);
		if (!CellsObservers.ContainsKey(key))
			CellsObservers.Add(key, new HashSet<NetworkConnection>());

		foreach (NetworkConnection conn in CellsObservers[key])
			newObservers.Add(conn);
	}


	//! Clear hashSets to avoid allocations
	private void ClearNonAlloc()
	{
		foreach (HashSet<NetworkConnection> hash in CellsObservers.Values)
			hash.Clear();
	}

	//* Just adding player into grid
	//* If key is does not exist, we just create new key
	//* And we create cells, which affecting by this player-observer
	private void AddPlayerIntoGrid(NetworkConnection conn)
	{
		Vector2Int position = ProjectToGrid(conn.identity.transform.position);
		foreach (Vector2Int offset in neighbourOffsets)
		{
			Vector2Int key = position + offset;
			if (!CellsObservers.ContainsKey(key))
				CellsObservers.Add(key, new HashSet<NetworkConnection>());

			CellsObservers[key].Add(conn);
		}
	}
#if UNITY_EDITOR || DEVELOPMENT_BUILD
	void OnGUI()
	{
		if (!showSlider) return;
		if (!NetworkServer.active) return;

		int height = 30;
		int width = 250;
		GUILayout.BeginArea(new Rect(Screen.width / 2 - width / 2, Screen.height - height, width, height));
		GUILayout.BeginHorizontal("Box");
		GUILayout.Label("Radius:");
		visRange = Mathf.RoundToInt(GUILayout.HorizontalSlider(visRange, 0, 200, GUILayout.Width(150)));
		GUILayout.Label(visRange.ToString());
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
#endif
}

