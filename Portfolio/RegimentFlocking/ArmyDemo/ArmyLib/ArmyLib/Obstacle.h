#pragma once

#include "glm.hpp"

struct Obstacle
{

public:

	glm::vec2 position;
	float width;
	float height;

	float minX;
	float maxX;

	float minY;
	float maxY;

// Name: Obstacle
// Parameter: a vec2 and width and height values
// Description: Used to represent terrain for line of sight checks
// Return Value:
	Obstacle(glm::vec2 pos, float w, float h)
	{
		position = pos;
		width = w;
		height = h;

		minX = pos.x;
		minY = pos.y;

		maxX = pos.x + width;
		maxY = pos.y + height;
	}
};