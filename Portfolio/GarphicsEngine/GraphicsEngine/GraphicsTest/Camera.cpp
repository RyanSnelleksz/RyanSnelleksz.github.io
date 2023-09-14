
#include "Camera.h"

Camera::Camera(float screenWidth, float screenHeight, float nearClip, float farClip, glm::vec3 target, glm::vec3 position)
{
	pos = position;
	targ = target;

	sWidth = screenWidth;
	sHeight = screenHeight;
	nClip = nearClip;
	fClip = farClip;

	view = glm::lookAt(target, position, glm::vec3(0, 1, 0)); // Sets the camera - Gets vertices in camera relative position

	projection = glm::perspective(3.14159f / 4, screenWidth / screenHeight, nearClip, farClip); // Sets Camera - Fixes aspect ratio and makes distant objects smaller
	//                          ^field of view^ aspect ratio ^ near and far clip planes - if nearer than 0.1 then disappear and if further than 100, disappear 

}

void Camera::Move(float x, float y)
{
	targ.x += x;
	targ.y += y;
	targ.z += x;

	pos.x += x;
	pos.y += y;
	pos.z += x;

	view = glm::lookAt(targ, pos, glm::vec3(0, 1, 0)); // Sets the camera - Gets vertices in camera relative position

	projection = glm::perspective(3.14159f / 4, sWidth / sHeight, nClip, fClip); // Sets Camera - Fixes aspect ratio and makes distant objects smaller
//                          ^field of view^ aspect ratio ^ near and far clip planes - if nearer than 0.1 then disappear and if further than 100, disappear 

}
