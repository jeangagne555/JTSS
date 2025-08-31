using JTSS.Core.Track.Enums;
using JTSS.Core.Track.Interfaces;
using JTSS.Core.Track.Models;

namespace JTSS.Core.Track;

/// <summary>
/// Implements the logic for traversing the track network, correctly handling complex geometries.
/// </summary>
public class TrackNavigator : ITrackNavigator
{
    /// <inheritdoc/>
    public NavigationResult? NavigateToNextSegment(ITrackSegment currentSegment, TravelDirection currentDirection)
    {
        ArgumentNullException.ThrowIfNull(currentSegment);

        // 1. Determine which node we are traveling towards.
        ITrackNode? destinationNode = currentDirection switch
        {
            TravelDirection.LeftToRight => currentSegment.RightEndNode,
            TravelDirection.RightToLeft => currentSegment.LeftEndNode,
            _ => throw new ArgumentOutOfRangeException(nameof(currentDirection), "Invalid travel direction.")
        };

        if (destinationNode == null)
        {
            return null; // End of the line.
        }

        // 2. Ask the node for the next segment.
        ITrackSegment? nextSegment = destinationNode.GetValidPaths(currentSegment).FirstOrDefault();

        if (nextSegment == null)
        {
            return null; // Path is blocked or track ends.
        }

        // 3. Determine the new direction of travel on the next segment.
        TravelDirection newDirection;
        if (destinationNode == nextSegment.LeftEndNode)
        {
            // We are entering the new segment at its Left end, so we must be traveling Left-to-Right.
            newDirection = TravelDirection.LeftToRight;
        }
        else if (destinationNode == nextSegment.RightEndNode)
        {
            // We are entering the new segment at its Right end, so we must be traveling Right-to-Left.
            newDirection = TravelDirection.RightToLeft;
        }
        else
        {
            // This indicates a configuration error in the track network.
            throw new InvalidOperationException($"Track topology inconsistency: The destination node '{destinationNode.Id}' is not connected to either end of the next segment '{nextSegment.Id}'.");
        }

        // 4. Return the complete navigation result.
        return new NavigationResult(nextSegment, newDirection);
    }

    /// <inheritdoc/>
    public ITrackPosition? MovePosition(ITrackPosition startPosition, TravelDirection direction, double distanceInMeters)
    {
        ArgumentNullException.ThrowIfNull(startPosition);
        if (distanceInMeters < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(distanceInMeters), "Distance to move must be non-negative.");
        }
        if (distanceInMeters == 0)
        {
            return startPosition;
        }

        ITrackSegment currentSegment = startPosition.Segment;
        double currentDistanceFromLeft = startPosition.DistanceFromLeftEnd;
        TravelDirection currentDirection = direction;
        double distanceRemaining = distanceInMeters;

        while (distanceRemaining > 0)
        {
            double distanceOnThisSegment;
            if (currentDirection == TravelDirection.LeftToRight)
            {
                distanceOnThisSegment = currentSegment.Length - currentDistanceFromLeft;
            }
            else // RightToLeft
            {
                distanceOnThisSegment = currentDistanceFromLeft;
            }

            // --- START OF FIX 1: Use strict inequality ---
            // If the move distance is less than what's available, it finishes here.
            if (distanceRemaining < distanceOnThisSegment)
            {
                double finalDistanceFromLeft = (currentDirection == TravelDirection.LeftToRight)
                    ? currentDistanceFromLeft + distanceRemaining
                    : currentDistanceFromLeft - distanceRemaining;

                return new TrackPosition(currentSegment, finalDistanceFromLeft);
            }
            // --- END OF FIX 1 ---

            // If we reach here, the move will consume the rest of the segment and cross a node.
            distanceRemaining -= distanceOnThisSegment;

            NavigationResult? navigationResult = NavigateToNextSegment(currentSegment, currentDirection);

            if (navigationResult == null)
            {
                return null; // Moved off the end of the track
            }

            // Update state for the next loop iteration
            currentSegment = navigationResult.NextSegment;
            currentDirection = navigationResult.NewDirection;
            currentDistanceFromLeft = (currentDirection == TravelDirection.LeftToRight)
                ? 0.0
                // This was also a potential source of error. It should be the full length.
                : currentSegment.Length;
        }

        // --- START OF FIX 2: Handle landing exactly on a node ---
        // If the loop finishes, it means distanceRemaining is exactly 0. The final position
        // is the start of the 'currentSegment' we just transitioned to.
        return new TrackPosition(currentSegment, currentDistanceFromLeft);
        // --- END OF FIX 2 ---
    }

    /// <inheritdoc/>
    public double? GetDistanceBetween(ITrackPosition positionA, ITrackPosition positionB)
    {
        ArgumentNullException.ThrowIfNull(positionA);
        ArgumentNullException.ThrowIfNull(positionB);

        // --- Case 1: Both positions are on the same segment. ---
        if (positionA.Segment.Id == positionB.Segment.Id)
        {
            return Math.Abs(positionA.DistanceFromLeftEnd - positionB.DistanceFromLeftEnd);
        }

        // --- Case 2: Positions are on different segments. Use BFS to find the shortest path. ---
        var queue = new Queue<(ITrackSegment, ITrackNode, double)>();
        var visitedNodes = new Dictionary<string, double>();

        // --- Initialize the search from positionA ---
        if (positionA.Segment.LeftEndNode != null)
        {
            double distToLeftNode = positionA.DistanceFromLeftEnd;
            queue.Enqueue((positionA.Segment, positionA.Segment.LeftEndNode, distToLeftNode));
            visitedNodes[positionA.Segment.LeftEndNode.Id] = distToLeftNode;
        }
        if (positionA.Segment.RightEndNode != null)
        {
            double distToRightNode = positionA.Segment.Length - positionA.DistanceFromLeftEnd;
            queue.Enqueue((positionA.Segment, positionA.Segment.RightEndNode, distToRightNode));
            visitedNodes[positionA.Segment.RightEndNode.Id] = distToRightNode;
        }

        // --- Start the Breadth-First Search ---
        while (queue.Any())
        {
            var (fromSegment, currentNode, distanceToCurrentNode) = queue.Dequeue();

            foreach (var nextSegment in currentNode.GetValidPaths(fromSegment))
            {
                // --- Check if this next segment is our destination. ---
                if (nextSegment.Id == positionB.Segment.Id)
                {
                    double finalDistanceOnSegment;

                    // --- START OF FIX ---
                    // The node we arrived at on the new segment is the currentNode from the previous segment.
                    if (currentNode == positionB.Segment.LeftEndNode) // Arrived at the left end
                    {
                        // The distance needed is from the left end to the position.
                        finalDistanceOnSegment = positionB.DistanceFromLeftEnd;
                    }
                    else // Arrived at the right end
                    {
                        // The distance needed is from the right end to the position.
                        finalDistanceOnSegment = positionB.Segment.Length - positionB.DistanceFromLeftEnd;
                    }
                    // --- END OF FIX ---

                    return distanceToCurrentNode + finalDistanceOnSegment;
                }

                // --- If not the destination, continue the search. ---
                var nextNode = nextSegment.GetOppositeNode(currentNode);
                if (nextNode != null && !visitedNodes.ContainsKey(nextNode.Id))
                {
                    double distanceToNextNode = distanceToCurrentNode + nextSegment.Length;
                    visitedNodes[nextNode.Id] = distanceToNextNode;
                    queue.Enqueue((nextSegment, nextNode, distanceToNextNode));
                }
            }
        }

        // If the queue becomes empty and we haven't found the destination, there is no path.
        return null;
    }

    /// <inheritdoc/>
    public bool ArePositionsApproximatelyEqual(ITrackPosition positionA, ITrackPosition positionB)
    {
        if (ReferenceEquals(positionA, positionB)) return true;
        if (positionA is null || positionB is null) return false;

        // The GetDistanceBetween method already handles all the complexity of
        // same-segment, different-segment, and cross-node calculations.
        double? distance = GetDistanceBetween(positionA, positionB);

        // If there is no path, they are not equal.
        if (!distance.HasValue) return false;

        // They are approximately equal if the distance between them is within our tolerance.
        return distance.Value <= TrackPrecision.Tolerance;
    }
}
