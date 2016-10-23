using System;
using System.Collections.Generic;
using System.Linq;

public class StarGrid<GridUnit>
{

    public static readonly IntVector3[] SUROUNDINGS_2D_4 = new IntVector3[]{  new IntVector3(0,1,0),  new IntVector3(0,-1,0),
                                                                                new IntVector3(-1,0,0),  new IntVector3(1,0,0)
                                                                            };

    public static readonly IntVector3[] SUROUDINGS_2D_8 = new IntVector3[]{   new IntVector3(-1,1,0), new IntVector3(0,1,0), new IntVector3(1,1,0),
                                                                                new IntVector3(-1,0,0),  new IntVector3(1,0,0),
                                                                                new IntVector3(-1,-1,0), new IntVector3(0,-1,0), new IntVector3(1,-1,0),
                                                                            };

    public static readonly IntVector3[] SUROUNDINGS_3D_6 = new IntVector3[]{  new IntVector3(0,1,0),  new IntVector3(0,-1,0),
                                                                                new IntVector3(-1,0,0),  new IntVector3(1,0,0),
                                                                                new IntVector3(0,0,1),  new IntVector3(0,0,-1)
                                                                            };

    public static readonly IHeuristic DEFAULT_HEURISTIC = DiagonalShortCut.Instance;
    public static readonly IntVector3[] DEFAULT_SURROUNDINGS = SUROUNDINGS_2D_4;

    private readonly StarNode<GridUnit>[,,] _dataGrid;
    private readonly IntVector3 _gridSize;
    private readonly IntVector3[] _surroundings;
    private IHeuristic _heuristic;
    private RunnableQueue _runnableQueue;

    public StarGrid(StarNode<GridUnit>[,,] dataGrid, IntVector3[] surroundings = null, IHeuristic heuristic = null, RunnableQueue runnableQueue = null, bool useBinarySeach = true)
    {
        this._dataGrid = dataGrid;
        _gridSize = new IntVector3(this._dataGrid.GetLength(0), this._dataGrid.GetLength(1), this._dataGrid.GetLength(2));
        this._surroundings = surroundings ?? DEFAULT_SURROUNDINGS;
        this._heuristic = heuristic ?? DEFAULT_HEURISTIC;
        this._runnableQueue = runnableQueue;
    }

    public void SearchPath(IntVector3 startPosition, IntVector3 endPosition, object agent, Action<List<GridUnit>> completionCallBack)
    {
        if (_runnableQueue != null)
        {
            _runnableQueue.ThreadPooledQueue.Enqueue(() =>
                 {
                     SearchPathInternal(startPosition, endPosition, agent, completionCallBack);
                 }
             );
        }
        else
        {
            SearchPathInternal(startPosition, endPosition, agent, completionCallBack);
        }
    }

    private void SearchPathInternal(IntVector3 startPosition, IntVector3 endPosition, object agent, Action<List<GridUnit>> completionCallBack)
    {
        RuntimeNode[,,] runtimeGrid = new RuntimeNode[_gridSize.X > 0 ? _gridSize.X : 1, _gridSize.Y > 0 ? _gridSize.Y : 1, _gridSize.Z > 0 ? _gridSize.Z : 1];
        StarNode<GridUnit> endStarNode = _dataGrid[endPosition.X, endPosition.Y, endPosition.Z];
        StarNode<GridUnit> startStarNode = _dataGrid[startPosition.X, startPosition.Y, startPosition.Z];
        RuntimeNode currentNode = new RuntimeNode(startPosition, startStarNode);
        currentNode.G = 0;
        currentNode.H = _heuristic.Evaluate(startPosition, endPosition);
        currentNode.F = currentNode.H;
        runtimeGrid[startPosition.X, startPosition.Y, startPosition.Z] = currentNode;
        OpenList openList = new OpenList();
        openList.Enqueue(currentNode);
        while (!openList.IsEmpty())
        {
            currentNode = openList.Dequeue();
            if (currentNode.Position.Equals(endPosition))
            {
                ConstructPath(currentNode, completionCallBack);
                return;
            }
            IntVector3 tempPosition;
            foreach (IntVector3 offset in _surroundings)
            {
                tempPosition = currentNode.Position + offset;
                if (IsWithinBounds(tempPosition))
                {
                    RuntimeNode neighbour = runtimeGrid[tempPosition.X, tempPosition.Y, tempPosition.Z];
                    if (neighbour == null)
                    {
                        neighbour = new RuntimeNode(tempPosition, _dataGrid[tempPosition.X, tempPosition.Y, tempPosition.Z]);
                        runtimeGrid[neighbour.Position.X, neighbour.Position.Y, neighbour.Position.Z] = neighbour;
                    }
                    if (!neighbour.node.IsWalkable || !neighbour.node.CanWalk(agent))
                    {
                        neighbour.OnClosedList = true;
                        neighbour.OnOpenList = false;
                    }
                    if (neighbour.OnClosedList)
                    {
                        continue;
                    }
                    float G = currentNode.G + currentNode.node.NeighbourDistance(neighbour.node) + currentNode.node.GetCostModifier(startStarNode, endStarNode, currentNode.node, agent);
                    if (!neighbour.OnOpenList || G < neighbour.G)
                    {
                        float H = _heuristic.Evaluate(tempPosition, endPosition);
                        float F = G + H;
                        neighbour.Parent = currentNode;
                        neighbour.G = G;
                        neighbour.H = H;
                        neighbour.F = F;
                        if (!neighbour.OnOpenList)
                        {
                            neighbour.Parent = currentNode;
                            openList.Enqueue(neighbour);
                        }
                    }
                }
            }
        }
        ConstructPath(null, completionCallBack);
    }

    private void ConstructPath(RuntimeNode neighbour, Action<List<GridUnit>> completionCallBack)
    {
        List<GridUnit> path = new List<GridUnit>();
        if (neighbour != null)
        {
            RuntimeNode node = neighbour;
            while (node != null)
            {
                path.Add(node.node.Unit);
                node = node.Parent;
            }
            path.Reverse();
        }
        if (_runnableQueue != null)
        {
            _runnableQueue.UpdateQueue.Enqueue(() =>
            {
                completionCallBack(path);
            });
        }
        else
        {
            completionCallBack(path);
        }
    }

    private bool IsWithinBounds(IntVector3 position)
    {
        if (position.X >= 0 && position.X < _gridSize.X
            && position.Y >= 0 && position.Y < _gridSize.Y
            && position.Z >= 0 && position.Z < _gridSize.Z)
        {
            return true;
        }
        return false;
    }

    private class OpenList
    {
        private List<RuntimeNode> _internalArray = new List<RuntimeNode>();

        public void Enqueue(RuntimeNode node)
        {
            _internalArray.Add(node);
        }

        public void Clear()
        {
            _internalArray.Clear();
        }

        public RuntimeNode Dequeue()
        {
            if (_internalArray.Count > 0)
            {
                RuntimeNode dequeue = _internalArray.Aggregate((item, minObject) => item.F < minObject.F ? item : minObject);
                _internalArray.Remove(dequeue);
                dequeue.OnOpenList = false;
                dequeue.OnClosedList = true;
                return dequeue;
            }
            return null;
        }

        public bool IsEmpty()
        {
            return _internalArray.Count <= 0;
        }

        private class RuntimeNodeComparer : IComparer<RuntimeNode>
        {
            public int Compare(RuntimeNode x, RuntimeNode y)
            {
                return (int)(x.F - y.F);
            }
        }
    }

    public class RuntimeNode
    {
        public IntVector3 Position { get { return _position; } }
        public StarNode<GridUnit> node { get { return _node; } }
        public float G { get; set; }
        public float H { get; set; }
        public float F { get; set; }
        public RuntimeNode Parent { get; set; }
        public bool OnClosedList { get; set; }
        public bool OnOpenList { get; set; }

        private readonly IntVector3 _position;
        private readonly StarNode<GridUnit> _node;

        public RuntimeNode(IntVector3 position, StarNode<GridUnit> node)
        {
            this._position = position;
            this._node = node;
        }
    }
}

