#pragma once

#include "MapCopy.h"
#include <algorithm>
#include <vector>

class Pathfinder
{

	// Name: Pathfinder
	// Description: Used for pathfinding. Uses Dikstras but is altered to take into account the armies width .
	// Interations: Used by clone armies to find paths

public:

	std::vector<ClonedNode*> nodes;

	std::vector<ClonedNode*> Pathfinding(ClonedNode* startNode, ClonedNode* endNode, int armyWidth); 

	bool checkConnections(ClonedNode* node);
};