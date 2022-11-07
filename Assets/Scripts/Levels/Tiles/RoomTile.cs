using UnityEngine;

namespace Levels.Tiles
{
    public class RoomTile : NeighborTile
    {
#if UNITY_EDITOR
        [SerializeField] private Transform wall;
        [SerializeField] private Transform cornerOuter;
        [SerializeField] private Transform cornerInner;

        protected override void Decorate(Neighbors neighbors)
        {
            ClearDecorations();

            Transform decorations = new GameObject("Decorations").transform;
            decorations.parent = transform;

            if (neighbors.Count > 0)
            {
                for (int angle = 0; angle < 360; angle += 90)
                {
                    bool east = HasNeighbor(Rotate(Direction.East, angle), neighbors);
                    bool northEast = HasNeighbor(Rotate(Direction.NorthEast, angle), neighbors);
                    bool north = HasNeighbor(Rotate(Direction.North, angle), neighbors);
                    bool northWest = HasNeighbor(Rotate(Direction.NorthWest, angle), neighbors);
                    bool west = HasNeighbor(Rotate(Direction.West, angle), neighbors);
                    bool southWest = HasNeighbor(Rotate(Direction.SouthWest, angle), neighbors);
                    bool south = HasNeighbor(Rotate(Direction.South, angle), neighbors);
                    // bool southEast = HasNeighbor(Rotate(Direction.SouthEast, angle), neighbors);

                    if (west && south)
                    {
                        if (!north && !east)
                            SpawnDecoration(cornerOuter, angle, decorations);
                        if (!southWest)
                            SpawnDecoration(cornerInner, angle, decorations);
                    }

                    if (!north && !northWest && !northEast && (west || (east && !south)))
                        SpawnDecoration(wall, angle, decorations);
                }
            }
        }

        private void ClearDecorations()
        {
            Transform decorations = transform.Find("Decorations");

            if (decorations == null)
                return;

            DestroyImmediate(decorations.gameObject);
        }

        private void SpawnDecoration(Transform prefab, int rotation, Transform parent)
        {
            Transform spawned = SpawnPrefab(prefab, transform.position, Quaternion.Euler(0, 0, rotation));
            spawned.parent = parent;
        }
#endif
    }
}