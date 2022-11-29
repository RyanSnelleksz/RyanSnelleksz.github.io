#pragma once

#include "CloneArmy.h"
#include "Clone.h"
#include "MapCopy.h"
#include "Pathfinding.h"

class WarManager
{

	// Name: WarManager
	// Description: Manages the other classes and hold data
	// Interations: Has the updates of all the others

public:

	std::vector<CloneArmy*> cloneArmies;

	MapCopy mapCopy;

	Pathfinder pathFinder;

	WarManager();

	~WarManager();

	void Update();

	void AddArmy(CloneArmy* newArmy);

	void CloneMap(glm::vec3 nodes[], int mapWidth, int mapHeight, float spacing);

};