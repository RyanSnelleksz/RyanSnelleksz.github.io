
#include "Plane.h"

Plane::Plane()
{
	mass = 0.0f;
}

Plane::Plane(float xA, float yA, float xB, float yB)
{
	mass = 0.0f; // Static

	posA = { xA, yA };
	posB = { xB, yB };

	position = { (posA.x + posB.x) / 2, (posA.y + posB.y) / 2 };

	glm::vec2 v = posB - posA;
	v = glm::normalize(v);

	normal.x = -v.y;
	normal.y = v.x;

	d = glm::dot(-posA, normal);
}

Plane& Plane::operator=(const Plane& other)
{
	posA = other.posA;
	posB = other.posB;

	d = other.d;
	normal = other.normal;

	return *this;
}

void Plane::FixedUpdate(glm::vec2 gravity, float deltaTime)
{
	ApplyForce(gravity);

	velocity -= velocity * linearDrag * deltaTime; // Air Resistance

	if (glm::length(velocity) < 0.01f) // If velocity is small enough, make it 0 to help with jittering
	{
		velocity = glm::vec2(0, 0);
	}

	position += velocity * deltaTime;
}