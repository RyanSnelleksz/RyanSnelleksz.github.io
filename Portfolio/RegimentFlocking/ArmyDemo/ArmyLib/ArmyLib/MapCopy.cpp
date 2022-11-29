
#include "MapCopy.h"

MapCopy::MapCopy()
{

}

MapCopy::MapCopy(glm::vec3 nodes[], int mapWidth, int mapHeight, float spacing)
{
	width = mapWidth;
	height = mapHeight;

	space = spacing;

	int size = width * height;

	for (int i = 0; i < size; i++)
	{
		clonedNodes.push_back(new ClonedNode(nodes[i].x, nodes[i].y, nodes[i].z));
	}

	ConnectNodes();
}

MapCopy::~MapCopy()
{

}

// Name: ConnectNodes
// Parameter: 
// Description: Creates connections between adjacent nodes
// Return Value:
void MapCopy::ConnectNodes()
{
	for (int i = 0; i < clonedNodes.size(); i++)
	{
		ClonedNode* node = clonedNodes[i];

		if (i % width == 0) // Checking if we are at the start of a row
		{
			node->connections[Left] = emptyNode;
		}
		else
		{
			node->connections[Left] = clonedNodes[i - 1];
		}

		if ((i + 1) % width == 0) // Checking if we are at the end of a row
		{
			node->connections[Right] = emptyNode;
		}
		else
		{
			node->connections[Right] = clonedNodes[i + 1];
		}

		if (i >= 0 && i <= width - 1) // Checking if we are at the start of a column
		{
			node->connections[Up] = emptyNode;
		}
		else
		{
			node->connections[Up] = clonedNodes[i - width];
		}

		if (i >= width * (height - 1) && i <= width * height - 1) // Checking if we are at the end of a column
		{
			node->connections[Down] = emptyNode;
		}
		else
		{
			node->connections[Down] = clonedNodes[i + width];
		}
	}
}

// Name: GetNearestNode
// Parameter: Takes a vec2
// Description: Finds the nearest node to a given point
// Return Value: Returns the node
ClonedNode* MapCopy::GetNearestNode(glm::vec2 point)
{
	int columnIndex = (int)(point.x / space);
	int rowIndex = (int)(point.y / space);

	columnIndex = std::max(0, std::min(width - 1, columnIndex));
	rowIndex = std::max(0, std::min(height - 1, rowIndex));

	return clonedNodes[columnIndex + width * rowIndex];
}

// Name: FindObstacles
// Parameter: 
// Description: Checks each node and if its impassable create an obstacle and add it too the list
// Return Value:
void MapCopy::FindObstacles()
{
	for (ClonedNode* node : clonedNodes)
	{
		if (node->position.z == 0)
		{
			obstacles.push_back(Obstacle(node->position, space, space));
		}
	}
}
