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

SELECT S.spaceship_name AS ship, SUM(C.max_weight) AS total_container_mass
FROM SPACESHIP S
JOIN FLIGHT F ON S.spaceship_name = F.spaceship_name
JOIN CONTAINER_FLIGHT CF ON CF.flight_id = F.flight_id
JOIN CONTAINER C ON C.container_id = CF.container_id
GROUP BY S.spaceship_name;