
#include "Utilities.h"

//////////////////////
// LoadFileAsString
// Load file using given file name and return its contents in a string
//////////////////////
std::string LoadFileAsString(std::string filename)
{

	std::stringstream fileSoFar;
	std::ifstream file(filename);

	if (file.is_open())
	{
		while (!file.eof())
		{
			std::string thisLine;
			std::getline(file, thisLine);
			fileSoFar << thisLine << std::endl;
		}
		return fileSoFar.str();
	}
	else
	{
		std::cout << "Error loading file: " << filename << std::endl;
		return "";
	}

}