#pragma once

#include "glm.hpp"

struct Vertex
{
	glm::vec3 position;
	glm::vec3 colour;
	glm::vec3 normal;
	glm::vec2 texCoord;
	glm::vec4 tangent;
};