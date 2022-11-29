#pragma once

#include "RigidBody.h"
#include "Box.h"
#include "Circle.h"
#include "Plane.h"
#include "GameBase.h"
#include "CollisionFunctions.h"
#include <vector>

class PhysicsHandler
{

public:

	std::vector<RigidBody*> rigidbodies;

	glm::vec2 gravity = {0.0f, 0.0f};

	PhysicsHandler();

	~PhysicsHandler();

	void AddRigidBody(RigidBody* newRigidBody);

	void CollisionChecks();

	CollisionData CollideTwoRigidbodies(RigidBody* A, RigidBody* B);

	void CollisionResolution(CollisionData collision);
};