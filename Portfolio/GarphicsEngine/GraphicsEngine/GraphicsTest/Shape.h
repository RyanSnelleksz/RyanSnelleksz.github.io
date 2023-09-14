#pragma once

#include "ShaderProgram.h"
#include <cfloat>

enum class ShapeType
{
	Square,
	Circle,
	Plane
};

class Shape
{

public:

	glm::vec2 center = { 0,0 };
	glm::vec2 velocity = { 0,0 };

	Shape();

	Shape(float x, float y);

	Shape& operator=(const Shape& other);

	virtual ShapeType GetType() const = 0;

};

struct CollisionData
{
	glm::vec2 worldPos;
	glm::vec2 normal;
	float depth;

	const Shape* shapeA;
	Shape* shapeB;

	CollisionData()
	{
		worldPos = { 0,0 };
		normal = { 0,0 };
		depth = 0;

		shapeA = nullptr;
		shapeB = nullptr;
	}

	CollisionData(glm::vec2 worldPosition, glm::vec2 collisionNormal, float collisionDepth, Shape* a, Shape* b)
	{
		worldPos = worldPosition;
		normal = collisionNormal;
		depth = collisionDepth;

		shapeA = a;
		shapeB = b;
	}
};