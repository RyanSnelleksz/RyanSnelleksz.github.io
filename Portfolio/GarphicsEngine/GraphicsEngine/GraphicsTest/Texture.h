#pragma once

#include "Graphics.h"
#include <string>

class Texture
{
private:
	GLuint textureID;

	int width;
	int height;
	int channelCount;

	bool successfullyLoaded = false;

public:
	Texture() = delete;

	Texture(std::string filename);

	~Texture();

	Texture(const Texture& otherTexture) = delete;

	Texture& operator=(const Texture& otherTexture) = delete;

	Texture(Texture&& otherTexture);

	Texture& operator=(Texture&& otherTexture);

	void Bind(int textureUnit);

	static void Unbind(int textureUnit);

};