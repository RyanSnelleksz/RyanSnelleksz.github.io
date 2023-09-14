
#include "Circle.h"

Circle::Circle()
{

}

Circle::Circle(float x, float y, float newRadius)
{
	position = { x, y };
	radius = newRadius;
}

Circle& Circle::operator=(const Circle& other)
{
	position = other.position;
	radius = other.radius;
	velocity = other.velocity;

	return *this;
}

void Circle::FixedUpdate(glm::vec2 gravity, float deltaTime)
{
	ApplyForce(gravity);

	velocity -= velocity * linearDrag * deltaTime; // Air Resistance

	if (glm::length(velocity) < 0.01f) // If velocity is small enough, make it 0 to help with jittering
	{
		velocity = glm::vec2(0, 0);
	}

	position += velocity * deltaTime;
}