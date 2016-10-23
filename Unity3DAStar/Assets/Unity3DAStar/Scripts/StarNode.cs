public class StarNode<GridUnit> : IStarNode
{
    private readonly GridUnit _gridUnit;
    private readonly IntVector3 _position;
    private bool _isWalkable = true;

    public GridUnit Unit
    {
        get { return _gridUnit; }
    }

    public IntVector3 Position
    {
        get { return _position; }
    }

    public StarNode(GridUnit gridUnit, IntVector3 position)
    {
        this._gridUnit = gridUnit;
        this._position = position;
    }

    public virtual float NeighbourDistance(IStarNode target)
    {
        float dx = _position.X - target.Position.X;
        float dy = _position.Y - target.Position.Y;
        float dz = _position.Z - target.Position.Z;
        return (dx * dx) + (dy * dy) + (dz * dz);
    }

    public bool IsWalkable
    {        
        // use the grid unit here to have terrain modifier and such
        get
        {
            return _isWalkable;
        }
        set
        {
            _isWalkable = value;
        }
    }

    public virtual bool CanWalk(object walker)
    {
        return _isWalkable;
    }

    public virtual float GetCostModifier(IStarNode startNode, IStarNode endNode, IStarNode currentNode, object walker)
    {
        // use the grid unit here to have terrain modifier and such
        return 0;
    }
}

