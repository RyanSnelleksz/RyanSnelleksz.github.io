
#include "WarManager.h"

WarManager::WarManager()
{
}

WarManager::~WarManager()
{
	for (CloneArmy* army : cloneArmies)
	{
		delete army;
	}
}

void WarManager::Update()
{
	for (CloneArmy* army : cloneArmies)
	{
		army->Decide();
		for (Clone* clone : army->clones)
		{
			clone->Movement();
		}
	}
}

void WarManager::AddArmy(CloneArmy* newArmy)
{
	cloneArmies.push_back(newArmy);
	newArmy->managerIndex = cloneArmies.size();
}

void WarManager::CloneMap(glm::vec3 nodes[], int mapWidth, int mapHeight, float spacing)
{
	mapCopy = MapCopy(nodes, mapWidth, mapHeight, spacing);

	pathFinder.nodes = mapCopy.clonedNodes;
}
