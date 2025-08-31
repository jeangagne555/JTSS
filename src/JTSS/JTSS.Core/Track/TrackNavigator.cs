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
            // Calculate the distance available on the current segment in the direction of travel.
            double distanceOnThisSegment;
            if (currentDirection == TravelDirection.LeftToRight)
            {
                distanceOnThisSegment = currentSegment.Length - currentDistanceFromLeft;
            }
            else // RightToLeft
            {
                distanceOnThisSegment = currentDistanceFromLeft;
            }

            // --- Case 1: The move finishes on the current segment. ---
            if (distanceRemaining <= distanceOnThisSegment)
            {
                double finalDistanceFromLeft = (currentDirection == TravelDirection.LeftToRight)
                    ? currentDistanceFromLeft + distanceRemaining
                    : currentDistanceFromLeft - distanceRemaining;

                return new TrackPosition(currentSegment, finalDistanceFromLeft);
            }

            // --- Case 2: The move will cross over to the next segment. ---

            // Consume the distance from this segment.
            distanceRemaining -= distanceOnThisSegment;

            // Find the next segment.
            NavigationResult? navigationResult = NavigateToNextSegment(currentSegment, currentDirection);

            // If there's no next segment, we've run off the track.
            if (navigationResult == null)
            {
                return null;
            }

            // Update our state to be on the new segment.
            currentSegment = navigationResult.NextSegment;
            currentDirection = navigationResult.NewDirection;

            // The new position is at the very start of the new segment, from the perspective of our travel direction.
            currentDistanceFromLeft = (currentDirection == TravelDirection.LeftToRight)
                ? 0.0
                : currentSegment.Length;
        }

        // This should not be reached if distanceInMeters > 0, but provides a safe fallback.
        return null;
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

        // A queue for our search state: (fromSegment, atNode, distanceToThisNode)
        var queue = new Queue<(ITrackSegment, ITrackNode, double)>();

        // A dictionary to track visited nodes and the shortest distance to them.
        var visitedNodes = new Dictionary<string, double>();

        // --- Initialize the search from positionA ---

        // Path towards the Left node
        if (positionA.Segment.LeftEndNode != null)
        {
            double distToLeftNode = positionA.DistanceFromLeftEnd;
            queue.Enqueue((positionA.Segment, positionA.Segment.LeftEndNode, distToLeftNode));
            visitedNodes[positionA.Segment.LeftEndNode.Id] = distToLeftNode;
        }

        // Path towards the Right node
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

            // Find all possible next segments from the current node
            foreach (var nextSegment in currentNode.GetValidPaths(fromSegment))
            {
                // --- Check if this next segment is our destination. ---
                if (nextSegment.Id == positionB.Segment.Id)
                {
                    double finalDistanceOnSegment;
                    // We need to know which node we arrived at on the destination segment
                    var arrivalNode = nextSegment.GetOppositeNode(currentNode);
                    if (arrivalNode == positionB.Segment.LeftEndNode) // Arrived at the left
                    {
                        finalDistanceOnSegment = positionB.DistanceFromLeftEnd;
                    }
                    else // Arrived at the right
                    {
                        finalDistanceOnSegment = positionB.Segment.Length - positionB.DistanceFromLeftEnd;
                    }

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
}
