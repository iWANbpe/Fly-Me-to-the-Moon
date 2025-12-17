using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Microsoft.EntityFrameworkCore;

namespace Fly_Me_to_the_Moon.Services
{
    public class BookingService
    {
        private readonly SpaceFlightContext _context;
        private const int DefaultMaxWeightKg = 60;

        public BookingService(SpaceFlightContext context)
        {
            _context = context;
        }

        public async Task<Passenger> CreatePassengerWithDetails(BookingRequestDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            Passenger passengerResult = null;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    if (!dto.FullHealthAnalysisResultDetails.AllowedToFly)
                    {
                        throw new InvalidOperationException("Passenger is not allowed to fly based on health analysis.");
                    }
                    var insurance = new Insurance
                    {
                        ExpireBy = dto.InsuranceDetails.ExpireBy,
                        CompanyGrantedBy = dto.InsuranceDetails.CompanyGrantedBy
                    };

                    var healthAnalysis = new FullHealthAnalysisResult
                    {
                        ExpireBy = dto.FullHealthAnalysisResultDetails.ExpireBy,
                        AllowedToFly = dto.FullHealthAnalysisResultDetails.AllowedToFly,
                        GrantedBy = dto.FullHealthAnalysisResultDetails.GrantedBy
                    };

                    if (dto.BaggageDetails != null)
                    {
                        var baggage = new Baggage
                        {
                            Type = dto.BaggageDetails.Type,
                            MaxWeight = DefaultMaxWeightKg
                        };

                        passengerResult = new Passenger
                        {
                            Name = dto.Name,
                            PhoneNumber = dto.PhoneNumber,
                            Email = dto.Email,

                            Insurance = insurance,
                            InsuranceId = insurance.InsuranceId,
                            FullHealthAnalysisResult = healthAnalysis,
                            AnalysisId = healthAnalysis.AnalysisId,
                            Baggage = baggage
                        };
                    }

                    else
                    {
                        passengerResult = new Passenger
                        {
                            Name = dto.Name,
                            PhoneNumber = dto.PhoneNumber,
                            Email = dto.Email,

                            Insurance = insurance,
                            InsuranceId = insurance.InsuranceId,
                            FullHealthAnalysisResult = healthAnalysis,
                            AnalysisId = healthAnalysis.AnalysisId
                        };
                    }

                    _context.Passenger.Add(passengerResult);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (InvalidOperationException)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"FATAL TRANSACTION ERROR: {ex.Message}");

                    throw new ApplicationException("Passenger creation failed. Transaction rolled back.", ex);
                }
            });

            return passengerResult;
        }

        public async Task<bool> DeletePassengerWithDetails(int passengerId)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            bool deletionSuccessful = false;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var passenger = await _context.Passenger
                        .Include(p => p.Insurance)
                        .Include(p => p.FullHealthAnalysisResult)
                        .Include(p => p.Baggage)
                        .FirstOrDefaultAsync(p => p.PassengerId == passengerId);

                    if (passenger == null)
                    {
                        await transaction.CommitAsync();
                        return;
                    }

                    if (passenger.Insurance != null)
                    {
                        _context.Insurance.Remove(passenger.Insurance);
                    }

                    if (passenger.FullHealthAnalysisResult != null)
                    {
                        _context.FullHealthAnalysisResult.Remove(passenger.FullHealthAnalysisResult);
                    }

                    if (passenger.Baggage != null)
                    {
                        _context.Baggage.Remove(passenger.Baggage);
                    }

                    _context.Passenger.Remove(passenger);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    deletionSuccessful = true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"FATAL DELETION ERROR: {ex.Message}");
                    throw new ApplicationException($"Passenger deletion failed. Transaction rolled back. Details: {ex.Message}", ex);
                }
            });

            return deletionSuccessful;
        }
    }
}