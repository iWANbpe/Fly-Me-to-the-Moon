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
            // 1. ОТРИМАННЯ СТРАТЕГІЇ ВИКОНАННЯ
            var strategy = _context.Database.CreateExecutionStrategy();

            // 2. ЗАПУСК ВСІЄЇ ЛОГІКИ У БЛОЦІ СТРАТЕГІЇ
            // Ця перевантаження ExecuteAsync не повертає значення, тому ми повертаємо PassengerResult
            // УВАГА: Ми повинні ініціалізувати PassengerResult всередині блоку для коректного мапінгу

            Passenger passengerResult = null; // Буде містити створений об'єкт

            await strategy.ExecuteAsync(async () =>
            {
                // Транзакція автоматично починається тут
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    // 3. БІЗНЕС-ЛОГІКА ТА ВАЛІДАЦІЯ
                    if (!dto.FullHealthAnalysisResultDetails.AllowedToFly)
                    {
                        // Цей виняток буде перехоплено, транзакція відкотиться, і клієнт отримає 400 Bad Request.
                        throw new InvalidOperationException("Passenger is not allowed to fly based on health analysis.");
                    }

                    // 4. ПОВНИЙ МЕХАНІЗМ МАПІНГУ ТА ІНІЦІАЛІЗАЦІЇ

                    // Створення дочірніх об'єктів
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

            // 7. Повертаємо створений об'єкт Passenger (він вже має згенеровані ID)
            return passengerResult;
        }
    }
}