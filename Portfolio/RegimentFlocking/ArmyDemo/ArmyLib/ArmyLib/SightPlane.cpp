
#include "SightPlane.h"

SightPlane::SightPlane()
{

}

SightPlane::SightPlane(float xA, float yA, float xB, float yB)
{
	posA = { xA, yA };
	posB = { xB, yB };

	glm::vec2 v = posB - posA;
	v = glm::normalize(v);

	normal.x = -v.y;
	normal.y = v.x;

	d = glm::dot(-posA, normal);
}

SightPlane& SightPlane::operator=(const SightPlane& other)
{
	posA = other.posA;
	posB = other.posB;

	d = other.d;
	normal = other.normal;

	return *this;
}