
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

	if (dead = true)
	{
		velocity -= velocity * linearDrag * deltaTime;
	}

	if (glm::length(velocity) < 0.01f)
	{
		velocity = glm::vec2(0, 0);
	}

	position += velocity * deltaTime;
}