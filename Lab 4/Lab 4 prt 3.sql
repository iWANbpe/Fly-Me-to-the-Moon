SELECT PF.passenger_id
FROM PASSENGER_FLIGHT PF
JOIN FLIGHT F ON PF.flight_id = F.flight_id
JOIN SPACESHIP S ON F.spaceship_name = S.spaceship_name
WHERE S.spaceship_name = 'Kadenyuk 1'

SELECT
	F.FLIGHT_ID,
	(
		SELECT COUNT(*)
		FROM
			CONTAINER_FLIGHT CF
		WHERE
			CF.FLIGHT_ID = F.FLIGHT_ID
	) AS CONTAINERS
FROM
	FLIGHT F;

SELECT S.name AS ship, SUM(C.mass) AS total_cargo_mass
FROM SPACESHIP S
JOIN FLIGHT F ON S.spaceship_id = F.spaceship_id
JOIN CARGO_CONTAINER C ON C.flight_id = F.flight_id
GROUP BY S.name;