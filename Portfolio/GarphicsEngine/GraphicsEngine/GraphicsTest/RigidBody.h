#pragma once

#include "Renderer.h"
#include <cfloat>

enum class ShapeType
{
	Box,
	Circle,
	Plane
};

class RigidBody
{

public:

	glm::vec2 position = { 0,0 };
	glm::vec2 velocity = { 0,0 };

	float mass = 1.0f;

	float linearDrag = 0.3f;

	RigidBody();

	RigidBody(float x, float y);

	RigidBody& operator=(const RigidBody& other);

	virtual ShapeType GetType() const = 0;

	virtual void FixedUpdate(glm::vec2 gravity, float deltaTime) = 0;

	void ApplyForce(glm::vec2 force);

	virtual void Draw(Renderer& renderer) const = 0;

};

struct CollisionData
{
	glm::vec2 worldPos;
	glm::vec2 normal;
	float depth;

	RigidBody* shapeA; // Participating shapes
	RigidBody* shapeB;

	CollisionData()
	{
		worldPos = { 0,0 };
		normal = { 0,0 };
		depth = 0;

		shapeA = nullptr;
		shapeB = nullptr;
	}

	CollisionData(glm::vec2 worldPosition, glm::vec2 collisionNormal, float collisionDepth, RigidBody* a, RigidBody* b)
	{
		worldPos = worldPosition;
		normal = collisionNormal;
		depth = collisionDepth;

		shapeA = a;
		shapeB = b;
	}
};