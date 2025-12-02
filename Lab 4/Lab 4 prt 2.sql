SELECT P.name, F.depature_point, F.place_of_arrival
FROM PASSENGER P
JOIN PASSENGER_FLIGHT PF ON P.passenger_id = PF.passenger_id
JOIN FLIGHT F ON PF.flight_id = F.flight_id;

SELECT P.name, PF.flight_id
FROM PASSENGER P
LEFT JOIN PASSENGER_FLIGHT PF ON P.passenger_id = PF.passenger_id;

SELECT PF.passenger_id, F.flight_id
FROM PASSENGER_FLIGHT PF
RIGHT JOIN FLIGHT F ON PF.flight_id = F.flight_id;