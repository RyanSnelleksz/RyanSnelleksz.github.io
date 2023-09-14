
#include "Texture.h"
#include <iostream>

#define STB_IMAGE_IMPLEMENTATION
#include "stb_image.h"

Texture::Texture(std::string filename)
{
    unsigned char* imageData = stbi_load(filename.c_str(), &width, &height, &channelCount, 0); // Loads image data

    if (imageData == nullptr) // check if we failed to load
    {
        std::cout << "Failed to load " << filename << " texture" << std::endl;
        return;
    }

    glGenTextures(1, &textureID); // Make space for texture

    glBindTexture(GL_TEXTURE_2D, textureID); // bind it into use

    if (channelCount == 3) // check channel count
    {
        glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB, width, height, 0, GL_RGB, GL_UNSIGNED_BYTE, imageData);
    }
    else if (channelCount == 4)
    {
        glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, width, height, 0, GL_RGBA, GL_UNSIGNED_BYTE, imageData);
    }

    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
    glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR_MIPMAP_LINEAR);

    glGenerateMipmap(GL_TEXTURE_2D);

    glBindTexture(GL_TEXTURE_2D, 0); // Unbind

    stbi_image_free(imageData);
}

Texture::~Texture()
{
    if (successfullyLoaded)
    {
        glDeleteTextures(1, &textureID);
    }
}

Texture::Texture(Texture&& otherTexture)
{
    if (&otherTexture == this)
    {
        return;
    }
    if (successfullyLoaded)
    {
        glDeleteTextures(1, &textureID);
    }
    this->textureID = otherTexture.textureID;
    this->height = otherTexture.height;
    this->width = otherTexture.width;
    this->channelCount = otherTexture.channelCount;

    otherTexture.successfullyLoaded = false;
}

Texture& Texture::operator=(Texture&& otherTexture)
{
    if (&otherTexture == this)
    {
        return *this;
    }
    if (successfullyLoaded)
    {
        glDeleteTextures(1, &textureID);
    }

    this->textureID = otherTexture.textureID;
    this->height = otherTexture.height;
    this->width = otherTexture.width;
    this->channelCount = otherTexture.channelCount;

    otherTexture.successfullyLoaded = false;

    return *this;
}

void Texture::Bind(int textureUnit)
{
    glActiveTexture(GL_TEXTURE0 + textureUnit);
    glBindTexture(GL_TEXTURE_2D, textureID);
}

void Texture::Unbind(int textureUnit)
{
    glActiveTexture(GL_TEXTURE0 + textureUnit);
    glBindTexture(GL_TEXTURE_2D, 0);
}
