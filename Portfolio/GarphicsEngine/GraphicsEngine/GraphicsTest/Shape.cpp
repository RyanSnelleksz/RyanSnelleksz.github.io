
#include "Shape.h"

Shape::Shape()
{
	center = { 0,0 };
}

Shape::Shape(float x, float y)
{
	center = { x, y };
}

Shape& Shape::operator=(const Shape& other)
{
	center = other.center;

	return *this;
}