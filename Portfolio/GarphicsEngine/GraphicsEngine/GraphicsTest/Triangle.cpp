
#include "Triangle.h"

Triangle::Triangle()
{

}

Triangle::Triangle(glm::vec2 point1, glm::vec2 point2, glm::vec2 point3)
{
	glGenBuffers(1, &triangleID);

	pointsValues[0] = point1.x;
	pointsValues[1] = point1.y;
	pointsValues[2] = point2.x;
	pointsValues[3] = point2.y;
	pointsValues[4] = point3.x;
	pointsValues[5] = point3.y;

	pointOne = point1;
	pointTwo = point2;
	pointThree = point3;
};


Triangle::~Triangle()
{
}

void Triangle::Draw(ShaderProgram theShader)
{
	for (float& value : pointsValues)
	{
		if (isY == true)
		{
			value -= 0.0001f;
			isY = false;
		}
		else
		{
			isY = true;
		}
	}
	//
	glBindBuffer(GL_ARRAY_BUFFER, triangleID);
	glBufferData(GL_ARRAY_BUFFER, sizeof(float) * 6, pointsValues, GL_STATIC_DRAW);
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	//
	glBindBuffer(GL_ARRAY_BUFFER, triangleID);

	glVertexAttribPointer(0, 2, GL_FLOAT, GL_FALSE, 0, 0);

	//theShader.UseShader();

	glDrawArrays(GL_LINE_LOOP, 0, 3);
}
