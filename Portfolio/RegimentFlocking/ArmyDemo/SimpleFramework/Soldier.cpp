
#include "Soldier.h"

Soldier::Soldier()
{
}

Soldier::Soldier(float x, float y, float radius, Regiment& newRegiment, int health, int dam, float mSpeed)
{
	body = Circle(x, y, radius);
	myRegiment = &newRegiment;

	hp = health;
	damage = dam;
	maxSpeed = mSpeed;
}

Soldier::Soldier(const Soldier& other)
{
	body = other.body;
	myRegiment = other.myRegiment;

	hp = other.hp;
	damage = other.damage;
	maxSpeed = other.maxSpeed;
}

Soldier::Soldier(Soldier&& other)
{
	body = other.body;
	myRegiment = other.myRegiment;

	body = other.body;
	myRegiment = other.myRegiment;

	hp = other.hp;
	damage = other.damage;
	maxSpeed = other.maxSpeed;
}

Soldier& Soldier::operator=(Soldier&& other)
{
	body = other.body;
	myRegiment = other.myRegiment;

	body = other.body;
	myRegiment = other.myRegiment;

	hp = other.hp;
	damage = other.damage;
	maxSpeed = other.maxSpeed;

	return *this;
}

Soldier& Soldier::operator=(const Soldier& other)
{
	body = other.body;
	myRegiment = other.myRegiment;

	body = other.body;
	myRegiment = other.myRegiment;

	hp = other.hp;
	damage = other.damage;
	maxSpeed = other.maxSpeed;

	return *this;
}


Soldier::~Soldier()
{

}