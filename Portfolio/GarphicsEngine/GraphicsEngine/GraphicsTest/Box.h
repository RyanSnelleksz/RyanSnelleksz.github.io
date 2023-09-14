#pragma once

#include "Mesh.h"
#include "glm.hpp"
#include <ext/matrix_transform.hpp>
#include <ext/matrix_clip_space.hpp>

class Box
{

public:

	Mesh* boxMesh;

	glm::vec3 position = { 0, 0, 0 };

	glm::mat4 transform = {
		1, 0, 0, 0,
		0, 1, 0, 0,
		0, 0, 1, 0,
		0, 0 ,0, 1
	};

	Box() = delete;

	Box(glm::vec3, Mesh* mesh);
	
	~Box();
};