#pragma once

#include "GameBase.h"
#include "PhysicsHandler.h"
#include "CollisionFunctions.h"
#include "RigidBody.h"
#include "Maths.h"
#include <vector>
#include "Regiment.h"
#include "NodeMap.h"
#include "WarManager.h"


class Example : public GameBase
{
	glm::vec2 lastCursorPos = { 0,0 };

	glm::vec2 cursorVec = { 0,0 };

	glm::vec2 w = { 0,0 };
	glm::vec2 a = { 0,0 };
	glm::vec2 s = { 0,0 };
	glm::vec2 d = { 0,0 };
	glm::vec2 x = { 0,0 };

	PhysicsHandler physicsHandler;

	NodeMap nodeMap;

	std::vector<Regiment*> regiments;

	WarManager warManager;

	std::vector<ClonedNode*> cNodes;

	int count = 1; // exists fun

public:

	Example();

	void MakeArmies();

	void Update();

	void Render();

	void OnMouseClick(int mouseButton);

	void OnMouseRelease(int mouseButton);

	void DrawSquare(glm::vec2 pos)
	{
		lines.DrawLineSegment({ pos.x - 5, pos.y - 5 }, { pos.x + 5, pos.y - 5 }, { 0, 0, 1 }); // top left to top right
		lines.DrawLineSegment({ pos.x - 5, pos.y - 5 }, { pos.x - 5, pos.y + 5 }, { 0, 0, 1 }); // top left to bottom left
		lines.DrawLineSegment({ pos.x - 5, pos.y + 5 }, { pos.x + 5, pos.y + 5 }, { 0, 0, 1 }); // bottom left to bottom right
		lines.DrawLineSegment({ pos.x + 5, pos.y + 5 }, { pos.x + 5, pos.y - 5 }, { 0, 0, 1 }); // bottom right to top right
	}
};