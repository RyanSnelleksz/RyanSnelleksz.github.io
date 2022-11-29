#pragma once

#include "RigidBody.h"
#include "Box.h"
#include "Circle.h"
#include "Plane.h"

CollisionData CollideCircleToBox(Circle* circle, Box* box);

CollisionData CollideCircleToCircle(Circle* circleA,  Circle* circleB);

CollisionData CollideCircleToPlane(Circle* circle, Plane* plane);

CollisionData CollideBoxToPlane(Box* box, Plane* plane);

CollisionData CollideBoxToBox(Box* boxA, Box* boxB);