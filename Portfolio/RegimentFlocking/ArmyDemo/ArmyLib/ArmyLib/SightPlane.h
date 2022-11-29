#pragma once

#include "glm.hpp"

class SightPlane
{

	// Name: SighPlane
	// Description: A Plane class
	// Interations: Used by clone armies for line of sight checks

public:

	glm::vec2 posA = { 0,0 };
	glm::vec2 posB = { 0,0 };

	glm::vec2 normal = { 0,0 };

	float d = 0;

	SightPlane();

	SightPlane(float xA, float yA, float xB, float yB);

	SightPlane& operator=(const SightPlane& other);
};