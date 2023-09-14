#pragma once

#include "glm.hpp"
#include "Graphics.h"
#include "ShaderProgram.h"

class Triangle
{
	GLuint triangleID;

	float pointsValues[6];

	bool isY = false;

public:

	glm::vec2 pointOne;
	glm::vec2 pointTwo;
	glm::vec2 pointThree;

	Triangle();

	Triangle(glm::vec2 point1, glm::vec2 point2, glm::vec2 point3);

	~Triangle();

	void Draw(ShaderProgram theShader);

};