
#include "Example.h"

Example::Example() : GameBase()
{
	int mapWidth = 40;
	int mapHeight = 40;
	int mapSize = 1600;
	             // ^
	char charMap[1600] = // Tiles will be flipped horizontally
	{
		'O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','O','O','O','O','O','O','O','O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','X','O',
		'O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O','O'
	};

	nodeMap.Initialize(charMap, mapWidth, mapHeight, 5, lines);
	glm::vec3 nodePoints[1600] = {};

	for (int i = 0; i < mapSize; i++)
	{
		nodePoints[i] = glm::vec3(nodeMap.nodes[i]->position.x, nodeMap.nodes[i]->position.y, nodeMap.nodes[i]->position.z);
	}
	warManager.CloneMap(nodePoints, mapWidth, mapHeight, 5);
	warManager.mapCopy.FindObstacles();

	nodeMap.BuildWalls(&physicsHandler);

	MakeArmies();
}

void Example::Update()
{
	//This call ensures that your mouse position and aspect ratio are maintained as correct.
	GameBase::Update();

	warManager.Update();

	bool SE = true; // For switching between soldier and enemy
	if (regiments.size() > 0)
	{
		for (Regiment* regimentA : regiments)
		{
			for (Regiment* regimentB : regiments)
			{
				if (glm::distance(regimentA->myClone->armyCurrentPosition, regimentB->myClone->armyCurrentPosition) < 50.0f && regimentA->myClone->managerIndex != regimentB->myClone->managerIndex && regimentA->myClone->teamNumber != regimentB->myClone->teamNumber && regimentA->myClone->stance != Stance::Flee && regimentB->myClone->stance != Stance::Flee)
				{
					for (Soldier* soldier : regimentA->soldiers)
					{
						Soldier* target = nullptr;
						float dist = FLT_MAX;
						for (Soldier* enemy : regimentB->soldiers)
						{
							if (glm::distance(soldier->body.position, enemy->body.position) < dist)
							{
								target = enemy;
							}
						}

						glm::vec2 desiredVelocity = target->body.position - soldier->body.position;
						float distance = glm::distance(target->body.position, soldier->body.position);

						// Check if we are getting close
						float slowingRadius = 10.0f;
						if (distance < slowingRadius)
						{
							if (desiredVelocity != glm::vec2{ 0, 0 })
							{
								desiredVelocity = glm::normalize(desiredVelocity) * 10.0f * (distance / slowingRadius);
							}

							soldier->body.velocity = desiredVelocity;
						}
						else
						{
							if (desiredVelocity != glm::vec2{ 0, 0 })
							{
								desiredVelocity = glm::normalize(desiredVelocity) * 10.0f;
							}
							soldier->body.velocity = desiredVelocity;
						}
						if (glm::distance(soldier->body.position, target->body.position) < soldier->body.radius * 2)
						{
							regimentA->myClone->cloneCount--;
							regimentA->soldiers.back()->body.dead = true;
							regimentA->soldiers.erase(regimentA->soldiers.end() - 1);

							regimentA->myClone->clones.erase(regimentA->myClone->clones.end() - 1);

							regimentB->myClone->cloneCount--;
							regimentB->soldiers.back()->body.dead = true;
							regimentB->soldiers.erase(regimentB->soldiers.end() - 1);

							regimentB->myClone->clones.erase(regimentB->myClone->clones.end() - 1);
						}
					}
				}
			}
		}
	}

	for (RigidBody* rigidbody : physicsHandler.rigidbodies)
	{
		rigidbody->FixedUpdate(physicsHandler.gravity, deltaTime);
	}

	physicsHandler.CollisionChecks();

	// Tracking mouse pos data

	cursorVec = cursorPos - lastCursorPos;

	lastCursorPos = cursorPos;
}

void Example::Render()
{
	nodeMap.DrawMap();

	for (RigidBody* body : physicsHandler.rigidbodies)
	{
		body->Draw(lines);
		lines.DrawCross(body->position, 0.2f, { 0, 1, 0 }); // green - botttom left
	}

	for (ClonedNode* node : warManager.cloneArmies[0]->myPath)
	{
		lines.DrawCross(node->position, 2.0f, { 1, 0, 0 });
	}

	for (Regiment* r : regiments)
	{
		for (Soldier* s : r->soldiers)
		{
			glm::vec3 c = r->myClone->teamNumber > 0 ? glm::vec3{ 0, 0, 1 } : glm::vec3{ 1, 0, 0 };
			lines.DrawCircle(s->body.position, s->body.radius, c);
		}
	}

	//This call puts all the lines you've set up on screen - don't delete it or things won't work.
	GameBase::Render();
}

void Example::OnMouseClick(int mouseButton)
{
	warManager.cloneArmies[0]->changeStance(Stance::Move, glm::vec2{ cursorPos.x, cursorPos.y });
}

void Example::OnMouseRelease(int mouseButton)
{

}

void Example::MakeArmies()
{
	regiments.push_back(new Regiment(150, 25, 10, 10, 1)); // Player

	regiments.push_back(new Regiment(20, 150, 7, 3, 1));
	regiments.push_back(new Regiment(20, 30, 7, 3, 1));
	regiments.push_back(new Regiment(150, 90, 7, 3, 1));
	regiments.push_back(new Regiment(20, 80, 7, 3, 1));

	for (Regiment* regiment : regiments)
	{
		for (Soldier* soldier : regiment->soldiers)
		{
			physicsHandler.AddRigidBody(&soldier->body);
		}
	}

	for (Regiment* regiment : regiments)
	{
		std::vector<std::pair<std::pair<glm::vec2*, glm::vec2*>, float*>> soldierData;

		for (Soldier* soldier : regiment->soldiers)
		{
			soldierData.push_back(std::make_pair(std::make_pair(&soldier->body.position, &soldier->body.velocity), &soldier->maxSpeed));
		}

		CloneArmy* newCloneArmy = new CloneArmy(&warManager, regiment->Position, soldierData, regiment->width, regiment->height, regiment->spacing);
		regiment->myClone = newCloneArmy;

		warManager.AddArmy(newCloneArmy);
	}

	warManager.cloneArmies[0]->teamNumber = 1;

	warManager.cloneArmies[3]->changeStance(Stance::Attack, glm::vec2(0, 0)); // Set one enemy to attack. The rest are holding be default.
}
