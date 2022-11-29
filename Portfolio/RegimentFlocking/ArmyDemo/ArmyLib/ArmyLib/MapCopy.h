#pragma once

#include "ClonedNode.h"
#include "Obstacle.h"
#include <vector>

enum Direction
{
	Left = 0,
	Up = 1,
	Right = 2,
	Down = 3
};

// Name: MapCopy
// Description: Used to hold copy of map data
// Interations: Used by pathfinding and clone armies to do line of sight checks

class MapCopy
{

public:

	std::vector<ClonedNode*> clonedNodes;
	std::vector<Obstacle> obstacles;

	ClonedNode* emptyNode = new ClonedNode(0, 0, 0);

	int width;
	int height;

	float space;

	MapCopy();

	MapCopy(glm::vec3 nodes[], int mapWidth, int mapHeight, float spacing);

	~MapCopy();

	void ConnectNodes();

	ClonedNode* GetNearestNode(glm::vec2 point);

	void FindObstacles();
};