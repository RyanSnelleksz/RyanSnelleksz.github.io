#pragma once

#include "Graphics.h"
#include "glm.hpp"
#include <string>

#include "Utilities.h"
#include <iostream>

class ShaderProgram
{
private:

	GLuint vertexShader;
	GLuint fragmentShader;
	GLuint shaderProgram;

	bool everythingIsOkay = false;

public:
	ShaderProgram() = delete;
	ShaderProgram(std::string vertexFilename, std::string fragmentFilename);

	~ShaderProgram() {}

	bool IsEverythingOkay() const { return everythingIsOkay; }

	void UseShader();

	GLuint GetUniformLocation(std::string varName);

	void SetUniform(std::string varName, int value);
	void SetUniform(std::string varName, float value);
	void SetUniform(std::string varName, glm::mat4 value);
	void SetUniform(std::string varName, glm::mat3 value);
	void SetUniform(std::string varName, glm::vec3 value);
};
