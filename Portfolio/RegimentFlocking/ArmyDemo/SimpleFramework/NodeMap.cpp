
#include "NodeMap.h"

NodeMap::NodeMap()
{
}

NodeMap::~NodeMap()
{
}

void NodeMap::Initialize(char map[], int w, int h, float spacing, LineRenderer& line)
{
	lines = &line;

	width = w;
	height = h;

	space = spacing;

	int size = w * h - 1;

	char empty = 'O';
	char wall = 'X';

	int count = 0;

	float lastX = 0;
	float lastY = 0;

	for (int i = 0; i < height; i++)
	{
		for (int j = 0; j < width; j++)
		{
			if (map[size - count] == 'O')
			{
				nodes.push_back(new Node(lastX, lastY, 0));
			}
			else
			{
				nodes.push_back(new Node(lastX, lastY, 1));
			}
			lastX += spacing;
			count += 1;
		}
		lastX = 0;
		lastY += spacing;
	}
}

void NodeMap::DrawMap()
{
	for (Node* node : nodes)
	{
		lines->DrawCross({ node->position.x, node->position.y }, 0.2f, { 1, 1, 0 });

		if (node->position.z == 0)
		{
			lines->AddPointToLine({ node->position.x, node->position.y }); // box
			lines->AddPointToLine({ node->position.x + space, node->position.y });
			lines->AddPointToLine({ node->position.x + space, node->position.y + space });
			lines->AddPointToLine({ node->position.x, node->position.y + space });

			lines->FinishLineLoop();
		}
	}
}

void NodeMap::BuildWalls(PhysicsHandler* handler)
{
	for (Node* node : nodes)
	{
		lines->DrawCross({ node->position.x, node->position.y }, 0.2f, { 1, 1, 0 });

		if (node->position.z == 0)
		{
			Box* newBox = new Box(node->position.x, node->position.x + space, node->position.y, node->position.y + space);
			handler->AddRigidBody(newBox);
		}
	}
}

// Connections if 4 node pointers that each node has. The first node is left, the next is up, the next is right and the last is down.

enum Direction
{
	Left = 0,
	Up = 1,
	Right = 2,
	Down = 3
};


// Not used for this node map but good for future uses of a nodemap
void NodeMap::ConnectNodes()
{
	for (int i = 0; i < nodes.size(); i++)
	{
		Node* node = nodes[i];

		if (i % width == 0) // Checking if we are at the start of a row
		{
			node->connections[Left] = nullptr;
		}
		else
		{
			node->connections[Left] = nodes[i - 1];

			lines->DrawLineSegment(glm::vec2(node->position.x, node->position.y), glm::vec2(node->connections[Left]->position.x, node->connections[Left]->position.y));
		}

		if ((i + 1) % width == 0) // Checking if we are at the end of a row
		{
			node->connections[Right] = nullptr;
		}
		else
		{
			node->connections[Right] = nodes[i + 1];

			lines->DrawLineSegment(glm::vec2(node->position.x, node->position.y), glm::vec2(node->connections[Right]->position.x, node->connections[Right]->position.y));
		}

		if (i >= 0 && i <= width - 1) // Checking if we are at the start of a column
		{
			node->connections[Up] = nullptr;
		}
		else
		{
			node->connections[Up] = nodes[i - width];

			lines->DrawLineSegment(glm::vec2(node->position.x, node->position.y), glm::vec2(node->connections[Up]->position.x, node->connections[Up]->position.y));
		}

		if (i >= width * (height - 1) && i <= width * height - 1) // Checking if we are at the end of a column
		{
			node->connections[Down] = nullptr;
		}
		else
		{
			node->connections[Down] = nodes[i + width];

			lines->DrawLineSegment(glm::vec2(node->position.x, node->position.y), glm::vec2(node->connections[Down]->position.x, node->connections[Down]->position.y));
		}
	}
}
