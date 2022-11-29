
#include "Regiment.h"

Regiment::Regiment()
{
}

Regiment::Regiment(float x, float y, int theWidth, int theHeight, float space)
{
	Position = { x, y };

	width = theWidth;
	height = theHeight;

	float lastX = x;
	float lastY = y;

	float soldierRadius = 1;

	int count = 0;

	spacing = space + soldierRadius * 2;

	for (int i = 0; i < height; i++)
	{
		for (int j = 0; j < width; j++)
		{
			soldiers.push_back(new Soldier(lastX, lastY, soldierRadius, *this, 3, 1, 10));

			count += 1;

			lastX += 0.5 + soldierRadius * 2;
		}
		lastX = x;
		lastY += spacing;
	}
	Position = { x + ((width / 2) - 1) * (space * 2 + soldierRadius), y + ((height / 2) + 1) * (space + soldierRadius) };
}

Regiment::~Regiment()
{

}