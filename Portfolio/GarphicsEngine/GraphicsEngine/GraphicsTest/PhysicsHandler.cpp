
#include "PhysicsHandler.h"

PhysicsHandler::PhysicsHandler()
{

}

PhysicsHandler::~PhysicsHandler()
{
	for (RigidBody* rigidbody : rigidbodies)
	{
		delete rigidbody;
	}
}

void PhysicsHandler::AddRigidBody(RigidBody* newRigidBody)
{
	rigidbodies.push_back(newRigidBody);
}

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// CollisionChecks
// 
// Runs the collision checks and resolutions
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
void PhysicsHandler::CollisionChecks()
{
	for (int i = 0; i < rigidbodies.size(); i++)
	{
		for (int j = i + 1; j < rigidbodies.size(); j++)
		{
			CollisionResolution(CollideTwoRigidbodies(rigidbodies[i], rigidbodies[j]));
		}
	}
}

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// CollideTwoRigidBodies
// 
// Checks the rigidbodies types and runs the correct collision check returning that checks CollisionData
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
CollisionData PhysicsHandler::CollideTwoRigidbodies(RigidBody* A, RigidBody* B)
{
	if (A->GetType() == ShapeType::Circle && B->GetType() == ShapeType::Circle) 	// circle to circle
	{
		return CollideCircleToCircle((Circle*)A, (Circle*)B);
	}
	else if (A->GetType() == ShapeType::Circle && B->GetType() == ShapeType::Box) 	// circle to box
	{
		//return CollideCircleToBox((Circle*)A, (Box*)B);
	}
	else if (A->GetType() == ShapeType::Box && B->GetType() == ShapeType::Circle) 	// box to circle
	{
		//return CollideCircleToBox((Circle*)B, (Box*)A);
	}
	else if (A->GetType() == ShapeType::Circle && B->GetType() == ShapeType::Plane) 	// circle to plane
	{
		return CollideCircleToPlane((Circle*)A, (Plane*)B);
	}
	else if (A->GetType() == ShapeType::Plane && B->GetType() == ShapeType::Circle) 	// plane to circle
	{
		return CollideCircleToPlane((Circle*)B, (Plane*)A);
	}
	else if (A->GetType() == ShapeType::Box && B->GetType() == ShapeType::Box) 	// box to box
	{
		//return CollideBoxToBox((Box*)A, (Box*)B);
	}
	else if (A->GetType() == ShapeType::Box && B->GetType() == ShapeType::Plane) 	// box to plane
	{
		//return CollideBoxToPlane((Box*)A, (Plane*)B);
	}
	else if (A->GetType() == ShapeType::Plane && B->GetType() == ShapeType::Box) 	// plane to box
	{
		//return CollideBoxToPlane((Box*)B, (Plane*)A);
	}
	return CollisionData();
}

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
// Collision Reoslution
// 
// Uses given collision data to resolve the collision
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
void PhysicsHandler::CollisionResolution(CollisionData collision)
{
	if (collision.depth < 0) { return; } // Means no collision

	if (collision.shapeA != nullptr && collision.shapeB != nullptr) // If we have two shapes
	{
		float elasticity = 0.8f;

		float j; // impulse magnitude

		if (collision.shapeB->mass == 0) // If one is static
		{
			collision.shapeA->position = collision.shapeA->position + collision.normal * collision.depth; // Fix penetration
			collision.shapeA->ApplyForce(-((1 + elasticity) * glm::dot(collision.shapeA->velocity, collision.normal) * collision.normal)); // Resolve collision
		}
		else // Else two dynamic are colliding
		{
			glm::vec2 relativeVelocity = collision.shapeA->velocity - collision.shapeB->velocity;

			j = (-(1 + elasticity) * glm::dot(relativeVelocity, collision.normal)) / (glm::dot(collision.normal, collision.normal) * ((1 / collision.shapeA->mass) + (1 / collision.shapeB->mass)));

			collision.shapeA->position = collision.shapeA->position + collision.normal * (collision.depth / 2); // Fix penetration
			collision.shapeB->position = collision.shapeB->position - collision.normal * (collision.depth / 2);

			collision.shapeA->ApplyForce(j / collision.shapeA->mass * collision.normal); // Resolve collision
			collision.shapeB->ApplyForce(-(j / collision.shapeB->mass * collision.normal));
		}
	}
}