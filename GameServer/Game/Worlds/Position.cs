using NahidaImpact.Proto;
using System;
using System.Collections.Generic;

namespace NahidaImpact.GameServer.Game.Worlds;

public class Position
{
    public static readonly Position Zero = new Position(0, 0, 0);
    public static readonly Position Identity = new Position(0, 0);

    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Position()
    {
    }

    public Position(float x, float y)
    {
        Set(x, y);
    }

    public Position(float x, float y, float z)
    {
        Set(x, y, z);
    }

    public Position(List<float> xyz)
    {
        switch (xyz.Count)
        {
            case 3:
                Z = xyz[2];
                goto case 2;
            case 2:
                Y = xyz[1];
                goto case 1;
            case 1:
                X = xyz[0];
                break;
            case 0:
                break;
        }
    }

    public Position(string p)
    {
        var split = p.Split(',');
        if (split.Length >= 2)
        {
            X = float.Parse(split[0]);
            Y = float.Parse(split[1]);
        }
        if (split.Length >= 3)
        {
            Z = float.Parse(split[2]);
        }
    }

    public Position(Vector vector)
    {
        Set(vector);
    }

    public Position(Position pos)
    {
        Set(pos);
    }

    public Position Set(float x, float y)
    {
        X = x;
        Y = y;
        return this;
    }

    // Deep copy
    public Position Set(Position pos)
    {
        return Set(pos.X, pos.Y, pos.Z);
    }

    public Position Set(Vector pos)
    {
        return Set(pos.X, pos.Y, pos.Z);
    }

    public Position Set(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
        return this;
    }

    public Position Multiply(float value)
    {
        X *= value;
        Y *= value;
        Z *= value;
        return this;
    }

    public Position Add(Position add)
    {
        X += add.X;
        Y += add.Y;
        Z += add.Z;
        return this;
    }

    public Position AddX(float d)
    {
        X += d;
        return this;
    }

    public Position AddY(float d)
    {
        Y += d;
        return this;
    }

    public Position AddZ(float d)
    {
        Z += d;
        return this;
    }

    public Position Subtract(Position sub)
    {
        X -= sub.X;
        Y -= sub.Y;
        Z -= sub.Z;
        return this;
    }

    /** In radians */
    public Position Translate(float dist, float angle)
    {
        X += dist * (float)Math.Sin(angle);
        Y += dist * (float)Math.Cos(angle);
        return this;
    }

    public bool Equal2d(Position other)
    {
        // Y is height
        return X == other.X && Z == other.Z;
    }

    public bool Equal3d(Position other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public double ComputeDistance(Position b)
    {
        double detX = X - b.X;
        double detY = Y - b.Y;
        double detZ = Z - b.Z;
        return Math.Sqrt(detX * detX + detY * detY + detZ * detZ);
    }

    public Position Nearby2d(float range)
    {
        Position position = Clone();
        position.Z += RandomFloatRange(-range, range);
        position.X += RandomFloatRange(-range, range);
        return position;
    }

    public Position TranslateWithDegrees(float dist, float angle)
    {
        angle = (float)Math.PI * angle / 180.0f; // Convert degrees to radians
        X += dist * (float)Math.Sin(angle);
        Y += -dist * (float)Math.Cos(angle);
        return this;
    }

    public Position Clone()
    {
        return new Position(X, Y, Z);
    }

    public override string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }

    public Vector ToProto()
    {
        return new Vector { X = X, Y = Y, Z = Z };
    }

    private static float RandomFloatRange(float min, float max)
    {
        return (float)(Random.Shared.NextDouble() * (max - min) + min);
    }
}