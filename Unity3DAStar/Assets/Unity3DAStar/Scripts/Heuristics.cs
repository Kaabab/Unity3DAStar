using System;
using UnityEngine;

public class Manhattan2D : EmptyConstructorSingleton<Manhattan2D>, IHeuristic
{
    public float Evaluate(IntVector3 start, IntVector3 end)
    {
        return Mathf.Abs(start.X - end.X) + Mathf.Abs(start.Y - end.Y);
    }
}

public class Manhattan3D : EmptyConstructorSingleton<Manhattan3D>, IHeuristic
{
    public float Evaluate(IntVector3 start, IntVector3 end)
    {
        return (Mathf.Abs(start.X - end.X) + Mathf.Abs(start.Y - end.Y) + Mathf.Abs(start.Z - end.Z));
    }
}

public class SameAxis : EmptyConstructorSingleton<SameAxis>, IHeuristic
{
    public float Evaluate(IntVector3 start, IntVector3 end)
    {
        int H = 0;
        if (start.X != end.X)
        {
            H += 1;
        }
        if (start.Y != end.Y)
        {
            H += 1;
        }
        if (start.Z != end.Z)
        {
            H += 1;
        }
        return H;
    }
}

public class EuclideanDistance : EmptyConstructorSingleton<EuclideanDistanceSquared>, IHeuristic
{
    public float Evaluate(IntVector3 start, IntVector3 end)
    {
        return Mathf.Sqrt((Math.Abs(start.X - end.X) * Math.Abs(start.X - end.X)) + (Math.Abs(start.Y - end.Y) * Math.Abs(start.Y - end.Y)) + (Math.Abs(start.Z - end.Z) * Math.Abs(start.Z - end.Z)));
    }
}

public class EuclideanDistanceSquared : EmptyConstructorSingleton<EuclideanDistanceSquared>, IHeuristic
{
    public float Evaluate(IntVector3 start, IntVector3 end)
    {
        return (Math.Abs(start.X - end.X) * Math.Abs(start.X - end.X)) + (Math.Abs(start.Y - end.Y) * Math.Abs(start.Y - end.Y)) + (Math.Abs(start.Z - end.Z) * Math.Abs(start.Z - end.Z));
    }
}

public class DiagonalShortCut : EmptyConstructorSingleton<DiagonalShortCut>, IHeuristic
{
    public float Evaluate(IntVector3 start, IntVector3 end)
    {
        float xDistance = Mathf.Abs(start.X - end.X);
        float yDistance = Mathf.Abs(start.Y - end.Y);

        if (xDistance > yDistance)
        {
            return 14 * xDistance + 10 * (xDistance - yDistance);
        }
        else
        {
            return 14 * xDistance - 10 * (xDistance - yDistance);
        }
    }
}

public interface IHeuristic
{
    float Evaluate(IntVector3 start, IntVector3 end);
}


public abstract class EmptyConstructorSingleton<T> where T : class
{
    private static T instance = null;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = CreateInstance();
            }
            return instance;
        }
    }

    private static T CreateInstance()
    {
        try
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return null;
        }
    }

}
