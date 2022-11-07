using System;
using System.Collections.Generic;
using UnityEngine;

namespace Levels.Tiles
{
#if UNITY_EDITOR
    public enum Direction
    {
        East,
        NorthEast,
        North,
        NorthWest,
        West,
        SouthWest,
        South,
        SouthEast
    }
#endif

    public abstract class NeighborTile : Tile
    {
#if UNITY_EDITOR
        protected class Neighbors : Dictionary<Direction, NeighborTile>
        {
            public Neighbors(Dictionary<Direction, NeighborTile> neighbors) : base(neighbors)
            {
            }
        }

        protected Neighbors GetNeighbors()
        {
            Vector2 position = transform.position;

            return new Neighbors(new Dictionary<Direction, NeighborTile>
            {
                [Direction.East] = OverlapTile<NeighborTile>(position, Vector2.right, Snap, mask),
                [Direction.NorthEast] = OverlapTile<NeighborTile>(position, Vector2.up + Vector2.right, Snap, mask),
                [Direction.North] = OverlapTile<NeighborTile>(position, Vector2.up, Snap, mask),
                [Direction.NorthWest] = OverlapTile<NeighborTile>(position, Vector2.up + Vector2.left, Snap, mask),
                [Direction.West] = OverlapTile<NeighborTile>(position, Vector2.left, Snap, mask),
                [Direction.SouthWest] = OverlapTile<NeighborTile>(position, Vector2.down + Vector2.left, Snap, mask),
                [Direction.South] = OverlapTile<NeighborTile>(position, Vector2.down, Snap, mask),
                [Direction.SouthEast] = OverlapTile<NeighborTile>(position, Vector2.down + Vector2.right, Snap, mask)
            });
        }

        protected static bool HasNeighbor(Direction direction, Dictionary<Direction, NeighborTile> neighbors)
        {
            return neighbors.ContainsKey(direction) && neighbors[direction] != null;
        }

        protected static Direction Rotate(Direction direction, int angle = 45)
        {
            if (angle <= 0)
                return direction;

            if (angle >= 45)
                direction = Rotate(direction, angle - 45);

            switch (direction)
            {
                case Direction.East:
                    return Direction.NorthEast;
                case Direction.NorthEast:
                    return Direction.North;
                case Direction.North:
                    return Direction.NorthWest;
                case Direction.NorthWest:
                    return Direction.West;
                case Direction.West:
                    return Direction.SouthWest;
                case Direction.SouthWest:
                    return Direction.South;
                case Direction.South:
                    return Direction.SouthEast;
                case Direction.SouthEast:
                    return Direction.East;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        public override void Refresh()
        {
            Neighbors neighbors = GetNeighbors();

            Decorate(neighbors);

            foreach (NeighborTile neighbor in neighbors.Values)
            {
                if (neighbor == null)
                    continue;

                neighbor.Decorate(neighbor.GetNeighbors());
            }
        }

        protected abstract void Decorate(Neighbors neighbors);
#endif
    }
}