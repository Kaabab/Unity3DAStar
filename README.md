# Unity3DAStar

Example project showcasing a multithreaded implementation of AStar for 2D and 3D grids in Unity game engine.

Allows for dynamic grids and cost modifier affecting path calculation, proof of concept, could be extended for use in RTS, RPG, or puzzle games.

Please free to ask any question, or contribute !

## Example code

- Init Grid
```cs
        StarNode<DataType>[,,] dataGrid = new StarNode<DataType>[gridWith, gridHeight, gridDepth];
        // init grid data ...
        IntVector3[] surroundings = StarGrid<DataType>.SUROUNDINGS_3D_6;
        IHeuristic heuristic = Manhattan3D.Instance;
        StarGrid<DataType> starGrid = new StarGrid<DataType>(dataGrid, surroundings, heuristic);
```
- Query path
```cs
        starGrid.SearchPath(startPosition, endPosition, walkingAgent, CompletionCallback);

        public void CompletionCallcack(List<DataType> completionCallBack)
        {
            // handle path
        }
```

## Example scenes

- Left Click to mark location as Unwalkable (Colored in gray), Click again to make Walkable
- Right Click to setup start location (Colored in red)
- Right click again to setup to end location (Colored in green)
- Resulting AStar path should be drawn in yellow
- Happy pathing !
