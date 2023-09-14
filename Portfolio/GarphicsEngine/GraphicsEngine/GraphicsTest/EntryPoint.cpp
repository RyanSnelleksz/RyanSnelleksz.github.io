#pragma once

#include "Graphics.h"

#include <math.h>
#include <vector>
#include "Utilities.h"
#include "PhysicsHandler.h"

#include <iostream>

#include "glm.hpp"

#include "ext/matrix_transform.hpp"
#include "ext/matrix_clip_space.hpp"

#include "Camera.h"
#include "Box.h"
#include "Renderer.h"
#include "Mesh.h"
#include "Texture.h"

#include <iostream>
#include <windows.h>

#include <chrono>
#include <ctime>    

int main()
{
	const float fixedDeltaTime = 0.0166667f;

	GLFWwindow* window;	//The pointer to the GLFW window that gives us a place to draw.

	if (!glfwInit())
		return -1;	//glfw failed to initialise.

	int screenWidth = 1280;
	int screenHeight = 720;

	// Setting screen resolution here.
	window = glfwCreateWindow(screenWidth, screenHeight, "GPU Graphics", nullptr, nullptr);

	float aspectRatio = (float)screenWidth / (float)screenHeight;

	if (!window)
	{
		//If the window failed to create for some reason, abandon ship.
		glfwTerminate();
		return -1;
	}

	// Tell glfw to use the OpenGL context from the window.
	glfwMakeContextCurrent(window);

	// Set yp GLAD so we can use openGL functions.
	if (!gladLoadGL())
		return -1;

	glEnable(GL_DEPTH_TEST); // Tells program to start recording depth of pixels and to check whether you are drawing over less depth pixls
	// allows render order independence

	ShaderProgram meshShader("MeshShader.vsd", "MeshShader.fsd"); // Shader for loading 3D meshes
	ShaderProgram shapeShader("ShapeShader.vsd", "ShapeShader.fsd"); // Shader for loading simple 2D shapes

	Camera mainCamera(1920.0f, 1080.0f, 1.0f, 100.0f, glm::vec3(-5.0f, 0.0f, 5.0f), glm::vec3(0.0f, 0.0f, 00.0f));
	//                                                       ^ Where cam points to  ^ where it looks from

	Renderer renderer(&meshShader, &mainCamera);
	renderer.Initialise();

	// Need the camera orthographic for 2D stuff
	
	glm::mat4 cameraOrtho = glm::ortho(-aspectRatio * 10.0f / 2.0f + mainCamera.pos.x, aspectRatio * 10.0f / 2.0f + mainCamera.pos.x, -10.0f / 2.0f + mainCamera.pos.y, 10.0f / 2.0f + mainCamera.pos.y, -1.0f, 1.0f);

	shapeShader.SetUniform("vpMatrix", cameraOrtho);

	// 

	glEnableVertexAttribArray(0); // Enabling our attributes locations
	glEnableVertexAttribArray(1);
	glEnableVertexAttribArray(2);
	glEnableVertexAttribArray(3);
	glEnableVertexAttribArray(4);

	Texture diffuseTexture("Meshes/monster/monster_monster_BaseMap.tga");
	Texture normalTexture("Meshes/monster/monster_monster_Normal.tga");
	Texture specularTexture("No Specular Given(On Purpose)");

	Mesh mesh(std::string("Meshes/monster/monster_mesh_01.obj"), glm::vec3(1.0f, 0.0f, 0.0f), meshShader);

	Mesh meshOther(std::string("Meshes/monster/monster_mesh_01.obj"), glm::vec3(1.0f, 0.0f, 0.0f), meshShader);

	glm::mat4 transform =
	{ 
	1.0f, 0.0f, 0.0f, 0.0f,
	0.0f, 1.0f, 0.0f, 0.0f,
	0.0f, 0.0f, 1.0f, 0.0f,
	0.0f, 0.0f, 0.0f, 1.0f
	};

	glm::mat4 transformTwo = 
	{ 
	1.0f, 0.0f, 0.0f, 0.0f,
	0.0f, 1.0f, 0.0f, 0.0f,
	0.0f, 0.0f, 1.0f, 0.0f,
	0.0f, 0.0f, 0.0f, 1.0f
	};

	transform = glm::translate(transform, glm::vec3(5.0f, 0.0f, 0.0f));

	std::vector<glm::vec2> customShape = {
		glm::vec2{-1.0f, 1.0f}, glm::vec2{1.0f, 1.0f}, // bottom
		glm::vec2{-1.0f, 1.0f}, glm::vec2{-1.5f, 0.6f}, // left
		glm::vec2{-1.5f, 0.6f}, glm::vec2{0.0f, -1.0f}, // top left
		glm::vec2{0.0f, -1.0f}, glm::vec2{1.5f, 0.6f}, // top right
		glm::vec2{1.5f, 0.6f}, glm::vec2{1.0f, 1.0f} // right
	};

	std::vector<glm::vec3> customShapeColours = {
	glm::vec3{1.0f, 0.0f, 0.0f}, glm::vec3{1.0f, 0.0f, 1.0f}, // bottom
	glm::vec3{0.0f, 1.0f, 0.0f}, glm::vec3{1.0f, 1.0f, 1.0f}, // left
	glm::vec3{0.0f, 0.0f, 1.0f}, glm::vec3{1.0f, 0.0f, 0.0f}, // top left
	glm::vec3{1.0f, 1.0f, 0.0f}, glm::vec3{0.0f, 1.0f, 0.0f}, // top right
	glm::vec3{0.0f, 1.0f, 1.0f}, glm::vec3{0.0f, 0.0f, 1.0f}  // right
	};

	/////////////////////////// Just Some Physics Stuff //////////////////////////////////////////////////////////////////////////////////////

	PhysicsHandler physicsHandler; 

	// I want some planes
	physicsHandler.AddRigidBody(new Plane(-8.0f, 3.0f, -8.0f, -3.0f)); // left plane
	physicsHandler.AddRigidBody(new Plane(-2.0f, -3.0f, -2.0f, 3.0f)); // right plane
	physicsHandler.AddRigidBody(new Plane(-2.0f, 3.0f, -8.0f, 3.0f)); // top plane
	physicsHandler.AddRigidBody(new Plane(-8.0f, -3.0f, -2.0f, -3.0f)); // bottom plane

	physicsHandler.AddRigidBody(new Circle(-4.0f, 0.1f, 0.5f));
	physicsHandler.AddRigidBody(new Circle(-4.1f, 0.5f, 0.5f));


	//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	/////////////////////////// DRAW STATIC SHAPES HERE //////////////////////////////////////////////////////////////////////////////////////

	// Make sure when you request the draw you set static to true

	renderer.RequestCustomShape(customShape, true);

	renderer.CompileStaticShapes();
	// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
	while (!glfwWindowShouldClose(window))
	{
		glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT); // Have to clear the colour buffer and the depth buffer

		// Controls

		transform = glm::rotate(transform, 0.015f, glm::vec3(0.0f, 1.0f, 0.0f));

		if (glfwGetKey(window, GLFW_KEY_W) == GLFW_PRESS || glfwGetKey(window, GLFW_KEY_UP) == GLFW_PRESS)
		{
			mainCamera.Move(0.0f, -0.5f);
		}
		if (glfwGetKey(window, GLFW_KEY_A) == GLFW_PRESS || glfwGetKey(window, GLFW_KEY_LEFT) == GLFW_PRESS)
		{
			mainCamera.Move(0.5f, 0.0f);
		}
		if (glfwGetKey(window, GLFW_KEY_S) == GLFW_PRESS || glfwGetKey(window, GLFW_KEY_DOWN) == GLFW_PRESS)
		{
			mainCamera.Move(0.0f, 0.5f);
		}
		if (glfwGetKey(window, GLFW_KEY_D) == GLFW_PRESS || glfwGetKey(window, GLFW_KEY_RIGHT) == GLFW_PRESS)
		{
			mainCamera.Move(-0.5f, 0.0f);
		}

		////////////////// Physics Updates //////////////////////////////////////////////////////////////////

		for (RigidBody* rigidbody : physicsHandler.rigidbodies)
		{
			rigidbody->FixedUpdate(physicsHandler.gravity, fixedDeltaTime);
		}

		physicsHandler.CollisionChecks();

		//////////////////////////////////////////////////////////////////////////////////////////////////////

		/// ////////////////////
		meshShader.UseShader(); // MESH SHADING BELOW ///////////////////////////////////////////////////////////////////////////////////////

		renderer.DrawIndexedTexturedMesh(mesh, transform, diffuseTexture, normalTexture, specularTexture);

		shapeShader.UseShader(); // DYNAMIC SHAPE SHADING BELOW ///////////////////////////////////////////////////////////////////////////////////////

		for (RigidBody* body : physicsHandler.rigidbodies)
		{
			body->Draw(renderer);
		}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////

		renderer.DrawStaticShapes();

		renderer.CompileDynamicShapes();
		renderer.DrawDynamicShapes();

		glfwSwapBuffers(window);

		glfwPollEvents();
	}
	glfwTerminate();
	return 0;
}