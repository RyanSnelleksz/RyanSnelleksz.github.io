
#include "Pathfinding.h"

bool NodeSort(ClonedNode* i, ClonedNode* j)
{
	return (i->gScore < j->gScore);
}

// Name: Pathfinding.
// Parameter: Takes two ClonedNode pointers as a start and a end as well as armyWidth to see how much space we need.
// Description: Finds a the shortest suitable path between the two nodes given.
// Return Value: Returns the path found or an empty vector if no path was found.
std::vector<ClonedNode*> Pathfinder::Pathfinding(ClonedNode* startNode, ClonedNode* endNode, int armyWidth)
{
	for (ClonedNode* node : nodes)
	{
		node->gScore = 0.0f;
	}

	// Make Lists
	std::vector<ClonedNode*> openList;
	std::vector<ClonedNode*> closedList;

	if (startNode == endNode) // If there is only one node
	{
		std::vector<ClonedNode*> singleNodePath;
		singleNodePath.push_back(startNode);
		return singleNodePath;
	}

	// Our return value
	std::vector<ClonedNode*> path;

	// set current node and add it to openList
	startNode->gScore = 0;
	startNode->previous = nullptr;

	ClonedNode* currentNode = startNode;
	openList.push_back(currentNode);

	// We go until the list is empty
	while (!openList.empty())
	{
		// sort so highest g score goes first
		std::sort(openList.begin(), openList.end(), NodeSort);

		// we go from top which has the smallest g score
		currentNode = *openList.begin();
		openList.erase(openList.begin());

		closedList.push_back(currentNode);

		// if currentNode = endNode 
		if (currentNode == endNode)
		{
			break;
		}
		for (ClonedNode* node : currentNode->connections) // for each connection
		{
			bool badNode = false;
			if (node == nullptr || node->position.z == 0) // if the first one is impassable or a nullptr, mark it
			{
				badNode = true;
			}
			badNode = checkConnections(node); // check its connections too

			std::vector<ClonedNode*> checkNodes;
			checkNodes.push_back(node);

			int checkIndex = 0;
			bool allChecked = false;

			while (checkIndex < armyWidth * 2) // Checks to see if the node is too close to a wall
			{
				for (int i = 0; i < 4; i++)
				{
					checkNodes.push_back(checkNodes[checkIndex]->connections[i]);
				}
				checkIndex += 1;
			}
			for (ClonedNode* checkNode : checkNodes)
			{
				if (checkNode == nullptr || checkNode->position.z == 0 || checkConnections(checkNode))
				{
					badNode = true;
				}
			}


			if (badNode == true || std::find(closedList.begin(), closedList.end(), node) != closedList.end()) 
			{
				continue;
			}
			//If the target node is not in the openList, update it
			if (std::find(openList.begin(), openList.end(), node) == openList.end()) {
				//Calculate the target node's g Score and set the nodes previous
				node->gScore = currentNode->gScore + node->weight;
				node->previous = currentNode;

				// Find the earliest point too insert the node
				bool inserted = false;
				for (int i = 0; i < openList.size(); i++)
				{
					if (openList[i]->gScore > node->gScore)
					{
						openList.insert(openList.begin() + i, node);
						inserted = true;
					}
				}
				if (inserted == false)
				{
					openList.push_back(node);
				}
			}
			else
			{
				//Check the g scores before updating
				if (currentNode->gScore + node->weight < node->gScore) 
				{
					node->gScore = currentNode->gScore + node->weight;
					node->previous = currentNode;
				}
			}
		}
	}

	path.clear();

	ClonedNode* pathNode = endNode;
	float lastPathG;

	bool copied = false;
	while (copied == false)
	{
		if (pathNode->previous != nullptr) // Create path in using the nodes previous'
		{
			path.insert(path.begin(), pathNode);
			pathNode = pathNode->previous;
		}
		else
		{
			copied = true;
			lastPathG = endNode->gScore;
			endNode->gScore = 0;
		}
	}

	return path;
}

// Name: Check Connections
// Parameter: Takes a ClonedNode pointer
// Description: Takes a node a checks if it's connections are considered impassable or are nullptrs
// Return Value: Returns true if any are bad

bool Pathfinder::checkConnections(ClonedNode* node)
{
	bool isBad = false;
	for (ClonedNode* nextNode : node->connections)
	{
		if (nextNode == nullptr || nextNode->position.z == 0)
		{
			isBad = true;
		}
		else
		{

		}
	}
	return isBad;
}
