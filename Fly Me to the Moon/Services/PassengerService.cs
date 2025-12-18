using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Microsoft.EntityFrameworkCore;

namespace Fly_Me_to_the_Moon.Services
{
    public class PassengerService
    {
        private readonly SpaceFlightContext _context;

        public PassengerService(SpaceFlightContext context)
        {
            _context = context;
        }

        public async Task<Passenger> CreatePassengerWithDetails(PassengerRegistryRequestDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            Passenger passengerResult = null;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var insurance = new Insurance
                    {
                        ExpireBy = dto.InsuranceDetails.ExpireBy,
                        CompanyGrantedBy = dto.InsuranceDetails.CompanyGrantedBy,
                        RowVersion = 1
                    };

                    var healthAnalysis = new FullHealthAnalysisResult
                    {
                        ExpireBy = dto.FullHealthAnalysisResultDetails.ExpireBy,
                        AllowedToFly = dto.FullHealthAnalysisResultDetails.AllowedToFly,
                        GrantedBy = dto.FullHealthAnalysisResultDetails.GrantedBy,
                        RowVersion = 1
                    };

                    passengerResult = new Passenger
                    {
                        Name = dto.Name,
                        PhoneNumber = dto.PhoneNumber,
                        Email = dto.Email,
                        RowVersion = 1,
                        Insurance = insurance,
                        InsuranceId = insurance.InsuranceId,
                        FullHealthAnalysisResult = healthAnalysis,
                        AnalysisId = healthAnalysis.AnalysisId
                    };

                    insurance.Passenger = passengerResult;
                    healthAnalysis.Passenger = passengerResult;

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

        public async Task<Passenger> UpdatePassengerAndLinked(int passengerId, UpdatePassengerDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            Passenger updatedPassenger = null;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var passengerToUpdate = await _context.Passenger
                        .Include(p => p.Insurance)
                        .Include(p => p.FullHealthAnalysisResult)
                        .FirstOrDefaultAsync(p => p.PassengerId == passengerId);

                    if (passengerToUpdate == null)
                    {
                        throw new KeyNotFoundException($"Passenger with ID {passengerId} not found.");
                    }

                    passengerToUpdate.Name = dto.Name;
                    passengerToUpdate.PhoneNumber = dto.PhoneNumber;
                    passengerToUpdate.Email = dto.Email;

                    _context.Entry(passengerToUpdate).Property(p => p.RowVersion).OriginalValue = dto.PassengerRowVersion;
                    passengerToUpdate.RowVersion = dto.PassengerRowVersion + 1;

                    if (passengerToUpdate.Insurance != null)
                    {
                        var ins = passengerToUpdate.Insurance;
                        ins.ExpireBy = dto.InsuranceDetails.ExpireBy;
                        ins.CompanyGrantedBy = dto.InsuranceDetails.CompanyGrantedBy;
                        _context.Entry(ins).Property(i => i.RowVersion).OriginalValue = dto.InsuranceDetails.InsuranceRowVersion;
                        ins.RowVersion = dto.InsuranceDetails.InsuranceRowVersion + 1;
                    }

                    if (passengerToUpdate.FullHealthAnalysisResult != null)
                    {
                        var fhar = passengerToUpdate.FullHealthAnalysisResult;
                        fhar.ExpireBy = dto.FHARDetails.ExpireBy;
                        fhar.AllowedToFly = dto.FHARDetails.AllowedToFly;
                        fhar.GrantedBy = dto.FHARDetails.GrantedBy;
                        _context.Entry(fhar).Property(f => f.RowVersion).OriginalValue = dto.FHARDetails.FHARRowVersion;
                        fhar.RowVersion = dto.FHARDetails.FHARRowVersion + 1;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    updatedPassenger = passengerToUpdate;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    await transaction.RollbackAsync();
                    string error = "Update failed. The record (Passenger, Insurance or FHAR) has been modified by another user. Please refresh and try again.";
                    throw new InvalidOperationException(error, ex);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            return updatedPassenger;
        }

        public async Task<List<PassengerInsuranceDto>> GetPassengersWithExpiredInsurance(ExpiredInsuranceFilterCriteriaDto filter, int pageNumber = 1, int pageSize = 15)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 15;

            int skip = (pageNumber - 1) * pageSize;

            var query = from p in _context.Passenger
                        join i in _context.Insurance on p.InsuranceId equals i.InsuranceId
                        where i.ExpireBy < filter.MaxExpiryDate
                        select new { p, i };


            if (!string.IsNullOrWhiteSpace(filter.CompanyName))
            {
                string companyNameLower = filter.CompanyName.ToLower();
                query = query.Where(x => x.i.CompanyGrantedBy.ToLower().Contains(companyNameLower));
            }

            var result = await query
                .OrderBy(x => x.i.ExpireBy)
                .Skip(skip)
                .Take(pageSize)
                .Select(x => new PassengerInsuranceDto
                {
                    PassengerId = x.p.PassengerId,
                    Name = x.p.Name,
                    Email = x.p.Email,
                    Insurance = new InsuranceDto
                    {
                        ExpireBy = x.i.ExpireBy,
                        CompanyGrantedBy = x.i.CompanyGrantedBy
                    }
                })
                .ToListAsync();

            return result;
        }
    }
}