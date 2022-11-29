#pragma once

#include "glm.hpp"
#include "Node.h"
#include "GameBase.h"
#include "PhysicsHandler.h"

class NodeMap
{
	int height;
	int width;

	float space;

	LineRenderer* lines;

public:

	std::vector<Node*> nodes;

	NodeMap();

	NodeMap(const NodeMap& other) = delete;

	~NodeMap(); // do

	void Initialize(char map[], int w, int h, float spacing, LineRenderer& line);

	void DrawMap();

	void BuildWalls(PhysicsHandler* handler);

	void ConnectNodes();

};