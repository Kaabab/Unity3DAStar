using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class TestGridController : MonoBehaviour
{
    public Color gridColor;
    public Camera rayCastCamera;

    public StarGrid<MeshNode> Grid
    {
        get { return _grid; }
        set { _grid = value; }
    }
    private MeshNode _startPath = null;
    private MeshNode _endPath = null;
    private StarGrid<MeshNode> _grid = null;
    private List<MeshNode> _currentPath = new List<MeshNode>();
    private bool _cbPending = false;
    private float _startTime;

    public void Update()
    {
        if (_grid == null)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            MeshNode hit = GetHitNode();
            if (hit != null)
            {
                hit.IsLocked = !hit.IsLocked;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            MeshNode hit = GetHitNode();
            if (hit != null)
            {
                if (_startPath == null)
                {
                    _startPath = hit;
                    _startPath.Color = Color.red;
                }
                else if (_endPath == null)
                {
                    if (!_cbPending)
                    {
                        _cbPending = true;
                        _endPath = hit;
                        _endPath.Color = Color.green;
                        RunAStarSearch();
                    }
                }
                else
                {
                    _startPath.Color = Color.black;
                    _endPath.Color = Color.black;
                    _startPath = null;
                    _endPath = null;
                    foreach (MeshNode node in _currentPath)
                    {
                        node.Color = Color.black;
                    }
                    _currentPath.Clear();
                }
            }
        }
    }

    private void RunAStarSearch()
    {
        _startTime = Time.realtimeSinceStartup;
        _grid.SearchPath(_startPath.StarNode.Position, _endPath.StarNode.Position, null, RenderPath);
    }

    public MeshNode GetHitNode()
    {
        if (rayCastCamera == null)
        {
            rayCastCamera = Camera.main;
        }
        Ray ray = rayCastCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                return hit.collider.gameObject.GetComponent<MeshNode>();
            }
        }
        return null;
    }
 
    public void RenderPath(List<MeshNode> completionCallBack)
    {
        Debug.Log("Path computation complete in " + (Time.realtimeSinceStartup - _startTime));
        _cbPending = false;
        _currentPath = new List<MeshNode>();
        foreach (MeshNode cell in completionCallBack)
        {
            if (cell == _startPath || cell == _endPath)
            {
                continue;
            }
            cell.Color = Color.yellow;
            _currentPath.Add(cell);
        }
    }
}

