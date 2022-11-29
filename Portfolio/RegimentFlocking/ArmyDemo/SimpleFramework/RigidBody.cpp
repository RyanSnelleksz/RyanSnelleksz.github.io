
#include "RigidBody.h"

RigidBody::RigidBody()
{
	position = { 0,0 };
}

RigidBody::RigidBody(float x, float y)
{
	position = { x, y };
}

RigidBody& RigidBody::operator=(const RigidBody& other)
{
	position = other.position;

	return *this;
}

void RigidBody::ApplyForce(glm::vec2 force)
{
	if (mass != 0)
	{
		velocity += force / mass;
	}
}