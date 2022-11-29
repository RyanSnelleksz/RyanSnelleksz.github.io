#pragma once

#include "Soldier.h"
#include "CloneArmy.h"
#include <vector>

class Regiment
{

public:

	std::vector<Soldier*> soldiers;

	glm::vec2 Position;

	CloneArmy* myClone;

	int width;

	int height;

	float spacing;

	Regiment();

	Regiment(float x, float y, int theWidth, int theHeight, float space);

	~Regiment();
};