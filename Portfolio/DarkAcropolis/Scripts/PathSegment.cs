using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSegment
{
    Vector3 pointOne;
    Vector3 pointTwo;

    public PathSegment()
    {

    }

    public PathSegment(Vector3 point1, Vector3 point2)
    {
        pointOne = point1;
        pointTwo = point2;
    }

    public bool LineToLineCollision(PathSegment pathSeg, PathSegment rectSeg)
    {
        //float directionA = ((rectSeg.pointTwo.x - rectSeg.pointOne.x) * (pathSeg.pointOne.y - rectSeg.pointOne.y) - (rectSeg.pointTwo.y - rectSeg.pointOne.y) * (pathSeg.pointOne.x - rectSeg.pointOne.x)) / ((rectSeg.pointTwo.y - rectSeg.pointOne.y) * (pathSeg.pointTwo.x - pathSeg.pointOne.x) - (rectSeg.pointTwo.x - rectSeg.pointOne.x) * (pathSeg.pointTwo.y - pathSeg.pointOne.y));
        //float  directionB = ((pathSeg.pointTwo.x - pathSeg.pointOne.x) * (pathSeg.pointOne.y - rectSeg.pointOne.y) - (pathSeg.pointTwo.y - pathSeg.pointOne.y) * (pathSeg.pointOne.x - rectSeg.pointOne.x)) / ((rectSeg.pointTwo.y - rectSeg.pointOne.y) * (pathSeg.pointTwo.x - pathSeg.pointOne.x) - (rectSeg.pointTwo.x - rectSeg.pointOne.x) * (pathSeg.pointTwo.y - pathSeg.pointOne.y));


        double directionA = (rectSeg.pointTwo.x - rectSeg.pointOne.x) * (pathSeg.pointOne.y - rectSeg.pointOne.y) - (rectSeg.pointTwo.y - rectSeg.pointOne.y) * (pathSeg.pointOne.x - rectSeg.pointOne.x);
        double dirA = (rectSeg.pointTwo.y - rectSeg.pointOne.y) * (pathSeg.pointTwo.x - pathSeg.pointOne.x) - (rectSeg.pointTwo.x - rectSeg.pointOne.x) * (pathSeg.pointTwo.y - pathSeg.pointOne.y);
        double aDivided = directionA / dirA;

        double directionB = (pathSeg.pointTwo.x - pathSeg.pointOne.x) * (pathSeg.pointOne.y - rectSeg.pointOne.y) - (pathSeg.pointTwo.y - pathSeg.pointOne.y) * (pathSeg.pointOne.x - rectSeg.pointOne.x);
        double dirB = (rectSeg.pointTwo.y - rectSeg.pointOne.y) * (pathSeg.pointTwo.x - pathSeg.pointOne.x) - (rectSeg.pointTwo.x - rectSeg.pointOne.x) * (pathSeg.pointTwo.y - pathSeg.pointOne.y);
        double bDivided = directionB / dirB;

        if (directionA >= 0 && directionA <= 1 && directionB >= 0 && directionB <= 1)
        {
            return true;
        }
        return false;
    }

    public bool LineToLineCollision(PathSegment pathSeg, Vector3 point1, Vector3 point2)
    {
        float directionA = ((point2.x - point1.x) * (pathSeg.pointOne.y - point1.y) - (point2.y - point1.y) * (pathSeg.pointOne.x - point1.x)) / ((point2.y - point1.y) * (pathSeg.pointTwo.x - pathSeg.pointOne.x) - (point2.x - point1.x) * (pathSeg.pointTwo.y - pathSeg.pointOne.y));
        float directionB = ((pathSeg.pointTwo.x - pathSeg.pointOne.x) * (pathSeg.pointOne.y - point1.y) - (pathSeg.pointTwo.y - pathSeg.pointOne.y) * (pathSeg.pointOne.x - point1.x)) / ((point2.y - point1.y) * (pathSeg.pointTwo.x - pathSeg.pointOne.x) - (point2.x - point1.x) * (pathSeg.pointTwo.y - pathSeg.pointOne.y));

        if (directionA >= 0 && directionA <= 1 && directionB >= 0 && directionB <= 1)
        {
            return true;
        }
        return false;
    }
}
