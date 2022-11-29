#pragma once

#include "glm.hpp"
#include <gtx/norm.hpp>
#include "WarManager.h"

class CloneArmy;

class Clone
{

	// Name: Clones
	// Description: Holds pointers to given units data and does the movement of units
	// Interations: Is used by the clone armies

public:

	float maxSpeed;

	float acceleration = 0.1f;

	glm::vec2* position;

	glm::vec2* velocity;

	glm::vec2 targetPosition;
	bool inPosition = true;

	CloneArmy* myArmy;

	Clone();

	Clone(float myMaxSpeed, glm::vec2* myPos, glm::vec2* myVel, CloneArmy* army);

	~Clone();

	void Movement();

	bool CheckOthers();
};