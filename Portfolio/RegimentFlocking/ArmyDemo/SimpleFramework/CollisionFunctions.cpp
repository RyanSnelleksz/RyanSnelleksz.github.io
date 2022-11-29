
#include "CollisionFunctions.h"
#include <vector>

CollisionData CollideCircleToBox(Circle* circle, Box* box)
{
	CollisionData collisionData;

	collisionData.shapeA = circle;
	collisionData.shapeB = box;

	glm::vec2 clampedPos = circle->position;

	if (clampedPos.x < box->xMin) { clampedPos.x = box->xMin; }
	if (clampedPos.x > box->xMax) { clampedPos.x = box->xMax; }
	if (clampedPos.y < box->yMin) { clampedPos.y = box->yMin; }
	if (clampedPos.y > box->yMax) { clampedPos.y = box->yMax; }

	collisionData.worldPos = clampedPos;

	if (glm::distance(clampedPos, circle->position) <= circle->radius)
	{
		if (circle->position.x > box->xMin && circle->position.x < box->xMax && circle->position.y > box->yMin && circle->position.y < box->yMax)
		{
			collisionData.normal = glm::normalize(circle->position - box->position);
		}
		else
		{
			collisionData.normal = glm::normalize(circle->position - clampedPos);
		}
		collisionData.depth = (glm::distance(clampedPos, circle->position) - circle->radius) * -1;
	}
	else
	{
		collisionData.depth = -1;
	}

	return collisionData;
}

CollisionData CollideCircleToCircle(Circle* circleA, Circle* circleB)
{
	CollisionData collisionData;

	collisionData.shapeA = circleA;
	collisionData.shapeB = circleB;

	float distance = glm::distance(circleA->position, circleB->position);

	collisionData.depth = -1;

	if (distance <= circleA->radius + circleB->radius)
	{
		collisionData.worldPos = (circleB->position + circleA->position) / 2.0f;

		collisionData.normal = glm::normalize(circleA->position - circleB->position);
		collisionData.depth = (distance - (circleA->radius + circleB->radius)) * -1;
	}

	return collisionData;
}

CollisionData CollideCircleToPlane(Circle* circle, Plane* plane)
{
	CollisionData collisionData;

	collisionData.shapeA = circle;
	collisionData.shapeB = plane;

	float distanceToPlane = glm::dot(circle->position, plane->normal) + plane->d;

	if (distanceToPlane <= circle->radius && distanceToPlane >= -circle->radius) // intersects
	{
		glm::vec2 touchingPoint = circle->position - plane->normal * distanceToPlane;

		collisionData.worldPos = touchingPoint;

		collisionData.normal = glm::normalize(circle->position - touchingPoint);

		collisionData.depth = (glm::distance(touchingPoint, circle->position) - circle->radius) * -1;
	}
	else if(distanceToPlane > circle->radius) // if infront
	{
		collisionData.depth = -1;
	}
	else if (distanceToPlane < -circle->radius) // if behind
	{
		glm::vec2 touchingPoint = circle->position - plane->normal * distanceToPlane;

		collisionData.worldPos = touchingPoint;

		collisionData.normal = -1.0f * (glm::normalize(circle->position - touchingPoint));

		collisionData.depth = (glm::distance(touchingPoint, circle->position) + circle->radius);
	}

	return collisionData;
}

CollisionData CollideBoxToPlane(Box* box, Plane* plane)
{
	CollisionData collisionData;

	collisionData.shapeA = box; // Pointers to shapes
	collisionData.shapeB = plane;

	collisionData.normal = plane->normal; // collision normal is the same as plane normal

	std::vector<glm::vec2> points; // box corners


	points.push_back({ box->xMin, box->yMin });
	points.push_back({ box->xMax, box->yMin });
	points.push_back({ box->xMax, box->yMax });
	points.push_back({ box->xMin, box->yMax });

	float distanceToPlane = -FLT_MAX;
	collisionData.depth = -1;

	glm::vec2 farthestPoint = { 0,0 };
	bool touching = false;

	for (glm::vec2 point : points)
	{
		distanceToPlane = -1 * (glm::dot(point, plane->normal) + plane->d);
		if (distanceToPlane >= 0)
		{
			if (distanceToPlane > collisionData.depth)
			{
				farthestPoint = point;
				collisionData.depth = distanceToPlane;
				touching = true;
			}
		}
	}

	if (touching == true)
	{
		collisionData.worldPos = farthestPoint; // world pos
		collisionData.depth = -1 * (glm::dot(farthestPoint, plane->normal) + plane->d); // depth
	}

	return collisionData;
}

CollisionData CollideBoxToBox(Box* boxA, Box* boxB)
{
	CollisionData collisionData;

	collisionData.shapeA = boxA; // Pointers to shapes
	collisionData.shapeB = boxB;

	collisionData.depth = -1;

	if (!(boxA->xMax < boxB->xMin || boxA->yMax < boxB->yMin || boxA->xMin > boxB->xMax || boxA->yMin > boxB->yMax))
	{
		float horizontalDepth = boxA->xMax - boxB->xMin < boxB->xMax - boxA->xMin ? boxA->xMax - boxB->xMin : boxB->xMax - boxA->xMin;
		float verticalDepth = boxA->yMax - boxB->yMin < boxB->yMax - boxA->yMin ? boxA->yMax - boxB->yMin : boxB->yMax - boxA->yMin;

		if (horizontalDepth < verticalDepth)  // depth
		{
			collisionData.depth = horizontalDepth;

			if (boxA->position.x < boxB->position.x)
			{
				collisionData.normal = { -1, 0 }; // normal
			}
			else
			{
				collisionData.normal = { 1, 0 }; // normal
			}
		}
		else
		{
			collisionData.depth = verticalDepth;

			if (boxA->position.y < boxB->position.y)
			{
				collisionData.normal = { 0, -1 }; // normal
			}
			else
			{
				collisionData.normal = { 0, 1 }; // normal
			}
		}
	}
	else
	{

	}

	return collisionData;
}