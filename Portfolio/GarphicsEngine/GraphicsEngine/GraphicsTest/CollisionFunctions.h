#pragma once

#include "RigidBody.h"
#include "Box.h"
#include "Circle.h"
#include "Plane.h"

//CollisionData CollideCircleToBox(Circle* circle, Box* box); // Collide circle to box

CollisionData CollideCircleToCircle(Circle* circleA,  Circle* circleB); // Collide circle to circle

CollisionData CollideCircleToPlane(Circle* circle, Plane* plane); // Collide circle to plane

//CollisionData CollideBoxToPlane(Box* box, Plane* plane); // Collide box to plane

//CollisionData CollideBoxToBox(Box* boxA, Box* boxB); // Collide box to box