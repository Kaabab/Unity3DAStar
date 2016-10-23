public interface IStarNode
{
    bool IsWalkable
    {
        get;
        set;
    }

    IntVector3 Position { get; }

    bool CanWalk(object walker);

    float NeighbourDistance(IStarNode target);

    float GetCostModifier(IStarNode startNode, IStarNode endNode, IStarNode currentNode, object walker);
}

