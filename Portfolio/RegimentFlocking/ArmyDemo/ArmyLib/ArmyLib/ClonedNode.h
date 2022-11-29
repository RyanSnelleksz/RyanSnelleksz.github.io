#pragma once

#include "glm.hpp"

class ClonedNode
{

	// Name: ClonedNode
	// Description: Used to represent nodes in the map
	// Interations: Used by pathfinding and mapcopy

public:

	glm::vec3 position; // x and y are for coordinates, for z however, 0 means not accessible and 1 means accessible

	ClonedNode* connections[4];
	ClonedNode* previous = nullptr;

	float weight = 1.0f;
	float gScore = 0.0f;

	ClonedNode();

	ClonedNode(float x, float y, int z);

	~ClonedNode();
};