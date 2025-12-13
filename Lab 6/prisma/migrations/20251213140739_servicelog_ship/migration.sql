-- CreateTable
CREATE TABLE "ServiceLog" (
    "log_id" SERIAL NOT NULL,
    "date" TIMESTAMP(3) NOT NULL DEFAULT CURRENT_TIMESTAMP,
    "description" VARCHAR(512) NOT NULL,
    "spaceshipName" TEXT NOT NULL,

    CONSTRAINT "ServiceLog_pkey" PRIMARY KEY ("log_id")
);

-- CreateIndex
CREATE UNIQUE INDEX "ServiceLog_spaceshipName_key" ON "ServiceLog"("spaceshipName");

-- AddForeignKey
ALTER TABLE "ServiceLog" ADD CONSTRAINT "ServiceLog_spaceshipName_fkey" FOREIGN KEY ("spaceshipName") REFERENCES "spaceship"("spaceship_name") ON DELETE CASCADE ON UPDATE CASCADE;
