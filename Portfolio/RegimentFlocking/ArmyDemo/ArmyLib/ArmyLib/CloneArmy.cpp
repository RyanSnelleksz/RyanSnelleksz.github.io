
#include "CloneArmy.h"

CloneArmy::CloneArmy()
{

}

CloneArmy::CloneArmy(WarManager* warManager, glm::vec2 armyPos, std::vector<std::pair<std::pair<glm::vec2*, glm::vec2*>, float*>> cloneData, int width, int height, float space)
{
	myManager = warManager;

	armyDesiredPosition = armyPos;
	armyCurrentPosition = armyPos;

	cloneCount = width * height;
	originalCount = cloneCount;

	armyWidth = width;
	armyHeight = height;

	spacing = space;

	for (int i = 0; i < cloneData.size(); i++)
	{
		clones.push_back(new Clone(*cloneData[i].second, cloneData[i].first.first, cloneData[i].first.second, this));
		clones[i]->targetPosition = *clones[i]->position;
	}
}

CloneArmy::~CloneArmy()
{
	delete captain;

	for (Clone* clone : clones)
	{
		delete clone;
	}
}

// Name: changeStance
// Parameter: Takes a stance and vec2 for the buffer to be set as
// Description: Sets the stance and desired pos of the buffer
// Return Value:

void CloneArmy::changeStance(Stance nStance, glm::vec2 newPos)
{
	newStance.first = nStance;
	newStance.second = newPos;

	hasNewOrders = true;
}

// Name: UpdateStance
// Parameter: 
// Description: Updates stance to whatever is in the buffer(newStance) and resets variables for other stances to use
// Return Value:

void CloneArmy::updateStance()
{
	stance = newStance.first;
	targetArmy = nullptr;

	myPath.clear();
	routeIndex = 0;

	rotated = false;
	rotating = false;

	moved = false;
	startedMoving = false;

	for (Clone* clone : clones)
	{
		clone->inPosition = false;
	}

	if (stance == Stance::Move)
	{
		armyDesiredPosition = newStance.second;

		GetMyPath();

		if (myPath.size() != 0) // Whenever it says this it means that we are checking if we have a path, if we don't it means the area we want to go is innaccessible
		{
			armyDesiredPosition = newStance.second;
			armyDesiredPosition = myPath[0]->position;
		}
		else
		{
			armyDesiredPosition = armyCurrentPosition;
			changeStance(Stance::Hold, armyCurrentPosition);
		}
	}

	if (stance == Stance::AttackThisTarget)
	{
		float distance = FLT_MAX;
		for (CloneArmy* army : myManager->cloneArmies) // Find nearest enemy army to the given position
		{
			if (distance > glm::distance(army->armyCurrentPosition, newStance.second) && army->managerIndex != managerIndex && army->teamNumber != teamNumber)
			{
				distance = glm::distance(army->armyCurrentPosition, newStance.second);
				targetArmy = army;
			}
		}
		if (targetArmy != nullptr)
		{
			armyDesiredPosition = targetArmy->armyCurrentPosition;

			GetMyPath();

			if (myPath.size() != 0)
			{
				armyDesiredPosition = targetArmy->armyCurrentPosition;
				armyDesiredPosition = myPath[0]->position;
			}
			else
			{
				armyDesiredPosition = armyCurrentPosition;
				changeStance(Stance::Hold, armyCurrentPosition);
			}
		}
	}
}

// Name: Decide
// Parameter: 
// Description: Decides what the army will do based on it's current stance. Also sets army to flee if we've lost enough soldiers
// Return Value:

void CloneArmy::Decide()
{
	if (cloneCount < (int)(originalCount * 0.2))
	{
		stance = Stance::Flee;
	}

	if (stance == Stance::Flee)
	{
		Flee();
	}
	else if (stance == Stance::Attack)
	{
		Attack();
	}
	else if (stance == Stance::Hold)
	{
		Hold();
	}
	else if (stance == Stance::AttackThisTarget)
	{
		AttackThis();
	}
	else if (stance == Stance::Move)
	{
		Move();
	}
}

// Name: Move
// Parameter: 
// Description: Will rotate or move the target positions of the clones until it's time for the next step in pathing
// Return Value:

void CloneArmy::Move()
{
	if (rotated == false && rotating == false) // rotated will be set to true after first use to make sure its done once, and rotating indicated whether the army is done rotating or not
	{
		glm::vec2 desiredVelocity = -glm::vec2{ armyDesiredPosition.y - armyCurrentPosition.y, armyDesiredPosition.x - armyCurrentPosition.x };
		float angle = atan2(desiredVelocity.x, desiredVelocity.y); // radians

		float angleChange = -(currentRotation - angle);

		for (Clone* clone : clones)
		{
			clone->targetPosition = RotateSoldier(clone->targetPosition, angleChange);
		}
		currentRotation = angle;
		rotating = true;
	}
	else if (moved == false && startedMoving == false) // same as rotating but for movement
	{
		glm::vec2 moveVec = glm::vec2(armyDesiredPosition.x - armyCurrentPosition.x, armyDesiredPosition.y - armyCurrentPosition.y);
		if (moveVec != glm::vec2{ 0, 0 })
		{
			moveVec = glm::normalize(moveVec);
		}

		float distance = glm::distance(armyDesiredPosition, armyCurrentPosition);
		for (Clone* clone : clones)
		{
			clone->targetPosition = clone->targetPosition + (moveVec * distance);
		}
		armyCurrentPosition = armyDesiredPosition;
		startedMoving = true;
	}
	else if (rotated == true && rotating == true && moved == true && startedMoving == true && hasNewOrders == true) // when we are done with this step we can change stance if needed
	{
		updateStance();
		hasNewOrders = false;
	}
	else if (rotated == true && rotating == true && moved == true && startedMoving == true) // when all is true then move to next step
	{
		routeIndex++;
		if (routeIndex < myPath.size())
		{
			for (Clone* clone : clones)
			{
				clone->inPosition = false;
			}
			rotated = false;
			rotating = false;
			moved = false;
			startedMoving = false;

			armyDesiredPosition = myPath[routeIndex]->position;
		}
		else
		{
			changeStance(Stance::Hold, armyCurrentPosition);
		}
	}
}

// Name: RotateSoldier
// Parameter: Takes a vec2 which will be the position of the soldier that is being rotated, and a float which is the angle to rotate by
// Description: Rotates the soldiers position around the armies current position
// Return Value:

glm::vec2 CloneArmy::RotateSoldier(glm::vec2 soldierPos, float angle)
{
	for (Clone* clone : clones)
	{
		clone->inPosition = false;
	}

	float s = sin(angle);
	float c = cos(angle);

	// translate point back to origin:
	soldierPos.x -= armyCurrentPosition.x;
	soldierPos.y -= armyCurrentPosition.y;

	// rotate point
	float xNew = soldierPos.x * c - soldierPos.y * s;
	float yNew = soldierPos.x * s + soldierPos.y * c;

	// translate point back:
	soldierPos.x = xNew + armyCurrentPosition.x;
	soldierPos.y = yNew + armyCurrentPosition.y;

	return soldierPos;
}

// Name: FindMyPath
// Parameter: 
// Description: Finds a path
// Return Value:

void CloneArmy::GetMyPath()
{
	myPath = myManager->pathFinder.Pathfinding(myManager->mapCopy.GetNearestNode(armyCurrentPosition), myManager->mapCopy.GetNearestNode(armyDesiredPosition), armyWidth);
}

// Name: Hold
// Parameter: 
// Description: Finds nearest eligbl army to face
// Return Value:

void CloneArmy::Hold()
{
	if (hasNewOrders == false)
	{
		if (rotating == false)
		{
			rotating = true;
			float distance = FLT_MAX;
			glm::vec2 targetPosition = { 0, 0 };
			if (myManager->cloneArmies.size() > 0)
			{
				for (CloneArmy* army : myManager->cloneArmies) // Find any elligible armies to face and turn
				{
					if (army->managerIndex != managerIndex && HasLineOfSight(army) && army->teamNumber != teamNumber) // hold changes
					{
						if (glm::distance(armyCurrentPosition, army->armyCurrentPosition) < distance)
						{
							distance = glm::distance(armyCurrentPosition, army->armyCurrentPosition);
							targetPosition = army->armyCurrentPosition;
						}
					}
				}
				if (targetPosition != glm::vec2{ 0,0 })
				{
					glm::vec2 desiredDirection = -glm::vec2{ targetPosition.y - armyCurrentPosition.y, targetPosition.x - armyCurrentPosition.x };
					float angle = atan2(desiredDirection.x, desiredDirection.y); // radians

					float angleChange = -(currentRotation - angle);

					for (Clone* clone : clones)
					{
						clone->targetPosition = RotateSoldier(clone->targetPosition, angleChange);
					}
					currentRotation = angle;
				}
			}
		}
		else if (rotating == true && rotated == true)
		{
			rotating = false;
			rotated = false;
		}
	}
	else
	{
		updateStance();
		hasNewOrders = false;
	}
}

// Name: AttackThis
// Parameter: 
// Description: Chases target army
// Return Value:

void CloneArmy::AttackThis()
{
	if (targetArmy != nullptr)
	{
		if (HasLineOfSight(targetArmy) && hasNewOrders == false && glm::distance(targetArmy->armyCurrentPosition, armyCurrentPosition) < targetArmy->armyWidth + armyWidth * 3)
		{
			armyDesiredPosition = targetArmy->armyCurrentPosition;
			Move();
		}
		else if (hasNewOrders == true)
		{
			updateStance();
			hasNewOrders = false;
		}
		else
		{
			Move();
		}
	}
}

// Name: Attack
// Parameter: 
// Description: Finds nearest army to chase
// Return Value:

void CloneArmy::Attack()
{
	if (targetArmy == nullptr) // If we don't have a target, find one
	{
		float distance = FLT_MAX;
		for (CloneArmy* army : myManager->cloneArmies)
		{
			if (HasLineOfSight(army) && army->teamNumber != teamNumber && army->stance != Stance::Flee && distance > glm::distance(army->armyCurrentPosition, newStance.second))
			{ // Can't be out of sight or on our team or be us
				distance = glm::distance(army->armyCurrentPosition, newStance.second);
				targetArmy = army;
			}
		}
	}
	if (targetArmy == nullptr)
	{
		changeStance(newStance.first, newStance.second);
	}
	else
	{ // if they move to far then re-route
		if (myPath.empty() || glm::distance(targetArmy->armyCurrentPosition, glm::vec2{ myPath[myPath.size() - 1]->position.x, myPath[myPath.size() - 1]->position.y }) > 5 + targetArmy->armyWidth)
		{
			armyDesiredPosition = targetArmy->armyCurrentPosition;

			GetMyPath();

			if (myPath.size() != 0)
			{
				armyDesiredPosition = targetArmy->armyCurrentPosition;
				armyDesiredPosition = myPath[0]->position;
			}
			else
			{
				armyDesiredPosition = armyCurrentPosition;
				changeStance(Stance::Attack, armyCurrentPosition);
			}
		}
		if (HasLineOfSight(targetArmy) && hasNewOrders == false && glm::distance(targetArmy->armyCurrentPosition, armyCurrentPosition) < targetArmy->armyWidth + armyWidth * 3)
		{
			armyDesiredPosition = targetArmy->armyCurrentPosition;
			Move();
		}
		else if (hasNewOrders == true)
		{
			updateStance();
			hasNewOrders = false;
		}
		else
		{
			Move();
		}
	}
}

// Name: Flee
// Parameter: 
// Description: Gets nearest enemy army to run from
// Return Value:

void CloneArmy::Flee()
{
	float distance = FLT_MAX;
	for (CloneArmy* army : myManager->cloneArmies)
	{
		if (HasLineOfSight(army) && army->teamNumber != teamNumber && army->stance != Stance::Flee && distance > glm::distance(army->armyCurrentPosition, newStance.second))
		{
			distance = glm::distance(army->armyCurrentPosition, newStance.second);
			targetArmy = army;
		}
	}
}

// Name: HasLineOfSight
// Parameter: Takes a CloneArmy pointer which is what will be checked for line of sight
// Description: Checks if there is no obstacles between the the army calling the method and the parameter army
// Return Value: Returns true if there is line of sight

bool CloneArmy::HasLineOfSight(CloneArmy* army)
{
	bool lineOfSight = true;

	glm::vec2 armyOne;
	glm::vec2 armyTwo;

	if (armyCurrentPosition.y > army->armyCurrentPosition.y) // armyOne will always be the army one top
	{
		armyOne = { armyCurrentPosition.x, armyCurrentPosition.y };
		armyTwo = { army->armyCurrentPosition.x, army->armyCurrentPosition.y };
	}
	else
	{
		armyOne = { army->armyCurrentPosition.x, army->armyCurrentPosition.y };
		armyTwo = { armyCurrentPosition.x, armyCurrentPosition.y };
	}

	glm::vec2 directionVector = { armyTwo.x - armyOne.x, armyTwo.y - armyOne.y };
	glm::vec2 directionNormal = glm::normalize(directionVector);

	if (armyOne.x > armyTwo.x) // Check if we need get a perpendicular vector
	{
		directionNormal = glm::vec2{ directionNormal.y, -directionNormal.x };
	}

	glm::vec2 armyLeft = armyOne - (directionNormal * (armyWidth * 1.5f)); // Set offset of SightPlane positions
	glm::vec2 armyRight = armyOne + (directionNormal * (armyWidth * 1.5f));
	glm::vec2 otherArmyLeft = armyTwo - (directionNormal * (army->armyWidth * 1.5f));
	glm::vec2 otherArmyRight = armyTwo + (directionNormal * (army->armyWidth * 1.5f));

	SightPlane leftSightPlane = SightPlane(armyLeft.x, armyLeft.y, otherArmyLeft.x, otherArmyLeft.y);
	SightPlane rightSightPlane = SightPlane(otherArmyRight.x, otherArmyRight.y, armyRight.x, armyRight.y);

	SightPlane sides[2] = { leftSightPlane, rightSightPlane };

	float maxX = -FLT_MAX;
	float minX = FLT_MAX;

	float maxY = -FLT_MAX;
	float minY = FLT_MAX;

	glm::vec2 SightPlanePoints[4] = { leftSightPlane.posA, leftSightPlane.posB, rightSightPlane.posA, rightSightPlane.posB }; // Create boundries for what she need to bother checking

	for (glm::vec2 vec : SightPlanePoints)
	{
		if (vec.x > maxX)
		{
			maxX = vec.x;
		}
		if (vec.x < minX)
		{
			minX = vec.x;
		}
		if (vec.y > maxY)
		{
			maxY = vec.y;
		}
		if (vec.y < minY)
		{
			minY = vec.y;
		}
	}

	for (Obstacle obstacle : myManager->mapCopy.obstacles)
	{
		int behindSightPlane = 0;
		if (obstacle.position.y >= minY && obstacle.position.y <= maxY && obstacle.position.x >= minX && obstacle.position.x <= maxX) // If they're near
		{
			for (SightPlane& SightPlane : sides)
			{
				std::vector<glm::vec2> points; // obstacle corners

				points.push_back({ obstacle.minX, obstacle.minY });                       // This is all AABB and SightPlane collision
				points.push_back({ obstacle.maxX, obstacle.minY });
				points.push_back({ obstacle.maxX, obstacle.maxY });
				points.push_back({ obstacle.minX, obstacle.maxY });

				float distanceToSightPlane = -FLT_MAX;
				float depth = -1;

				bool touching = false;

				for (glm::vec2 point : points)
				{
					distanceToSightPlane = -1 * (glm::dot(point, SightPlane.normal) + SightPlane.d); 
					if (distanceToSightPlane >= 0)
					{
						if (distanceToSightPlane > depth)
						{
							depth = distanceToSightPlane;
							touching = true;
						}
					}
				}
				if (touching == true)
				{
					behindSightPlane++;
				}
			}
		}
		if (behindSightPlane == 2) // If any where between the SightPlanes, we dont have line of sight
		{
			lineOfSight = false;
		}
	}

	return lineOfSight;
}
