
#include "ShaderProgram.h"


ShaderProgram::ShaderProgram(std::string vertexFilename, std::string fragmentFilename)
{
	everythingIsOkay = true; // Check if everything is good so far

	vertexShader = glCreateShader(GL_VERTEX_SHADER); // create 'blank' vertex shader
	fragmentShader = glCreateShader(GL_FRAGMENT_SHADER); // create 'blank' fragment shader
	
	shaderProgram = glCreateProgram(); // create 'blank' shader program
	
	std::string vertexSource = LoadFileAsString(vertexFilename); // We are getting the file path of our vertex shader file
	std::string fragmentSource = LoadFileAsString(fragmentFilename); // We are getting the file path of our fragment shader file
	
	const char* vertexSourceC = vertexSource.c_str(); // Setting the string to a char array

	glShaderSource(vertexShader, 1, &vertexSourceC, nullptr); // sets the source of the vertex shader to the vertex shader file
	glCompileShader(vertexShader); // compiles the code in the vertex shader file

	GLchar errorLog[512];
	GLint success = 0;
	glGetShaderiv(vertexShader, GL_COMPILE_STATUS, &success); // checks if the vertex shader is succesfully made
	if (success == GL_FALSE)
	{
		//Something failed with the vertex shader compilation
		std::cout << "Vertex Shader " << vertexFilename << " failed because..." << std::endl; // if its not ok then output why
		glGetShaderInfoLog(vertexShader, 512, nullptr, errorLog);
		std::cout << errorLog << std::endl;
		everythingIsOkay = false; // we are not ok
	}
	else
	{
		std::cout << "Vertex Shader Works" << std::endl;
	}

	const char* fragmentSourceC = fragmentSource.c_str(); // ^

	glShaderSource(fragmentShader, 1, &fragmentSourceC, nullptr); // set the source of the fragment shader to the fragment shader file
	glCompileShader(fragmentShader); // compile the code in it

	glGetShaderiv(fragmentShader, GL_COMPILE_STATUS, &success); // same as earlier
	if (success == GL_FALSE)
	{
		//Something failed with the fragment shader compilation
		std::cout << "Fragment Shader " << fragmentFilename << " failed because..." << std::endl;
		glGetShaderInfoLog(fragmentShader, 512, nullptr, errorLog);
		std::cout << errorLog << std::endl;
		everythingIsOkay = false;
	}
	else
	{
		std::cout << "Fragment Shader Works" << std::endl;
	}


	glAttachShader(shaderProgram, fragmentShader); // Give the vertex and fragment shader to the shader program
	glAttachShader(shaderProgram, vertexShader); // ^
	glLinkProgram(shaderProgram); // Link the vertex and fragment shader to create the shader program
	glGetProgramiv(shaderProgram, GL_LINK_STATUS, &success); // check if it worked
	if (success == GL_FALSE)
	{
		std::cout << "The linking is busted or something else has gone wrong" << std::endl; // if it didnt work, output why
		glGetProgramInfoLog(shaderProgram, 512, nullptr, errorLog);
		std::cout << errorLog << std::endl;
		everythingIsOkay = false;
	}

	if (everythingIsOkay)
	{
		std::cout << "Everything is good so far" << std::endl;
	}

}

void ShaderProgram::UseShader()
{
	glUseProgram(shaderProgram);
}

GLuint ShaderProgram::GetUniformLocation(std::string varName)
{
	return glGetUniformLocation(shaderProgram, varName.c_str());
}

// Sets integer uniforms
void ShaderProgram::SetUniform(std::string varName, int value)
{
	GLuint varLoc = glGetUniformLocation(shaderProgram, varName.c_str());
	UseShader();
	glUniform1i(varLoc, value);
}

// Sets float uniforms
void ShaderProgram::SetUniform(std::string varName, float value)
{
	GLuint varLoc = glGetUniformLocation(shaderProgram, varName.c_str());
	UseShader();
	glUniform1f(varLoc, value);
}

// Sets mat4 uniforms
void ShaderProgram::SetUniform(std::string varName, glm::mat4 value)
{
	GLuint varLoc = glGetUniformLocation(shaderProgram, varName.c_str());
	UseShader();
	glUniformMatrix4fv(varLoc, 1, GL_FALSE, &value[0][0]);
}

// Sets mat3 uniforms
void ShaderProgram::SetUniform(std::string varName, glm::mat3 value)
{
	GLuint varLoc = glGetUniformLocation(shaderProgram, varName.c_str());
	UseShader();
	glUniformMatrix3fv(varLoc, 1, GL_FALSE, &value[0][0]);
}

// Sets vec3 uniforms
void ShaderProgram::SetUniform(std::string varName, glm::vec3 value)
{
	GLuint varLoc = glGetUniformLocation(shaderProgram, varName.c_str());
	UseShader();
	glUniform3fv(varLoc, 3, &value[0]);
}