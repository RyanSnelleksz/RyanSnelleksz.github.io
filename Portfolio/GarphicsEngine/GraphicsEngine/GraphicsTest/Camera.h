#pragma once

#include <ext/matrix_transform.hpp>
#include <glfw3.h>
#include <ext/matrix_clip_space.hpp>

class Camera
{
	float sWidth;
	float sHeight;
	float nClip;
	float fClip;

public:

	glm::vec3 targ;
	glm::vec3 pos;

	glm::mat4 view;
	glm::mat4 projection;

	Camera() = delete;

	Camera(float screenWidth, float screenHeight, float nearClip, float farClip, glm::vec3 target, glm::vec3 position);

	void Move(float x, float y);
};