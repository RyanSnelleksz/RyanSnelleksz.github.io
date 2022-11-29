#pragma once

#include <vector>
#include "glm.hpp"

class Node
{
public:

	glm::vec3 position; // x and y are for coordinates, for z however, 0 means not accessible and 1 means accessible

	Node* connections[4];

	Node();

	Node(float x, float y, int z);

	~Node();
};