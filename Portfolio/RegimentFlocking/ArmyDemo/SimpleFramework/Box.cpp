
#include "Box.h"

Box::Box()
{

}

Box::Box(float minX, float maxX, float minY, float maxY)
{
	xMin = minX;
	xMax = maxX;
	yMin = minY;
	yMax = maxY;

	mass = 0;

	position = { (xMax + xMin) / 2, (yMax + yMin) / 2 };
}

Box& Box::operator=(const Box& other)
{
	xMin = other.xMin;
	xMax = other.xMax;
	yMin = other.yMin;
	yMax = other.yMax;

	position = other.position;
	velocity = other.velocity;

	return *this;
}

void Box::FixedUpdate(glm::vec2 gravity, float deltaTime)
{
	ApplyForce(gravity);

	velocity -= velocity * linearDrag * deltaTime;

	if (glm::length(velocity) < 0.01f)
	{
		velocity = glm::vec2(0, 0);
	}

	//position += velocity * deltaTime;

	//xMin = position.x - 1;
	//xMax = position.x + 1;
	//yMin = position.y - 1;
	//yMax = position.y + 1;
}