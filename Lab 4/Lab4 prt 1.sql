SELECT COUNT(*) AS total_passengers
FROM PASSENGER;

SELECT passenger_id, COUNT(*) AS baggage_count
FROM BAGGAGE
GROUP BY passenger_id;

SELECT PF.flight_id, COUNT(B.baggage_id) AS baggage_on_flight
FROM PASSENGER_FLIGHT PF
LEFT JOIN BAGGAGE B ON B.passenger_id = PF.passenger_id
GROUP BY PF.flight_id;

SELECT flight_id, COUNT(*) AS num_bags
FROM BAGGAGE B
JOIN PASSENGER P ON P.passenger_id = B.passenger_id
JOIN PASSENGER_FLIGHT PF ON PF.passenger_id = P.passenger_id
GROUP BY flight_id
HAVING COUNT(*) > 2;