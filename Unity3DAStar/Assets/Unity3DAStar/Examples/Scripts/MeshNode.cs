using UnityEngine;

public class MeshNode : MonoBehaviour
{
    public StarNode<MeshNode> StarNode
    {
        get { return _starNode; }
        set { _starNode = value; }
    }

    public bool IsLocked
    {
        get { return _isLocked; }
        set
        {
            _isLocked = value;
            StarNode.IsWalkable = !_isLocked;
            UpdateState();
        }
    }

    private TextMesh _textMesh;
    private StarNode<MeshNode> _starNode;
    private Color _color;
    private Color _LockColor;
    private bool _isLocked;

    private MeshRenderer _renderer = null;
    private Material _mat = null;

    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _mat = _renderer.material;
        _textMesh = GetComponentInChildren<TextMesh>();
    }

    public Color Color
    {
        get { return _color; }
        set
        {
            _color = value;
            _LockColor = (_color * 0.2f) + (Color.white * 0.8f);
            UpdateColor();
        }
    }

    private void UpdateState()
    {
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (_mat != null)
        {
            if (_isLocked)
            {
                _mat.color = _LockColor;
            }
            else
            {
                _mat.color = _color;
            }
        }
    }

    public string Text
    {
        set
        {
            if (_textMesh != null)
            {

                _textMesh.text = value;
            }
        }
        get
        {
            if (_textMesh != null)
            {
                return _textMesh.text;
            }
            return null;
        }
    }
}
