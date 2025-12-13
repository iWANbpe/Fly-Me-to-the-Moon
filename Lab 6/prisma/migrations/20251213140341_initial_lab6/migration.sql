-- CreateTable
CREATE TABLE "baggage" (
    "baggage_id" SERIAL NOT NULL,
    "max_weight" SMALLINT NOT NULL,
    "passenger_id" INTEGER,
    "container_id" INTEGER NOT NULL,

    CONSTRAINT "baggage_pkey" PRIMARY KEY ("baggage_id")
);

-- CreateTable
CREATE TABLE "container" (
    "container_id" SERIAL NOT NULL,
    "max_weight" INTEGER NOT NULL,

    CONSTRAINT "container_pkey" PRIMARY KEY ("container_id")
);

-- CreateTable
CREATE TABLE "container_flight" (
    "container_id" INTEGER NOT NULL,
    "flight_id" INTEGER NOT NULL,

    CONSTRAINT "container_flight_pkey" PRIMARY KEY ("container_id","flight_id")
);

-- CreateTable
CREATE TABLE "flight" (
    "flight_id" SERIAL NOT NULL,
    "departure_point" VARCHAR(32) NOT NULL,
    "departure_date" TIMESTAMP(6) NOT NULL,
    "place_of_arrival" VARCHAR(32) NOT NULL,
    "arrival_date" TIMESTAMP(6) NOT NULL,
    "spaceship_name" VARCHAR(64),

    CONSTRAINT "flight_pkey" PRIMARY KEY ("flight_id")
);

-- CreateTable
CREATE TABLE "full_health_analysis_result" (
    "analysis_id" SERIAL NOT NULL,
    "expire_by" DATE NOT NULL,
    "allowed_to_fly" BOOLEAN NOT NULL,
    "granted_by" VARCHAR(32) NOT NULL,

    CONSTRAINT "full_health_analysis_result_pkey" PRIMARY KEY ("analysis_id")
);

-- CreateTable
CREATE TABLE "insurance" (
    "insurance_id" SERIAL NOT NULL,
    "expire_by" DATE NOT NULL,
    "company_granted_by" VARCHAR(32) NOT NULL,

    CONSTRAINT "insurance_pkey" PRIMARY KEY ("insurance_id")
);

-- CreateTable
CREATE TABLE "passenger" (
    "passenger_id" SERIAL NOT NULL,
    "name" VARCHAR(32) NOT NULL,
    "phone_number" VARCHAR(32) NOT NULL,
    "email" VARCHAR(32) NOT NULL,
    "analysis_id" INTEGER NOT NULL,
    "insurance_id" INTEGER NOT NULL,

    CONSTRAINT "passenger_pkey" PRIMARY KEY ("passenger_id")
);

-- CreateTable
CREATE TABLE "passenger_flight" (
    "passenger_id" INTEGER NOT NULL,
    "flight_id" INTEGER NOT NULL,

    CONSTRAINT "passenger_flight_pkey" PRIMARY KEY ("passenger_id","flight_id")
);

-- CreateTable
CREATE TABLE "robot" (
    "robot_id" SERIAL NOT NULL,
    "robot_model" VARCHAR(32) NOT NULL,
    "container_id" INTEGER NOT NULL,

    CONSTRAINT "robot_pkey" PRIMARY KEY ("robot_id")
);

-- CreateTable
CREATE TABLE "robot_model_catalog" (
    "robot_model" VARCHAR(32) NOT NULL,
    "function" VARCHAR(128) NOT NULL,
    "weight" SMALLINT NOT NULL,

    CONSTRAINT "robot_model_catalog_pkey" PRIMARY KEY ("robot_model")
);

-- CreateTable
CREATE TABLE "spaceship" (
    "spaceship_name" VARCHAR(64) NOT NULL,
    "date_of_manufacture" DATE NOT NULL,
    "passenger_capacity" SMALLINT NOT NULL,
    "containers_capacity" SMALLINT NOT NULL,

    CONSTRAINT "spaceship_pkey" PRIMARY KEY ("spaceship_name")
);

-- CreateIndex
CREATE UNIQUE INDEX "baggage_passenger_id_key" ON "baggage"("passenger_id");

-- CreateIndex
CREATE UNIQUE INDEX "passenger_analysis_id_key" ON "passenger"("analysis_id");

-- CreateIndex
CREATE UNIQUE INDEX "passenger_insurance_id_key" ON "passenger"("insurance_id");

-- AddForeignKey
ALTER TABLE "baggage" ADD CONSTRAINT "baggage_container_id_fkey" FOREIGN KEY ("container_id") REFERENCES "container"("container_id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- AddForeignKey
ALTER TABLE "baggage" ADD CONSTRAINT "baggage_passenger_id_fkey" FOREIGN KEY ("passenger_id") REFERENCES "passenger"("passenger_id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- AddForeignKey
ALTER TABLE "container_flight" ADD CONSTRAINT "container_flight_container_id_fkey" FOREIGN KEY ("container_id") REFERENCES "container"("container_id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- AddForeignKey
ALTER TABLE "container_flight" ADD CONSTRAINT "container_flight_flight_id_fkey" FOREIGN KEY ("flight_id") REFERENCES "flight"("flight_id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- AddForeignKey
ALTER TABLE "flight" ADD CONSTRAINT "flight_spaceship_name_fkey" FOREIGN KEY ("spaceship_name") REFERENCES "spaceship"("spaceship_name") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- AddForeignKey
ALTER TABLE "passenger" ADD CONSTRAINT "passenger_analysis_id_fkey" FOREIGN KEY ("analysis_id") REFERENCES "full_health_analysis_result"("analysis_id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- AddForeignKey
ALTER TABLE "passenger" ADD CONSTRAINT "passenger_insurance_id_fkey" FOREIGN KEY ("insurance_id") REFERENCES "insurance"("insurance_id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- AddForeignKey
ALTER TABLE "passenger_flight" ADD CONSTRAINT "passenger_flight_flight_id_fkey" FOREIGN KEY ("flight_id") REFERENCES "flight"("flight_id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- AddForeignKey
ALTER TABLE "passenger_flight" ADD CONSTRAINT "passenger_flight_passenger_id_fkey" FOREIGN KEY ("passenger_id") REFERENCES "passenger"("passenger_id") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- AddForeignKey
ALTER TABLE "robot" ADD CONSTRAINT "fk_robot_model" FOREIGN KEY ("robot_model") REFERENCES "robot_model_catalog"("robot_model") ON DELETE NO ACTION ON UPDATE NO ACTION;

-- AddForeignKey
ALTER TABLE "robot" ADD CONSTRAINT "robot_container_id_fkey" FOREIGN KEY ("container_id") REFERENCES "container"("container_id") ON DELETE NO ACTION ON UPDATE NO ACTION;
