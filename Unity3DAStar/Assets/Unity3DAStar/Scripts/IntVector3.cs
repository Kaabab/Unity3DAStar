using System;

public struct IntVector3 : IEquatable<IntVector3>
{
    private int _x;
    private int _y;
    private int _z;

    public int X
    {
        get { return _x; }
        set { _x = value; }
    }

    public int Y
    {
        get { return _y; }
        set { _y = value; }
    }

    public int Z
    {
        get { return _z; }
        set { _z = value; }
    }

    public IntVector3(int x, int y, int z)
    {
        this._x = x;
        this._y = y;
        this._z = z;
    }

    public static bool operator ==(IntVector3 one, IntVector3 other)
    {
        return one.Equals(other);
    }

    public static bool operator !=(IntVector3 one, IntVector3 other)
    {
        return !one.Equals(other);
    }

    public static IntVector3 operator +(IntVector3 one, IntVector3 other)
    {
        return new IntVector3(one._x + other._x, one._y + other._y, one._z + other._z);
    }

    public override int GetHashCode()
    {
        return this._x.GetHashCode() ^ this._y.GetHashCode() << 2 ^ this._z.GetHashCode() >> 2;
    }

    public override string ToString()
    {
        return string.Format("{0},{1},{2}", _x, _y, _z);
    }

    public bool Equals(IntVector3 other)
    {
        return (_x == other._x) && (_y == other._y) && (_z == other._z);
    }

    public override bool Equals(object other)
    {
        return Equals((IntVector3)other);
    }

}

