#pragma once

#include <utility>
#include <gtx/norm.hpp>
#include "WarManager.h"
#include "glm.hpp"
#include "SightPlane.h"
#include <vector>

class Clone;
class ClonedNode;
class WarManager;

enum Stance
{
	Attack = 0,
	AttackThisTarget = 1,
	Hold = 2,
	Move = 3, 
	Flee = 4
};

class CloneArmy
{

	// Name: CloneArmy
	// Description: Governs the movement of the clones and uses stances to decide what it does
	// Interations: Uses essentially all of the other classes

public:

	WarManager* myManager;
	int managerIndex;
	int teamNumber = 0;

	std::vector<Clone*> clones;
	Clone* captain = nullptr;

	std::vector<ClonedNode*> myPath;
	int routeIndex = 0;

	glm::vec2 armyDesiredPosition;
	glm::vec2 armyCurrentPosition;
	bool moved = false;
	bool rotated = false;
	bool rotating = false;
	bool startedMoving = false;
	bool hasNewOrders = false;

	CloneArmy* targetArmy = nullptr;

	int originalCount;
	int cloneCount;

	int armyWidth;
	int armyHeight;

	float currentRotation = 1.5708f;

	float spacing;

	Stance stance = Stance::Hold;
	std::pair<Stance, glm::vec2> newStance;

	CloneArmy();

	CloneArmy(WarManager* warManagaer, glm::vec2 armyPos, std::vector<std::pair<std::pair<glm::vec2*, glm::vec2*>, float*>> cloneData, int width, int height, float spacing);

	~CloneArmy();

	void changeStance(Stance newStance, glm::vec2 newPos); // 0 = attack, 1 = attack this target, 2 = hold, 3 = move, 4 = flee

	void updateStance();

	void Decide();

	glm::vec2 RotateSoldier(glm::vec2 soldierPos, float angle);

	void GetMyPath();

	void Move();

	void Hold();

	void AttackThis();

	void Attack();

	void Flee();

	bool HasLineOfSight(CloneArmy* army);
};