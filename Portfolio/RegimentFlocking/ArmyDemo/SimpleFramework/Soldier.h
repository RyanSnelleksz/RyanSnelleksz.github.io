#pragma once

#include "Circle.h"
#include "glm.hpp"

class Regiment;

class Soldier
{

public:

	int hp;

	int damage;

	float maxSpeed;

	Circle body;

	Regiment* myRegiment;

	Soldier();

	Soldier(float x, float y, float radius, Regiment& newRegiment, int health, int dam, float mSpeed);

	Soldier(const Soldier& other);

	Soldier(Soldier&& other);

	Soldier& operator=(Soldier&& other);

	Soldier& operator=(const Soldier& other);

	~Soldier();
};