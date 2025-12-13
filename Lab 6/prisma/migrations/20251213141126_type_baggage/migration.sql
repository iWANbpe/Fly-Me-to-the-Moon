/*
  Warnings:

  - Added the required column `type` to the `baggage` table without a default value. This is not possible if the table is not empty.

*/
-- AlterTable
ALTER TABLE "baggage" ADD COLUMN     "type" VARCHAR(32) NOT NULL,
ALTER COLUMN "max_weight" SET DATA TYPE INTEGER;
